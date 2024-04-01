# Datadog.Logging.Middleware

Datadog.Logging.Middleware adds a middleware to a .NET web application that includes in generated logs Datadog's [default standard attributes](https://docs.datadoghq.com/logs/log_configuration/attributes_naming_convention/).

## Adding the middleware

Inject the middleware using the method `UseDatadogLogging()`.

```csharp
// Program.cs
using Datadog.Logging.Middleware;

var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

app.UseDatadogLogging();  // Add the middleware

app.Run();
```

That's it!

## Added attributes

The following attributes will be added to the log:

| Attribute | Description |
| - | - |
| network.client.ip | The IP address of the client that initiated the inbound connection. |
| network.destination.port | The port of the client that initiated the connection. |
| http.url | The URL of the HTTP request, including the obfuscated query string. For more information on obfuscation, see Configure Data Security. |
| http.status_code | The HTTP response status code. |
| http.method | The port of the client that initiated the connection. |
| http.referer | HTTP header field that identifies the address of the webpage that linked to the resource being requested. |
| http.useragent | The User-Agent header received with the request. |
| http.version | The version of HTTP used for the request. |