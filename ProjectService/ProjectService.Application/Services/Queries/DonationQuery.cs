using MediatR;
using ProjectService.Application.Common.Base;
using ProjectService.Application.Common.Interfaces;
using ProjectService.Application.Services.DTOs;

namespace ProjectService.Application.Services.Queries;

// ============================ REQUEST ============================
public sealed record GetDonationFundsQuery() : BaseQuery<IReadOnlyList<DonationFundDto>>;

// ============================ SERVICE INTERFACE ============================
/// <summary>Interface của Donation Query Service.</summary>
public interface IDonationQueryService : IQueryService<GetDonationFundsQuery, IReadOnlyList<DonationFundDto>> { }

// ============================ HANDLER (READ SIDE) ============================
/// <summary>Trả về danh sách các quỹ quyên góp / ủng hộ (không cần truy vấn DB).</summary>
public class DonationQuery : IDonationQueryService,
    IRequestHandler<GetDonationFundsQuery, IReadOnlyList<DonationFundDto>>
{
    // --- MediatR dispatch ---
    public Task<IReadOnlyList<DonationFundDto>> Handle(GetDonationFundsQuery request, CancellationToken ct)
        => GetAsync(request, ct);

    // --- Operations ---
    public Task<IReadOnlyList<DonationFundDto>> GetAsync(GetDonationFundsQuery request, CancellationToken ct)
        => Task.FromResult(DonationFundCatalog.ToDtos());
}
