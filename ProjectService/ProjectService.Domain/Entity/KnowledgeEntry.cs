using ProjectService.Domain.Common;

namespace ProjectService.Domain.Entity;

/// <summary>
/// Một mục kiến thức (knowledge base) cho chat bot — "train nhẹ" theo kiểu RAG.
/// Bot sẽ tìm các mục có từ khóa khớp với câu hỏi của người dùng rồi
/// chèn nội dung vào system prompt để trả lời chính xác theo tài liệu.
/// </summary>
public class KnowledgeEntry : BaseEntity
{
    /// <summary>Danh sách từ khóa, cách nhau bằng dấu phẩy (VD: "chuyển tiền, transfer, chuyển khoản").</summary>
    public string Keywords { get; set; } = "";

    /// <summary>Chủ đề / câu hỏi ngắn gọn.</summary>
    public string Title { get; set; } = "";

    /// <summary>Nội dung hướng dẫn chi tiết (markdown đơn giản).</summary>
    public string Content { get; set; } = "";

    /// <summary>Còn sử dụng hay không (khóa mềm).</summary>
    public bool IsActive { get; set; } = true;
}
