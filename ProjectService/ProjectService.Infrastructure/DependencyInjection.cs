using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProjectService.Application.Common.Interfaces;
using ProjectService.Infrastructure.Persistence;
using ProjectService.Infrastructure.Persistence.Repositories;
using ProjectService.Infrastructure.Services;

namespace ProjectService.Infrastructure;

public static class DependencyInjection
{
    /// <summary>
    /// Đăng ký EF Core, Repository, UnitOfWork và các dịch vụ Infrastructure.
    /// </summary>
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? "Server=(localdb)\\mssqllocaldb;Database=OceanGreenBank;Trusted_Connection=True;MultipleActiveResultSets=true";

        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(connectionString));

        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IDateTime, DateTimeService>();

        return services;
    }
}
