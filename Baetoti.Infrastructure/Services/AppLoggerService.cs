using Baetoti.Core.Entites;
using Baetoti.Core.Interface.Base;
using Baetoti.Core.Interface.Repositories;
using Baetoti.Shared.Extentions;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Baetoti.Infrastructure.Services
{
    public class AppLoggerService<T> : IAppLogger<T> where T : class
    {

        private readonly IExceptionRepository _exceptionRepository;
        private readonly ILogger<T> _logger;

        public AppLoggerService(IExceptionRepository exceptionRepository, ILogger<T> logger)
        {
            _exceptionRepository = exceptionRepository;
            _logger = logger;
        }

        public async Task<AppException> LogErrorAsync(Exception exception, string url, string userId)
        {
            _logger.LogInformation(exception.Message);
            _logger.LogInformation(exception.StackTrace);
            var log = new AppException()
            {
                Type = "Error",
                Message = exception.Message,
                StackTrace = exception.StackTrace,
                UserId = userId,
                Url = url,
                InnerException = exception.InnerException?.Message,
                CreatedAt = DateTime.Now.ToTimeZoneTime("Arab Standard Time"),
            };
            return await _exceptionRepository.AddAsync(log);
        }

        public async Task<AppException> LogInformationAsync(string message, string url, string userId, ILogger<T> logger)
        {
            logger.LogInformation(message);
            var log = new AppException()
            {
                Type = "Information",
                Message = message,
                StackTrace = null,
                UserId = userId,
                Url = url,
                CreatedAt = DateTime.Now.ToTimeZoneTime("Arab Standard Time")
            };
            return await _exceptionRepository.AddAsync(log);
        }

        public async Task<AppException> LogValidationErrorAsync(string message, string userId, string url,
            string request, string errors, ILogger<T> logger)
        {
            logger.LogInformation($"Validation error at: {url}");
            logger.LogInformation(errors);
            var log = new AppException()
            {
                Type = "Validation Error",
                Message = message,
                StackTrace = request,
                InnerException = errors,
                UserId = userId,
                Url = url,
                CreatedAt = DateTime.Now.ToTimeZoneTime("Arab Standard Time")
            };
            return await _exceptionRepository.AddAsync(log);

        }

        public async Task<AppException> LogWarningAsync(string message, string url, string userId, ILogger<T> logger)
        {
            logger.LogInformation(message);
            var log = new AppException()
            {
                Type = "Warning",
                Message = message,
                StackTrace = null,
                UserId = userId,
                Url = url,
                CreatedAt = DateTime.Now.ToTimeZoneTime("Arab Standard Time")
            };
            return await _exceptionRepository.AddAsync(log);
        }

        public async Task<AppException> AllApiLogAsync(AppException appException)
        {
            var log = new AppException()
            {
                Type = "ApiCall",
                Message = appException.Message,
                StackTrace = null,
                UserId = appException.UserId,
                Url = appException.Url,
                RequestBody = appException.RequestBody,
                RequestType = appException.RequestType,
                CreatedAt = DateTime.Now.ToTimeZoneTime("Arab Standard Time")
            };
            return await _exceptionRepository.AddAsync(log);
        }

    }

}
