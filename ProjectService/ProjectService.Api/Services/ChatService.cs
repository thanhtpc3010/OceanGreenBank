using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Options;
using ProjectService.Application.Common.Interfaces;
using ProjectService.Domain.Entity;
using ProjectService.Domain.Exceptions;

namespace ProjectService.Api.Services;

/// <summary>Cấu hình Gemini từ appsettings.json.</summary>
public class GeminiSettings
{
    public string ApiKey { get; set; } = "";
    public string Model { get; set; } = "gemini-2.5-flash";
}

/// <summary>Một tin nhắn trong hội thoại.</summary>
public class ChatMessageDto
{
    public string Role { get; set; } = "user"; // "user" | "model"
    public string Content { get; set; } = "";
}

/// <summary>Request trò chuyện từ frontend.</summary>
public class ChatRequest
{
    public string Message { get; set; } = "";
    public List<ChatMessageDto>? History { get; set; }
}

/// <summary>Phản hồi trò chuyện trả về frontend.</summary>
public class ChatResponse
{
    public string Reply { get; set; } = "";
    public bool Enabled { get; set; } = true;
}

/// <summary>
/// Service trò chuyện với Gemini (Google AI) qua backend proxy:
///   - Giữ API key ở backend (không lộ xuống frontend).
///   - Tự động nạp ngữ cảnh của người dùng đang đăng nhập (hồ sơ, tài khoản,
///     số dư, giao dịch gần đây, AutoEarn) để bot trả lời chính xác.
///   - System prompt mô tả các chức năng SmartBank.
/// </summary>
public class ChatService
{
    private readonly IOptionsMonitor<GeminiSettings> _gemini;
    private readonly IReadRepository<User> _users;
    private readonly IReadRepository<Account> _accounts;
    private readonly IReadRepository<Transaction> _transactions;
    private readonly IReadRepository<AutoEarnSetting> _autoEarnSettings;
    private readonly IReadRepository<AutoEarnLog> _autoEarnLogs;
    private readonly IReadRepository<KnowledgeEntry> _knowledge;
    private readonly ILogger<ChatService> _logger;
    private static readonly HttpClient _http = new() { Timeout = TimeSpan.FromSeconds(60) };

    public ChatService(
        IOptionsMonitor<GeminiSettings> gemini,
        IReadRepository<User> users,
        IReadRepository<Account> accounts,
        IReadRepository<Transaction> transactions,
        IReadRepository<AutoEarnSetting> autoEarnSettings,
        IReadRepository<AutoEarnLog> autoEarnLogs,
        IReadRepository<KnowledgeEntry> knowledge,
        ILogger<ChatService> logger)
    {
        _gemini = gemini;
        _users = users;
        _accounts = accounts;
        _transactions = transactions;
        _autoEarnSettings = autoEarnSettings;
        _autoEarnLogs = autoEarnLogs;
        _knowledge = knowledge;
        _logger = logger;
    }

    /// <summary>Bot có sẵn sàng (đã có API key) hay chưa.</summary>
    public bool IsEnabled => !string.IsNullOrWhiteSpace(_gemini.CurrentValue.ApiKey);

