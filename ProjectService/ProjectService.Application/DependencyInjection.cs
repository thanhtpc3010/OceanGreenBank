using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using ProjectService.Application.Common.Behaviors;
using ProjectService.Application.Services;
using ProjectService.Application.Services.Commands;
using ProjectService.Application.Services.Queries;

namespace ProjectService.Application;

public static class DependencyInjection
{
    /// <summary>
    /// Đăng ký MediatR, các Command/Query Service và dependency của Application Layer.
    /// </summary>
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
            cfg.AddOpenBehavior(typeof(LoggingBehavior<,>));
        });

        // Command Services (Write side)
        services.AddScoped<IUserCommandService, UserCommand>();
        services.AddScoped<IAccountCommandService, AccountCommand>();
        services.AddScoped<ITransactionCommandService, TransactionCommand>();

        // Query Services (Read side)
        services.AddScoped<IUserQueryService, UserQuery>();
        services.AddScoped<IAccountQueryService, AccountQuery>();
        services.AddScoped<ITransactionQueryService, TransactionQuery>();

        // Dịch vụ AutoEarn (sinh lời tự động)
        services.AddScoped<IAutoEarnService, AutoEarnService>();

        return services;
    }
}
