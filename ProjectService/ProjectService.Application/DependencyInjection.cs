using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using ProjectService.Application.Common.Behaviors;

namespace ProjectService.Application;

public static class DependencyInjection
{
    /// <summary>
    /// Đăng ký MediatR và các dependency của Application Layer.
    /// </summary>
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
            cfg.AddOpenBehavior(typeof(LoggingBehavior<,>));
        });

        return services;
    }
}
