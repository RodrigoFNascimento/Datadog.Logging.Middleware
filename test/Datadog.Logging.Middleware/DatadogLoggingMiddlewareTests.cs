using Bogus;
using FluentAssertions;
using HttpContextMoq;
using HttpContextMoq.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using NSubstitute;
using System.Net;

namespace Datadog.Logging.Middleware.Tests.Unit;

public class DatadogLoggingMiddlewareTests
{
    private readonly Faker _faker;
    private readonly DatadogLoggingMiddleware _sut;
    private readonly RequestDelegate _next;
    private readonly ILogger<DatadogLoggingMiddleware> _logger;

    public DatadogLoggingMiddlewareTests()
    {
        _faker = new();
        _next = Substitute.For<RequestDelegate>();
        _logger = Substitute.For<ILogger<DatadogLoggingMiddleware>>();
        _sut = new(_next, _logger);
    }

    [Fact]
    public async Task Invoke_ShouldAddAttributesToLogContext()
    {
        // Arrange
        var ip = _faker.Internet.Ip();
        var referer = _faker.Internet.Url();
        var userAgent = _faker.Internet.UserAgent();
        var url = _faker.Internet.Url();
        var httpMethod = HttpMethod.Get.ToString();
        var statusCode = HttpStatusCode.OK;

        Dictionary<string, StringValues> headers = new()
        {
            { "X-Forwarded-For", ip },
            { "Referer", referer },
            { "User-Agent", userAgent }
        };

        var context = new HttpContextMock()
            .SetupUrl(url)
            .SetupRequestMethod(httpMethod)
            .SetupRequestHeaders(headers)
            .SetupResponseStatusCode(statusCode);

        Dictionary<string, object?> attributes = [];

        _logger.BeginScope(Arg.Do<Dictionary<string, object?>>(x => attributes = x));

        // Act
        await _sut.Invoke(context);

        // Assert
        ((string?)attributes["network.client.ip"]).Should().Be(ip);
        (attributes["network.destination.port"]).Should().BeNull();
        ((string?)attributes["http.url"]).Should().Be($"{url}/");
        (attributes["http.status_code"]).Should().Be((int)statusCode);
        ((string?)attributes["http.method"]).Should().Be(httpMethod);
        ((string?)attributes["http.referer"]).Should().Be(referer);
        ((string?)attributes["http.useragent"]).Should().Be(userAgent);
        ((string?)attributes["http.version"]).Should().Be("HTTP/1.1");

        await _next.Received(1).Invoke(context);
    }
}