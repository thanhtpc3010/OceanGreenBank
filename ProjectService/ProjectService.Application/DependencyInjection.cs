using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace ProjectService.Application;

public static class DependencyInjection
{
    /// <summary>
    /// Đăng ký MediatR và các dependency của Application Layer.
    /// </summary>
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

        return services;
    }
}
