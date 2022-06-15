using Microsoft.AspNetCore.Http;
using Serilog;
using Serilog.Context;

namespace DN.WebApi.Infrastructure.Middleware;

public class ResponseLoggingMiddleware : IMiddleware
{

	public ResponseLoggingMiddleware()
	{
	}

	public async Task InvokeAsync(HttpContext httpContext, RequestDelegate next)
	{
		await next(httpContext);
		var originalBody = httpContext.Response.Body;
		using var newBody = new MemoryStream();
		httpContext.Response.Body = newBody;
		string responseBody;
		if (httpContext.Request.Path.ToString().Contains("tokens"))
		{
			responseBody = "[Redacted] Contains Sensitive Information.";
		}
		else
		{
			newBody.Seek(0, SeekOrigin.Begin);
			responseBody = await new StreamReader(httpContext.Response.Body).ReadToEndAsync();
		}

		LogContext.PushProperty("ResponseTimeUTC", DateTime.UtcNow);
		Log.ForContext("ResponseHeaders", httpContext.Response.Headers.ToDictionary(h => h.Key, h => h.Value.ToString()), destructureObjects: true)
	   .ForContext("ResponseBody", responseBody)
	   .Information($"HTTP {httpContext.Request.Method} Request to {httpContext.Request.Path} has Status Code {httpContext.Response.StatusCode}.");
		newBody.Seek(0, SeekOrigin.Begin);
		await newBody.CopyToAsync(originalBody);
	}
}