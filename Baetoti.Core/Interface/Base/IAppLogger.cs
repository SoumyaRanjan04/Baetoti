using Baetoti.Core.Entites;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Baetoti.Core.Interface.Base
{
    public interface IAppLogger<T>
    {
        Task<AppException> LogInformationAsync(string message, string userId, string url, ILogger<T> logger);

        Task<AppException> LogValidationErrorAsync(string message, string userId, string url,
            string request, string errors, ILogger<T> logger);

        Task<AppException> LogWarningAsync(string message, string userId, string url, ILogger<T> logger);

        Task<AppException> LogErrorAsync(Exception exception, string url, string userId);

        Task<AppException> AllApiLogAsync(AppException appException);

    }
}
