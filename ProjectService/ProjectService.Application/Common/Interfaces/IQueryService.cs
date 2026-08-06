namespace ProjectService.Application.Common.Interfaces;

/// <summary>
/// Base interface cho mọi Query Service — method chung: GetAsync.
/// Query Service của từng domain kế thừa để không phải khai báo lặp lại.
/// </summary>
public interface IQueryService<TGetRequest, TDto>
{
    Task<TDto> GetAsync(TGetRequest request, CancellationToken cancellationToken = default);
}
