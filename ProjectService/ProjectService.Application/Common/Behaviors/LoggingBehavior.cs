using System.Diagnostics;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ProjectService.Application.Common.Behaviors;

/// <summary>
/// Pipeline behavior ghi log thời gian xử lý mỗi Command/Query — áp dụng cho mọi request.
/// </summary>
public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;

    public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;
        var stopwatch = Stopwatch.StartNew();

        _logger.LogInformation("Handling {RequestName}...", requestName);

        var response = await next();

        stopwatch.Stop();
        _logger.LogInformation(
            "Handled {RequestName} in {Elapsed}ms",
            requestName,
            stopwatch.ElapsedMilliseconds);

        return response;
    }
}
