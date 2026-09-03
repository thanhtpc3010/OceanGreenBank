namespace ProjectService.Application.Services.DTOs;

/// <summary>Tài khoản tiếp nhận của một quỹ quyên góp (tại một ngân hàng).</summary>
public sealed record DonationBankAccountDto(
    string BankCode,
    string BankName,
    string AccountNumber,
    string Currency);

/// <summary>Quỹ quyên góp / ủng hộ hiển thị cho người dùng lựa chọn.</summary>
public sealed record DonationFundDto(
    string Id,
    string Name,
    string Organization,
    string Description,
    IReadOnlyList<DonationBankAccountDto> Accounts);
