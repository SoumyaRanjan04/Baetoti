using Baetoti.Core.Entites;
using Baetoti.Core.Interface.Base;
using Baetoti.Shared.Response.Email;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Baetoti.Core.Interface.Repositories
{
    public interface IEmailTemplateRepository : IAsyncRepository<EmailTemplate>
    {
        Task<List<EmailTemplateResponse>> GetAllTemplate();

        Task<EmailTemplateResponse> GetTemplateByID(long ID);

        Task<EmailTemplateResponse> GetTemplateByType(int Type);

    }
}
