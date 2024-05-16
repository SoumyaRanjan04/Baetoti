using Baetoti.Shared.Request.Email;
using System.Threading.Tasks;

namespace Baetoti.Core.Interface.Services
{
    public interface IEmailService
    {
        Task<bool> SendEmailAsync(EmailRequest request);

    }

}
