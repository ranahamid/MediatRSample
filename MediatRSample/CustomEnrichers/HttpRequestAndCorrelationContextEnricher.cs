using Serilog.Core;
using Serilog.Events;

namespace MediatRSample.CustomEnrichers
{
    public class HttpRequestAndCorrelationContextEnricher : ILogEventEnricher
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        public HttpRequestAndCorrelationContextEnricher(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }
        public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
        {
            var httpContext = _httpContextAccessor.HttpContext;

            if (httpContext != null)
            {
                logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("RequestMethod", httpContext.Request.Method));
                logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("RequestPath", httpContext.Request.Path));
                logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("UserAgent", httpContext.Request.Headers["User-Agent"]));

                if (httpContext.Request.Headers.TryGetValue("App-Correlation-Id", out var appCorrelationId))
                {
                    logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("CorrelationId", appCorrelationId));
                }
            }
        }
    }
}
