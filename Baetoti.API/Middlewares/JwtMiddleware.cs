using Baetoti.Core.Interface.Base;
using Baetoti.Core.Interface.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Baetoti.API.Middlewares
{
    public class JwtMiddleware
    {

        private readonly RequestDelegate _next;
        private readonly IEncryptionService _encyptionService;
        private readonly IConfiguration _configuration;
        private readonly IAppLogger<JwtMiddleware> _logger;
        private readonly ILogger<JwtMiddleware> _localLogger;

        public JwtMiddleware(RequestDelegate next,
            IEncryptionService encyptionService,
            IConfiguration configuration,
            IAppLogger<JwtMiddleware> logger,
            ILogger<JwtMiddleware> localLogger)
        {
            _next = next;
            _encyptionService = encyptionService;
            _configuration = configuration;
            _logger = logger;
            _localLogger = localLogger;
        }

        public async Task Invoke(HttpContext context)
        {
            var requestDomain = context?.Request.Headers["Origin"].ToString();
            var origins = _configuration["AllowedOrigins"];
            var allowedOrigins = new List<string>();
            if (origins == null)
            {
                await _logger.LogInformationAsync("Allowed origins configuration is null, please update appsettings.",
                    null, context.Request.Path, _localLogger);
            }
            else
            {
                origins = $"{origins},";
                allowedOrigins = origins.Split(',').ToList();
            }

            //empty origin allowed only for development purpose
            if (allowedOrigins.Any(x => x.Trim() == requestDomain || string.IsNullOrEmpty(requestDomain)))
            {
                var token = context.Request.Headers["Authorization"].ToString();

                if (!string.IsNullOrEmpty(token))
                {
                    var decryptedToken = _encyptionService.Decrypt(token);
                    if (!decryptedToken.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                    {
                        decryptedToken = $"Bearer {decryptedToken}";
                    }
                    context.Request.Headers.Add("Authorization", decryptedToken);
                }
                await _next(context);
            }
            else
            {
                await _logger.LogInformationAsync("Request origin didn't matched any allowed origin",
                    null, context.Request.Path, _localLogger);

                context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                context.Response.ContentType = "application/json";
                var response = JsonConvert.SerializeObject(
                    new
                    {
                        isSuccess = true,
                        statusCode = HttpStatusCode.Forbidden,
                        message = "You are not allowed to make request to API"
                    });
                await context.Response.WriteAsync(response);
            }
        }

    }
}
