using System.Diagnostics;

namespace DishesAPI.EndpointFilters
{
    public class PerformanceTrakingFilter : IEndpointFilter
    {
        private readonly ILogger<PerformanceTrakingFilter> _logger;

        public PerformanceTrakingFilter(ILogger<PerformanceTrakingFilter> logger)
        {
            _logger = logger;
        }

        public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
        {
            var stopwatch = Stopwatch.StartNew();
            var result = await next(context);
            stopwatch.Stop();
            _logger.LogInformation("EndPoint {Method} {Path} completed in {ElapsedMs}ms",
                context.HttpContext.Request.Method,
                context.HttpContext.Request.Path,
                stopwatch.ElapsedMilliseconds
            );
            return result;
        }
    }
}
