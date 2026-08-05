namespace ProjectService.Domain.Exceptions;

/// <summary>
/// Ngoại lệ dùng chung cho các lỗi nghiệp vụ trong Domain Layer.
/// </summary>
public class DomainException : Exception
{
    public DomainException(string message) : base(message) { }

    public DomainException(string message, Exception innerException)
        : base(message, innerException) { }
}
