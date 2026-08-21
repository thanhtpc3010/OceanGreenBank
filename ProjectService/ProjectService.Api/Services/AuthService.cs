using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using MediatR;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using ProjectService.Api.Models;
using ProjectService.Application.Common.Interfaces;
using ProjectService.Application.Services.DTOs;
using ProjectService.Application.Services.Queries;
using ProjectService.Domain.Entity;
using ProjectService.Domain.Exceptions;

namespace ProjectService.Api.Services;

/// <summary>Cấu hình JWT từ appsettings.</summary>
public class JwtSettings
{
    public string Key { get; set; } = "";
    public string Issuer { get; set; } = "";
    public string Audience { get; set; } = "";
    public int ExpiryMinutes { get; set; } = 480;
}

/// <summary>
/// Dịch vụ xác thực: đăng nhập, đăng ký, tạo JWT.
/// Mật khẩu được xác thực bằng BCrypt (hỗ trợ cả legacy plaintext cũ).
/// </summary>
public class AuthService
{
    private readonly JwtSettings _jwt;
    private readonly IReadRepository<User> _userRepository;
    private readonly IWriteRepository<User> _userWriter;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMediator _mediator;

    public AuthService(
        IOptions<JwtSettings> jwt,
        IReadRepository<User> userRepository,
        IWriteRepository<User> userWriter,
        IUnitOfWork unitOfWork,
        IMediator mediator)
    {
        _jwt = jwt.Value;
        _userRepository = userRepository;
        _userWriter = userWriter;
        _unitOfWork = unitOfWork;
        _mediator = mediator;
    }

    /// <summary>Đăng nhập — kiểm tra email/password, trả JWT + thông tin user.</summary>
    public async Task<AuthResponse> LoginAsync(string email, string password, CancellationToken ct)
    {
        var users = await _userRepository.FindAsync(u => u.Email.ToLower() == email.Trim().ToLower(), ct);
        var user = users.FirstOrDefault()
            ?? throw new DomainException("Email hoặc mật khẩu không đúng.");

        if (!user.IsActive)
            throw new DomainException("Tài khoản đã bị khóa. Vui lòng liên hệ quản trị viên.");

        // Xác thực mật khẩu (BCrypt hoặc legacy plaintext) + nâng cấp lên hash nếu cần.
        var valid = user.PasswordHash.StartsWith("$2")
            ? BCrypt.Net.BCrypt.Verify(password, user.PasswordHash)
            : user.PasswordHash == password;

        if (!valid)
            throw new DomainException("Email hoặc mật khẩu không đúng.");

        if (!user.PasswordHash.StartsWith("$2"))
        {
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(password);
            _userWriter.Update(user);
            await _unitOfWork.SaveChangesAsync(ct);
        }

        return await BuildResponseAsync(user, ct);
    }

    /// <summary>Đăng ký — tạo user mới (mật khẩu hash) rồi trả JWT.</summary>
    public async Task<AuthResponse> RegisterAsync(RegisterRequest request, CancellationToken ct)
    {
        var existing = await _userRepository.FindAsync(
            u => u.Email.ToLower() == request.Email.Trim().ToLower(), ct);
        if (existing.Any())
            throw new DuplicateException($"Email {request.Email} đã được đăng ký.");

        var user = new User
        {
            Id = Guid.NewGuid().ToString("N"),
            CreatedDate = DateTime.UtcNow,
            CreatedBy = null,
            FullName = request.FullName,
            Email = request.Email,
            Phone = request.Phone,
            IdentityCard = request.IdentityCard,
            DateOfBirth = DateTime.SpecifyKind(request.DateOfBirth, DateTimeKind.Utc),
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            Address = request.Address
        };

        await _userWriter.AddAsync(user, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        return await BuildResponseAsync(user, ct);
    }

    /// <summary>Tạo AuthResponse: lấy roles/permissions + tạo JWT.</summary>
    private async Task<AuthResponse> BuildResponseAsync(User user, CancellationToken ct)
    {
        var perms = await _mediator.Send(new GetUserPermissionsQuery(user.Id), ct);
        var roles = perms.Roles.Select(r => r.Code).Where(c => c != null).Cast<string>().ToList();

        var expiresAt = DateTime.UtcNow.AddMinutes(_jwt.ExpiryMinutes);
        var token = GenerateToken(user, roles, perms.PermissionCodes, expiresAt);

        return new AuthResponse(
            token,
            expiresAt,
            new AuthUserDto(user.Id, user.FullName, user.Email, roles, perms.PermissionCodes));
    }

    private string GenerateToken(
        User user,
        IReadOnlyList<string> roles,
        IReadOnlyList<string> permissions,
        DateTime expiresAt)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id),
            new(ClaimTypes.Email, user.Email),
            new(ClaimTypes.Name, user.FullName),
        };
        foreach (var role in roles)
            claims.Add(new Claim(ClaimTypes.Role, role));
        foreach (var perm in permissions)
            claims.Add(new Claim("permission", perm));

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Key));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _jwt.Issuer,
            audience: _jwt.Audience,
            claims: claims,
            expires: expiresAt,
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
