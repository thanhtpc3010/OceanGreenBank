namespace ProjectService.Domain.Exceptions;

/// <summary>
/// Ném ra khi giao dịch (DB transaction) thất bại — thường bọc inner exception để giữ nguyên message gốc.
/// </summary>
public class TransactionException : Exception
{
    public TransactionException(string message, Exception innerException)
        : base(FormatMessage(message, innerException), innerException)
    {
    }

    private static string FormatMessage(string message, Exception innerException)
    {
        if (innerException.InnerException != null)
            return $"{message} | {innerException.InnerException.Message}";

        return $"{message} | {innerException.Message}";
    }
}
