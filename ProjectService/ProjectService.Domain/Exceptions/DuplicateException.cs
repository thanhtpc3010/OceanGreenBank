namespace ProjectService.Domain.Exceptions;

/// <summary>
/// Ném ra khi phát hiện dữ liệu trùng lặp (VD: Email, SĐT, Mã tài khoản đã tồn tại).
/// Thường được chuyển thành HTTP 409 Conflict.
/// </summary>
public class DuplicateException : Exception
{
    public DuplicateException(string message) : base(message)
    { }
}
