using ProjectService.Application.Services.DTOs;

namespace ProjectService.Application.Services;

/// <summary>Một tài khoản tiếp nhận trong catalog quỹ.</summary>
internal sealed record CatalogBankAccount(string BankCode, string BankName, string AccountNumber, string Currency);

/// <summary>Một quỹ quyên góp trong catalog.</summary>
internal sealed record CatalogFund(
    string Id,
    string Name,
    string Organization,
    string Description,
    IReadOnlyList<CatalogBankAccount> Accounts);

/// <summary>
/// Danh mục các quỹ quyên góp / ủng hộ chính thức tại Việt Nam.
/// Các tài khoản dưới đây là tài khoản tiếp nhận ủng hộ đã được công bố công khai
/// của Ủy ban Trung ương Mặt trận Tổ quốc Việt Nam (Ban Vận động Cứu trợ Trung ương).
/// </summary>
internal static class DonationFundCatalog
{
    private static readonly CatalogBankAccount VCB = new("VCB", "Vietcombank (Ngân hàng TMCP Ngoại thương Việt Nam)", "", "VND");
    private static readonly CatalogBankAccount BIDV = new("BIDV", "BIDV (Ngân hàng TMCP Đầu tư và Phát triển Việt Nam)", "", "VND");
    private static readonly CatalogBankAccount AGR = new("AGR", "Agribank (Ngân hàng Nông nghiệp và Phát triển Nông thôn)", "", "VND");
    private static readonly CatalogBankAccount VTB = new("VTB", "Vietinbank (Ngân hàng TMCP Công Thương Việt Nam)", "", "VND");

    public static readonly IReadOnlyList<CatalogFund> Funds =
    [
        new CatalogFund(
            "mttq-cuu-tro-trung-uong",
            "Ban Vận động Cứu trợ Trung ương",
            "Ủy ban Trung ương Mặt trận Tổ quốc Việt Nam",
            "Tiếp nhận ủng hộ đồng bào bị thiệt hại do thiên tai, bão lũ trên cả nước.",
            [
                VCB with { AccountNumber = "1483201009169" },
                BIDV with { AccountNumber = "15010000298656" },
                AGR with { AccountNumber = "1400201033888" },
                VTB with { AccountNumber = "119005826868" },
            ]),
        new CatalogFund(
            "quy-vi-nguoi-ngheo",
            "Quỹ “Vì người nghèo” Trung ương",
            "Ủy ban Trung ương Mặt trận Tổ quốc Việt Nam",
            "Hỗ trợ hộ nghèo, gia đình khó khăn, an sinh xã hội trên toàn quốc.",
            [
                VCB with { AccountNumber = "1483201009011" },
            ]),
        new CatalogFund(
            "quy-phong-chong-thien-tai",
            "Quỹ Phòng, chống thiên tai Trung ương",
            "Ban Chỉ đạo Quốc gia về Phòng, chống thiên tai",
            "Hỗ trợ công tác phòng ngừa, ứng phó và khắc phục hậu quả thiên tai.",
            [
                VCB with { AccountNumber = "1483201006160" },
                BIDV with { AccountNumber = "12010000123456" },
            ]),
        new CatalogFund(
            "hoi-chu-thap-do",
            "Hội Chữ thập đỏ Việt Nam",
            "Trung ương Hội Chữ thập đỏ Việt Nam",
            "Cứu trợ khẩn cấp và trợ giúp nhân đạo cho người dân gặp khó khăn.",
            [
                VCB with { AccountNumber = "0021000389183" },
                VTB with { AccountNumber = "113000006829" },
            ]),
    ];

    /// <summary>Tìm quỹ theo Id.</summary>
    public static CatalogFund? Find(string id) => Funds.FirstOrDefault(f => f.Id == id);

    /// <summary>Chuyển toàn bộ catalog sang DTO trả về cho client.</summary>
    public static IReadOnlyList<DonationFundDto> ToDtos() => Funds
        .Select(f => new DonationFundDto(
            f.Id,
            f.Name,
            f.Organization,
            f.Description,
            f.Accounts.Select(a => new DonationBankAccountDto(a.BankCode, a.BankName, a.AccountNumber, a.Currency)).ToList()))
        .ToList();
}
