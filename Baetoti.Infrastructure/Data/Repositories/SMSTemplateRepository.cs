using Baetoti.Core.Entites;
using Baetoti.Core.Interface.Repositories;
using Baetoti.Infrastructure.Data.Context;
using Baetoti.Infrastructure.Data.Repositories.Base;
using Baetoti.Shared.Response.SMS;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace Baetoti.Infrastructure.Data.Repositories
{
    public class SMSTemplateRepository : EFRepository<SMSTemplate>, ISMSTemplateRepository
    {

        private readonly BaetotiDbContext _dbContext;

        public SMSTemplateRepository(BaetotiDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<SMSTemplateResponse>> GetAllTemplate()
        {
            return await (from s in _dbContext.SMSTemplates
                          select new SMSTemplateResponse
                          {
                              ID = s.ID,
                              SMSText = s.SMSText
                          }).ToListAsync();
        }

        public async Task<SMSTemplateResponse> GetTemplateByID(long ID)
        {
            return await (from s in _dbContext.SMSTemplates
                          where s.ID == ID
                          select new SMSTemplateResponse
                          {
                              ID = s.ID,
                              SMSText = s.SMSText
                          }).FirstOrDefaultAsync();
        }

        public async Task<SMSTemplateResponse> GetTemplateByType(int Type)
        {
            return await (from s in _dbContext.SMSTemplates
                          where s.SMSType == Type
                          select new SMSTemplateResponse
                          {
                              ID = s.ID,
                              SMSText = s.SMSText
                          }).FirstOrDefaultAsync();
        }

    }
}
