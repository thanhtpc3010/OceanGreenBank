using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProjectService.Application.Common.Interfaces;
using ProjectService.Infrastructure.Persistence;
using ProjectService.Infrastructure.Persistence.Contexts;
using ProjectService.Infrastructure.Persistence.Repositories;
using ProjectService.Infrastructure.Services;

namespace ProjectService.Infrastructure;

public static class DependencyInjection
{
    /// <summary>
    /// Đăng ký Read/Write DbContext, Repository, UnitOfWork và các dịch vụ Infrastructure.
    /// </summary>
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? "Host=localhost;Database=OceanGreenBank;Username=postgres;Password=postgres";

        // Read side (Query) — NoTracking
        services.AddDbContext<ApplicationReadDbContext>(options =>
            options.UseNpgsql(connectionString));

        // Write side (Command) — tracking
        services.AddDbContext<ApplicationWriteDbContext>(options =>
            options.UseNpgsql(connectionString));

        services.AddScoped(typeof(IReadRepository<>), typeof(ReadRepository<>));
        services.AddScoped(typeof(IWriteRepository<>), typeof(WriteRepository<>));
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IDateTime, DateTimeService>();

        return services;
    }
}
