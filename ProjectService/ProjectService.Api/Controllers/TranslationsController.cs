using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectService.Application.Common.Interfaces;
using ProjectService.Domain.Entity;
using ProjectService.Domain.Exceptions;

namespace ProjectService.Api.Controllers;

/// <summary>DTO tạo/cập nhật bản dịch (admin).</summary>
public class TranslationRequest
{
    public string Key { get; set; } = "";
    public string Language { get; set; } = "";
    public string Value { get; set; } = "";
}

/// <summary>DTO trả về một bản dịch.</summary>
public class TranslationDto
{
    public string Id { get; set; } = "";
    public string Key { get; set; } = "";
    public string Language { get; set; } = "";
    public string Value { get; set; } = "";
}

/// <summary>
/// Quản lý bản dịch giao diện (i18n) lưu trong DB.
///   - GET    /api/translations/{language} → tải bản dịch (ẩn danh — login page cần)
///   - GET    /api/translations            → danh sách (admin)
///   - POST   /api/translations            → thêm (admin)
///   - PUT    /api/translations/{id}       → sửa (admin)
///   - DELETE /api/translations/{id}       → xóa (admin)
/// </summary>
[ApiController]
[Route("api/translations")]
public class TranslationsController : ControllerBase
{
    private readonly IReadRepository<Translation> _reader;
    private readonly IWriteRepository<Translation> _writer;
    private readonly IUnitOfWork _unitOfWork;

    public TranslationsController(
        IReadRepository<Translation> reader,
        IWriteRepository<Translation> writer,
        IUnitOfWork unitOfWork)
    {
        _reader = reader;
        _writer = writer;
        _unitOfWork = unitOfWork;
    }

    /// <summary>Tải toàn bộ bản dịch của một ngôn ngữ (vi/en) — không cần đăng nhập.</summary>
    [AllowAnonymous]
    [HttpGet("{language}")]
    public async Task<ActionResult<Dictionary<string, string>>> GetByLanguage(string language, CancellationToken ct)
    {
        var items = await _reader.FindAsync(t => t.Language == language, ct);
        return Ok(items.ToDictionary(t => t.Key, t => t.Value));
    }

    /// <summary>Danh sách toàn bộ bản dịch (admin).</summary>
    [Authorize(Policy = "AdminOnly")]
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<TranslationDto>>> GetAll(CancellationToken ct)
    {
        var items = await _reader.ListAsync(ct);
        return Ok(items
            .OrderBy(t => t.Key)
            .ThenBy(t => t.Language)
            .Select(ToDto)
            .ToList());
    }

    /// <summary>Thêm bản dịch mới (admin).</summary>
    [Authorize(Policy = "AdminOnly")]
    [HttpPost]
    public async Task<ActionResult<TranslationDto>> Create([FromBody] TranslationRequest request, CancellationToken ct)
    {
        var item = new Translation
        {
            Id = Guid.NewGuid().ToString("N"),
            CreatedDate = DateTime.UtcNow,
            CreatedBy = "admin",
            Key = request.Key.Trim(),
            Language = request.Language.Trim(),
            Value = request.Value,
        };

        await _writer.AddAsync(item, ct);
        await _unitOfWork.SaveChangesAsync(ct);
        return Ok(ToDto(item));
    }

    /// <summary>Cập nhật bản dịch (admin).</summary>
    [Authorize(Policy = "AdminOnly")]
    [HttpPut("{id}")]
    public async Task<ActionResult<TranslationDto>> Update(string id, [FromBody] TranslationRequest request, CancellationToken ct)
    {
        var item = await _writer.GetByIdAsync(id, ct)
            ?? throw new NotFoundException(nameof(Translation), id);

        item.Key = request.Key.Trim();
        item.Language = request.Language.Trim();
        item.Value = request.Value;
        item.LastModifiedDate = DateTime.UtcNow;
        item.LastModifiedBy = "admin";

        _writer.Update(item);
        await _unitOfWork.SaveChangesAsync(ct);
        return Ok(ToDto(item));
    }

    /// <summary>Xóa bản dịch (admin).</summary>
    [Authorize(Policy = "AdminOnly")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id, CancellationToken ct)
    {
        var item = await _writer.GetByIdAsync(id, ct)
            ?? throw new NotFoundException(nameof(Translation), id);

        _writer.Remove(item);
        await _unitOfWork.SaveChangesAsync(ct);
        return NoContent();
    }

    private static TranslationDto ToDto(Translation t) => new()
    {
        Id = t.Id,
        Key = t.Key,
        Language = t.Language,
        Value = t.Value,
    };
}
