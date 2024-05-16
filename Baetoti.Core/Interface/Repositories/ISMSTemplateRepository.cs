using Baetoti.Core.Entites;
using Baetoti.Core.Interface.Base;
using Baetoti.Shared.Response.SMS;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Baetoti.Core.Interface.Repositories
{
    public interface ISMSTemplateRepository : IAsyncRepository<SMSTemplate>
    {
        Task<List<SMSTemplateResponse>> GetAllTemplate();

        Task<SMSTemplateResponse> GetTemplateByID(long ID);

        Task<SMSTemplateResponse> GetTemplateByType(int Type);

    }
}
