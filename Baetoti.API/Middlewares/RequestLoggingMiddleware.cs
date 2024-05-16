using Baetoti.Core.Entites;
using Baetoti.Core.Interface.Base;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Baetoti.API.Middlewares
{
    public class RequestLoggingMiddleware
    {

        private readonly RequestDelegate _next;
        private readonly ILogger<RequestLoggingMiddleware> _logger;
        private readonly IAppLogger<JwtMiddleware> _applogger;

        public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger, IAppLogger<JwtMiddleware> applogger)
        {
            _next = next ?? throw new ArgumentNullException(nameof(next));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _applogger = applogger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            AppException appException = new AppException();
            appException.RequestType = context.Request.Method;
            appException.Url = context.Request.Path;

            _logger.LogInformation($"Request {context.Request.Method} {context.Request.Path} received");

            foreach (var (key, value) in context.Request.Headers)
            {
                _logger.LogInformation($"{key}: {value}");
            }

            context.Request.EnableBuffering();

            string requestBody = string.Empty;
            if (context.Request.Method.Equals("GET", StringComparison.OrdinalIgnoreCase))
            {
                requestBody = context.Request.QueryString.ToString();
            }
            else
            {
                requestBody = await new StreamReader(context.Request.Body).ReadToEndAsync();
            }

            _logger.LogInformation($"Request Body: {requestBody}");
            appException.RequestBody = requestBody;
            _logger.LogInformation($"Request Body: {requestBody}");

            context.Request.Body.Position = 0;
            var originalBodyStream = context.Response.Body;
            using (var responseBodyStream = new MemoryStream())
            {
                context.Response.Body = responseBodyStream;

                await _next(context);

                _logger.LogInformation($"Response status code: {context.Response.StatusCode}");

                responseBodyStream.Position = 0;
                var responseBody = await new StreamReader(responseBodyStream).ReadToEndAsync();
                appException.Message = responseBody;
                appException.UserId = context.User.Identity.Name;
                _logger.LogInformation($"Response Body: {responseBody}");

                // Copy the response body back to the original stream
                responseBodyStream.Position = 0;
                var requestpath = context.Request.Path.Value;
                if (requestpath.Contains("/api"))
                {
                    await _applogger.AllApiLogAsync(appException);
                }
                await responseBodyStream.CopyToAsync(originalBodyStream);

            }
        }

    }

    public static class RequestLoggingMiddlewareExtensions
    {
        public static IApplicationBuilder UseRequestLogging(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<RequestLoggingMiddleware>();
        }

    }

}