    /// <summary>Gửi tin nhắn + lịch sử → Gemini → trả lời.</summary>
    public async Task<ChatResponse> SendAsync(ChatRequest request, string userId, CancellationToken ct)
    {
        if (!IsEnabled)
            throw new DomainException(
                "Chat AI chưa được cấu hình API key. Quản trị viên cần điền Gemini:ApiKey trong appsettings.json.");

        var context = await LoadUserContextAsync(userId, ct);
        var knowledge = await LoadRelevantKnowledgeAsync(request.Message, ct);
        var systemPrompt = BuildSystemPrompt(context, knowledge);

        // Lịch sử (tối đa 10 tin) + tin nhắn hiện tại.
        var contents = new List<object>();
        foreach (var m in (request.History ?? new List<ChatMessageDto>()).TakeLast(10))
        {
            if (string.IsNullOrWhiteSpace(m.Content)) continue;
            contents.Add(new { role = m.Role == "model" ? "model" : "user", parts = new[] { new { text = m.Content } } });
        }
        contents.Add(new { role = "user", parts = new[] { new { text = request.Message } } });

        var payload = new
        {
            system_instruction = new { parts = new[] { new { text = systemPrompt } } },
            contents,
            generationConfig = new { temperature = 0.6, maxOutputTokens = 900, topP = 0.95 }
        };

        var url = $"https://generativelanguage.googleapis.com/v1beta/models/{_gemini.CurrentValue.Model}:generateContent?key={_gemini.CurrentValue.ApiKey}";
        using var body = new StringContent(JsonSerializer.Serialize(payload), System.Text.Encoding.UTF8, "application/json");

        try
        {
            using var resp = await _http.PostAsync(url, body, ct);
            var json = await resp.Content.ReadAsStringAsync(ct);

            if (!resp.IsSuccessStatusCode)
            {
                _logger.LogWarning("Gemini error {Status}: {Body}", (int)resp.StatusCode, Truncate(json, 400));
                throw new DomainException($"Gemini API lỗi ({(int)resp.StatusCode}). Vui lòng thử lại sau.");
            }

            var reply = ParseReply(json);
            if (string.IsNullOrWhiteSpace(reply))
                throw new DomainException("Gemini không trả về nội dung. Vui lòng thử lại.");

            return new ChatResponse { Reply = reply, Enabled = true };
        }
        catch (TaskCanceledException)
        {
            throw new DomainException("Hết thời gian chờ phản hồi từ AI. Vui lòng thử lại.");
        }
    }

    /// <summary>Nạp toàn bộ ngữ cảnh cá nhân của user đang đăng nhập.</summary>
    private async Task<UserChatContext> LoadUserContextAsync(string userId, CancellationToken ct)
    {
        var user = (await _users.FindAsync(u => u.Id == userId, ct)).FirstOrDefault();
        var accounts = (await _accounts.FindAsync(a => a.UserId == userId, ct)).ToList();
        var accountIds = accounts.Select(a => a.Id).ToHashSet();

        var recentTx = accountIds.Count == 0
            ? new List<Transaction>()
            : (await _transactions.FindAsync(
                    t => accountIds.Contains(t.FromAccountId)
                         || (t.ToAccountId != null && accountIds.Contains(t.ToAccountId)), ct))
                .OrderByDescending(t => t.CreatedDate)
                .Take(10)
                .ToList();

        var aeSetting = (await _autoEarnSettings.ListAsync(ct)).FirstOrDefault();
        var aeLogs = accountIds.Count == 0
            ? new List<AutoEarnLog>()
            : (await _autoEarnLogs.FindAsync(l => accountIds.Contains(l.AccountId), ct)).ToList();

        return new UserChatContext { User = user, Accounts = accounts, RecentTransactions = recentTx, AutoEarnSetting = aeSetting, AutoEarnLogs = aeLogs };
    }

