using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Datadog.Logging.Middleware
{
    internal class DatadogLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<DatadogLoggingMiddleware> _logger;

        public DatadogLoggingMiddleware(RequestDelegate next, ILogger<DatadogLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            var datadogAttributes = new Dictionary<string, object?>()
            {
                { "network.client.ip", context.Request.Headers["X-Forwarded-For"].FirstOrDefault() },
                { "network.destination.port", context.Request.Host.Port },
                { "http.url", $"{context.Request.Scheme}://{context.Request.Host.Value}{context.Request.Path.Value}" },
                { "http.status_code", context.Response.StatusCode },
                { "http.method", context.Request.Method },
                { "http.referer", context.Request.Headers["Referer"].FirstOrDefault() },
                { "http.useragent", context.Request.Headers["User-Agent"].FirstOrDefault() },
                { "http.version", context.Request.Protocol }
            };

            using (_logger.BeginScope(datadogAttributes))
            {
                await _next.Invoke(context);
            }
        }
    }
}
