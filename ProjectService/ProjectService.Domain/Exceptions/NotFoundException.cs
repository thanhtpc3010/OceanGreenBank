namespace ProjectService.Domain.Exceptions;

/// <summary>
/// Mục đích: Ném ra khi một thực thể cụ thể không được tìm thấy trong cơ sở dữ liệu.
/// Exception này sẽ được bắt và chuyển thành HTTP 404 Not Found.
/// </summary>
public class NotFoundException : Exception
{
    public NotFoundException(string name, object key) : base($"Entity \"{name}\" ({key}) was not found.")
    { }
}