    /// <summary>Xây system prompt: mô tả chức năng app + kiến thức liên quan + dữ liệu cá nhân của user.</summary>
    private static string BuildSystemPrompt(UserChatContext c, IReadOnlyList<KnowledgeEntry> knowledge)
    {
        var sb = new System.Text.StringBuilder();
        sb.AppendLine("Bạn là trợ lý AI của SmartBank (OceanGreenBank) - ngân hàng số Việt Nam. Nhiệm vụ: hướng dẫn người dùng dùng các chức năng của app và trả lời về tài khoản của họ. LUÔN trả lời bằng tiếng Việt, ngắn gọn, dễ hiểu, thân thiện. Nếu không biết, hãy nói thật và gợi ý liên hệ tổng đài 1900 0000.");

        sb.AppendLine();
        sb.AppendLine("=== CÁC CHỨC NĂNG CỦA APP (để hướng dẫn) ===");
        sb.AppendLine("- Quản lý tài khoản: xem danh sách tài khoản CASA (thanh toán) và tài khoản TIẾT KIỆM (SAVINGS) có kỳ hạn + lãi suất; mở sổ tiết kiệm (kỳ hạn 1/3/6/12 tháng); thêm/xóa tài khoản; đổi thông tin hồ sơ; đổi mật khẩu.");
        sb.AppendLine("- Chuyển tiền: chuyển nội bộ SmartBank (miễn phí) hoặc liên ngân hàng (phí 5.000đ). Tài khoản tiết kiệm chỉ được rút khi ĐÁO HẠN; rút trước hạn (khẩn cấp) sẽ MẤT TOÀN BỘ lãi chu kỳ.");
        sb.AppendLine("- Tiết kiệm định kỳ: kế hoạch gửi tự động hằng ngày/tuần/tháng từ tài khoản nguồn sang tài khoản đích; có thể gửi ngay hoặc hủy kế hoạch.");
        sb.AppendLine("- AutoEarn (sinh lời tự động): tài khoản tham gia được cộng lãi mỗi ngày theo tiền gốc (lãi suất %/năm / 365). Admin cấu hình bật/tắt, lãi suất, giờ chạy tự động.");
        sb.AppendLine("- PFM: thống kê thu/chi theo danh mục (ăn uống, mua sắm, hóa đơn...), biểu đồ chi tiêu trên Dashboard.");
        sb.AppendLine("- Quản lý User (admin): xem/tìm/khóa/mở khóa/xóa user, xem vai trò & quyền (RBAC).");

        if (knowledge.Count > 0)
        {
            sb.AppendLine();
            sb.AppendLine("=== KIẾN THỨC LIÊN QUAN (dùng để trả lời chính xác, ưu tiên theo nội dung này) ===");
            foreach (var k in knowledge)
            {
                sb.AppendLine($"## {k.Title}");
                sb.AppendLine(k.Content);
            }
        }

        sb.AppendLine();
        sb.AppendLine("=== DỮ LIỆU CÁ NHÂN CỦA NGƯỜI ĐANG TRÒ CHUYỆN (chỉ dùng để trả lời, KHÔNG bịa số liệu) ===");
        if (c.User != null)
            sb.AppendLine($"- Họ tên: {c.User.FullName}; Email: {c.User.Email}; SĐT: {c.User.Phone}");
        else
            sb.AppendLine("- Không tìm thấy hồ sơ người dùng.");

        if (c.Accounts.Count == 0)
        {
            sb.AppendLine("- Người dùng chưa có tài khoản nào.");
        }
        else
        {
            sb.AppendLine("- Danh sách tài khoản:");
            foreach (var a in c.Accounts)
            {
                var type = a.Type == ProjectService.Domain.Enum.AccountType.Savings ? "Tiết kiệm" : "Thanh toán (CASA)";
                sb.AppendLine($"  • {a.AccountNumber} ({type}) - Số dư: {a.Balance:N0} {a.Currency} - Trạng thái: {(a.IsActive ? "hoạt động" : "đã đóng")}"
                    + (a.Type == ProjectService.Domain.Enum.AccountType.Savings && a.SavingsTermMonths.HasValue
                        ? $" - Kỳ hạn {a.SavingsTermMonths} tháng, lãi suất {a.InterestRate:0.#}%/năm, đáo hạn {(a.SavingsMaturityDate.HasValue ? a.SavingsMaturityDate.Value.ToString("dd/MM/yyyy") : "N/A")}"
                        : "")
                    + (a.IsAutoEarnEnrolled ? $" - Có tham gia AutoEarn, gốc {a.AutoEarnPrincipal:N0} VND" : ""));
            }
        }

        if (c.RecentTransactions.Count > 0)
        {
            sb.AppendLine("- 10 giao dịch gần đây:");
            foreach (var t in c.RecentTransactions)
                sb.AppendLine($"  • {t.CreatedDate:dd/MM/yyyy HH:mm} - {t.TransactionCode} - {t.Description ?? "Chuyển tiền"} - {t.Amount:N0} VND - {(t.Status == ProjectService.Domain.Enum.TransactionStatus.Success ? "thành công" : t.Status.ToString())}");
        }

        if (c.AutoEarnSetting != null)
        {
            var monthly = c.AutoEarnLogs.Where(l => l.RunDate.Month == DateTime.UtcNow.Month).Sum(l => l.InterestAmount);
            sb.AppendLine($"- AutoEarn: {(c.AutoEarnSetting.IsActive ? "đang bật" : "đang tắt")}, lãi suất {c.AutoEarnSetting.AnnualInterestRate:0.#}%/năm, chạy lúc {c.AutoEarnSetting.RunTime} hằng ngày, tích lũy tháng này {monthly:N0} VND.");
        }

        sb.AppendLine();
        sb.AppendLine("Hướng dẫn: nếu câu hỏi về cách dùng → hướng dẫn từng bước ngắn gọn, ưu tiên dùng phần KIẾN THỨC LIÊN QUAN. Nếu về tài khoản/số dư → dùng dữ liệu trên, tuyệt đối không bịa. Không yêu cầu mật khẩu/OTP. Định dạng rõ ràng, dùng gạch đầu dòng khi cần.");

        return sb.ToString();
    }

