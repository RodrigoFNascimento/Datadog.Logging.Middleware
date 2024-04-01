using Microsoft.AspNetCore.Builder;

namespace Datadog.Logging.Middleware
{
    public static class IApplicationBuilderExtensions
    {
        /// <summary>
        /// Adds a middleware that adds Datadog's default standard attributes to the logs.
        /// </summary>
        /// <param name="builder"></param>
        /// <returns>The application builder.</returns>
        public static IApplicationBuilder UseDatadogLogging(
            this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<DatadogLoggingMiddleware>();
        }
    }
}
