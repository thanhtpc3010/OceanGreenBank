using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectService.Application.Common.Interfaces;
using ProjectService.Domain.Entity;
using ProjectService.Domain.Exceptions;

namespace ProjectService.Api.Controllers;

/// <summary>DTO tạo/cập nhật một mục kiến thức.</summary>
public class KnowledgeEntryRequest
{
    public string Keywords { get; set; } = "";
    public string Title { get; set; } = "";
    public string Content { get; set; } = "";
    public bool IsActive { get; set; } = true;
}

/// <summary>DTO trả về một mục kiến thức.</summary>
public class KnowledgeEntryDto
{
    public string Id { get; set; } = "";
    public string Keywords { get; set; } = "";
    public string Title { get; set; } = "";
    public string Content { get; set; } = "";
    public bool IsActive { get; set; }
    public DateTime CreatedDate { get; set; }
}

/// <summary>
/// Quản lý kho kiến thức cho chat bot (RAG-lite) — chỉ admin.
///   - GET    /api/knowledge          → danh sách
///   - POST   /api/knowledge          → thêm mới
///   - PUT    /api/knowledge/{id}     → cập nhật
///   - DELETE /api/knowledge/{id}     → xóa
/// </summary>
[ApiController]
[Authorize(Policy = "AdminOnly")]
[Route("api/knowledge")]
public class KnowledgeController : ControllerBase
{
    private readonly IReadRepository<KnowledgeEntry> _reader;
    private readonly IWriteRepository<KnowledgeEntry> _writer;
    private readonly IUnitOfWork _unitOfWork;

    public KnowledgeController(
        IReadRepository<KnowledgeEntry> reader,
        IWriteRepository<KnowledgeEntry> writer,
        IUnitOfWork unitOfWork)
    {
        _reader = reader;
        _writer = writer;
        _unitOfWork = unitOfWork;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<KnowledgeEntryDto>>> GetAll(CancellationToken ct)
    {
        var entries = await _reader.ListAsync(ct);
        return Ok(entries
            .OrderByDescending(e => e.CreatedDate)
            .Select(ToDto)
            .ToList());
    }

    [HttpPost]
    public async Task<ActionResult<KnowledgeEntryDto>> Create([FromBody] KnowledgeEntryRequest request, CancellationToken ct)
    {
        Validate(request);

        var entry = new KnowledgeEntry
        {
            Id = Guid.NewGuid().ToString("N"),
            CreatedDate = DateTime.UtcNow,
            CreatedBy = "admin",
            Keywords = request.Keywords.Trim(),
            Title = request.Title.Trim(),
            Content = request.Content.Trim(),
            IsActive = request.IsActive,
        };

        await _writer.AddAsync(entry, ct);
        await _unitOfWork.SaveChangesAsync(ct);
        return Ok(ToDto(entry));
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<KnowledgeEntryDto>> Update(string id, [FromBody] KnowledgeEntryRequest request, CancellationToken ct)
    {
        Validate(request);

        var entry = await _writer.GetByIdAsync(id, ct)
            ?? throw new NotFoundException(nameof(KnowledgeEntry), id);

        entry.Keywords = request.Keywords.Trim();
        entry.Title = request.Title.Trim();
        entry.Content = request.Content.Trim();
        entry.IsActive = request.IsActive;
        entry.LastModifiedDate = DateTime.UtcNow;
        entry.LastModifiedBy = "admin";

        _writer.Update(entry);
        await _unitOfWork.SaveChangesAsync(ct);
        return Ok(ToDto(entry));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id, CancellationToken ct)
    {
        var entry = await _writer.GetByIdAsync(id, ct)
            ?? throw new NotFoundException(nameof(KnowledgeEntry), id);

        _writer.Remove(entry);
        await _unitOfWork.SaveChangesAsync(ct);
        return NoContent();
    }

    private static void Validate(KnowledgeEntryRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Title))
            throw new DomainException("Tiêu đề không được để trống.");
        if (string.IsNullOrWhiteSpace(request.Content))
            throw new DomainException("Nội dung không được để trống.");
        if (string.IsNullOrWhiteSpace(request.Keywords))
            throw new DomainException("Từ khóa không được để trống (phân cách bằng dấu phẩy).");
    }

    private static KnowledgeEntryDto ToDto(KnowledgeEntry e) => new()
    {
        Id = e.Id,
        Keywords = e.Keywords,
        Title = e.Title,
        Content = e.Content,
        IsActive = e.IsActive,
        CreatedDate = e.CreatedDate,
    };
}
