using Baetoti.Core.Interface.Base;
using Baetoti.Shared.Extentions;
using Baetoti.Shared.Response.Shared;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using System;
using System.Net;

namespace Baetoti.API.Middlewares
{
    public static class MiddlewareExtensions
    {
        public static void UseJwtMiddleware(this IApplicationBuilder app)
        {
            app.UseMiddleware<JwtMiddleware>();
        }

        public static void UseExceptionMiddleware(this IApplicationBuilder app,  IAppLogger<Startup> logger)
        {
            app.UseExceptionHandler(appException =>
            {
                appException.Run(async context =>
                {
                    var exceptionContext = context.Features.Get<IExceptionHandlerPathFeature>();
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    context.Response.ContentType = "application/json";
                    var response = GenericResponses.ExceptionResponse();
                    try
                    {
                        if (exceptionContext != null)
                        {
                            await logger.LogErrorAsync(exceptionContext?.Error, exceptionContext.Path, context.User.GetUserId());
                            await context.Response.WriteAsync(response);
                        }
                    }
                    catch (Exception ex)
                    {
                        await logger.LogErrorAsync(ex, exceptionContext?.Path, context.User.GetUserId());
                        await context.Response.WriteAsync(response);
                    }
                });
            });
        }

    }
}
