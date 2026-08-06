namespace ProjectService.Application.Common.Interfaces;

/// <summary>
/// Base interface cho mọi Command Service — 3 method chung: CreateAsync, UpdateAsync, DeleteAsync.
/// Command Service của từng domain kế thừa để không phải khai báo lặp lại.
/// </summary>
public interface ICommandService<TCreateRequest, TUpdateRequest, TDeleteRequest, TDto>
{
    Task<TDto> CreateAsync(TCreateRequest request, CancellationToken cancellationToken = default);

    Task<TDto> UpdateAsync(TUpdateRequest request, CancellationToken cancellationToken = default);

    Task DeleteAsync(TDeleteRequest request, CancellationToken cancellationToken = default);
}