    /// <summary>Tải các mục kiến thức khớp với câu hỏi (theo từ khóa, không phân biệt dấu/hoa thường).</summary>
    private async Task<IReadOnlyList<KnowledgeEntry>> LoadRelevantKnowledgeAsync(string message, CancellationToken ct)
    {
        var entries = await _knowledge.FindAsync(k => k.IsActive, ct);
        if (entries.Count == 0) return new List<KnowledgeEntry>();

        var q = Normalize(message);
        var matched = entries
            .Select(e => new { Entry = e, Score = Score(e, q) })
            .Where(x => x.Score > 0)
            .OrderByDescending(x => x.Score)
            .Take(4)
            .Select(x => x.Entry)
            .ToList();
        return matched;
    }

    private static int Score(KnowledgeEntry e, string q)
    {
        var score = 0;
        foreach (var kw in e.Keywords.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
        {
            var nk = Normalize(kw);
            if (nk.Length == 0) continue;
            if (q.Contains(nk, StringComparison.Ordinal))
            {
                // Từ khóa dài hơn → khớp chính xác hơn.
                score += nk.Length;
            }
            else if (nk.Contains(q, StringComparison.Ordinal) && q.Length >= 4)
            {
                score += Math.Max(1, q.Length / 2);
            }
        }
        return score;
    }

    /// <summary>Chuẩn hóa: lowercase + bỏ dấu tiếng Việt để khớp từ khóa linh hoạt.</summary>
    private static string Normalize(string s)
    {
        if (string.IsNullOrEmpty(s)) return "";
        var temp = s.Normalize(System.Text.NormalizationForm.FormD);
        var sb = new System.Text.StringBuilder();
        foreach (var ch in temp)
        {
            if (System.Globalization.CharUnicodeInfo.GetUnicodeCategory(ch) != System.Globalization.UnicodeCategory.NonSpacingMark)
                sb.Append(ch);
        }
        return sb.ToString().Normalize(System.Text.NormalizationForm.FormC).ToLowerInvariant();
    }

    /// <summary>Parse nội dung trả lời từ JSON phản hồi Gemini.</summary>
    private static string ParseReply(string json)
    {
        using var doc = JsonDocument.Parse(json);
        if (!doc.RootElement.TryGetProperty("candidates", out var candidates)) return "";

        foreach (var cand in candidates.EnumerateArray())
        {
            if (!cand.TryGetProperty("content", out var content)) continue;
            if (!content.TryGetProperty("parts", out var parts)) continue;
            var text = string.Concat(parts.EnumerateArray().Select(p =>
                p.TryGetProperty("text", out var t) ? t.GetString() : ""));
            if (!string.IsNullOrWhiteSpace(text)) return text.Trim();
        }
        return "";
    }

    private static string Truncate(string s, int max)
        => s.Length <= max ? s : s[..max];
}

/// <summary>Ngữ cảnh cá nhân của user để đưa vào system prompt.</summary>
public class UserChatContext
{
    public User? User { get; set; }
    public List<Account> Accounts { get; set; } = new();
    public List<Transaction> RecentTransactions { get; set; } = new();
    public AutoEarnSetting? AutoEarnSetting { get; set; }
    public List<AutoEarnLog> AutoEarnLogs { get; set; } = new();
}
