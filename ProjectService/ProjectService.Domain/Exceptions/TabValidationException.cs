namespace ProjectService.Domain.Exceptions;

/// <summary>
/// Ném ra khi validation theo tab (màn hình) thất bại, kèm chi tiết lỗi từng tab.
/// </summary>
public class TabValidationException : Exception
{
    public TabValidationException(List<TabValidationResult> tabErrors)
        : base("Tab validation failed")
    {
        TabErrors = tabErrors;
    }

    public List<TabValidationResult> TabErrors { get; }
}

public class TabValidationResult
{
    public string TabName { get; set; } = string.Empty;

    public Dictionary<string, Dictionary<string, List<string>>> ItemErrors { get; set; } = new();

    public List<TabValidationError> ErrorMessages { get; set; } = new();
}

public class TabValidationError
{
    public int RowNumber { get; set; }

    public string FieldName { get; set; } = string.Empty;

    public string ErrorKey { get; set; } = string.Empty;

    public string? PropertyValue { get; set; }
}
