using Baetoti.Core.Entites;
using Baetoti.Core.Interface.Repositories;
using Baetoti.Infrastructure.Data.Context;
using Baetoti.Infrastructure.Data.Repositories.Base;
using Baetoti.Shared.Response.Email;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace Baetoti.Infrastructure.Data.Repositories
{
    public class EmailTemplateRepository : EFRepository<EmailTemplate>, IEmailTemplateRepository
    {

        private readonly BaetotiDbContext _dbContext;

        public EmailTemplateRepository(BaetotiDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<EmailTemplateResponse>> GetAllTemplate()
        {
            return await (from t in _dbContext.EmailTemplates
                          select new EmailTemplateResponse
                          {
                              ID = t.ID,
                              Subject = t.Subject,
                              HtmlBody = t.HtmlBody
                          }).ToListAsync();
        }

        public async Task<EmailTemplateResponse> GetTemplateByID(long ID)
        {
            return await (from t in _dbContext.EmailTemplates
                          where t.ID == ID
                          select new EmailTemplateResponse
                          {
                              ID = t.ID,
                              Subject = t.Subject,
                              HtmlBody = t.HtmlBody
                          }).FirstOrDefaultAsync();
        }

        public async Task<EmailTemplateResponse> GetTemplateByType(int Type)
        {
            return await (from t in _dbContext.EmailTemplates
                          where t.EmailType == Type
                          select new EmailTemplateResponse
                          {
                              ID = t.ID,
                              Subject = t.Subject,
                              HtmlBody = t.HtmlBody
                          }).FirstOrDefaultAsync();
        }

    }
}
