using Baetoti.Core.Entites;
using Baetoti.Core.Interface.Repositories;
using Baetoti.Infrastructure.Data.Context;
using Baetoti.Infrastructure.Data.Repositories.Base;
using Baetoti.Shared.Request.SupportRequest;
using Baetoti.Shared.Response.SupportRequest;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Baetoti.Shared.Response.Shared;
using System;
using Baetoti.Shared.Enum;
using Baetoti.Shared.Request.Dashboard;
using DashboardResponse = Baetoti.Shared.Response.SupportRequest.DashboardResponse;
using System.Collections.Generic;

namespace Baetoti.Infrastructure.Data.Repositories
{
    public class SupportRequestRepository : EFRepository<SupportRequest>, ISupportRequestRepository
    {

        private readonly BaetotiDbContext _dbContext;

        public SupportRequestRepository(BaetotiDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<PaginationResponse> GetAll(SupportRequestFilter request)
        {
            var list = from sr in _dbContext.SupportRequests
                       where (request.UserID == 0 || sr.UserID == request.UserID) &&
                       sr.SupportRequestStatus == request.SupportRequestStatus
                       select new SupportRequestResponse
                       {
                           ID = sr.ID,
                           UserID = sr.UserID,
                           SupportRequestType = sr.SupportRequestType,
                           SupportRequestTypeValue = ((SupportRequestType)sr.SupportRequestType).ToString(),
                           Title = sr.Title,
                           Comments = sr.Comments,
                           UserRating = sr.UserRating,
                           RecordDateTime = sr.RecordDateTime,
                           OpenDateTime = sr.OpenDateTime,
                           ResolveDateTime = sr.ResolveDateTime
                       };

            var totalRecords = list.Count();
            var totalPages = (int)Math.Ceiling((double)totalRecords / request.PageSize);

            PaginationResponse response = new PaginationResponse
            {
                CurrentPage = request.PageNumber,
                TotalPages = totalPages,
                PageSize = request.PageSize,
                TotalCount = totalRecords
            };

            if (!string.IsNullOrEmpty(request.SearchValue))
                response.Data = (await list.ToListAsync())
                                .Where(x =>
                                    x.Title.ToLower().Contains(request.SearchValue.ToLower()) ||
                                    x.Comments.ToLower().Contains(request.SearchValue.ToLower())
                                )
                                .Skip((request.PageNumber - 1) * request.PageSize)
                                .Take(request.PageSize)
                                .ToList();
            else
                response.Data = await list.Skip((request.PageNumber - 1) * request.PageSize)
                    .Take(request.PageSize).ToListAsync();

            return response;
        }

        public async Task<SupportRequestByIDResponse> GetById(long ID, long UserID, string UserRole)
        {
            return await (from sr in _dbContext.SupportRequests
                          join e in _dbContext.Employee on sr.SupervisorID equals e.ID into tempEmployee
                          from te in tempEmployee.DefaultIfEmpty()
                          join d in _dbContext.Designations on te.DesignationID equals d.ID into tempDesignation
                          from td in tempDesignation.DefaultIfEmpty()
                          where sr.ID == ID && (!string.IsNullOrEmpty(UserRole) || sr.UserID == UserID)
                          select new SupportRequestByIDResponse
                          {
                              SupportRequestType = sr.SupportRequestType,
                              SupportRequestTypeValue = ((SupportRequestType)sr.SupportRequestType).ToString(),
                              UserID = sr.UserID,
                              ID = sr.ID,
                              Title = sr.Title,
                              Picture = sr.Picture,
                              Comments = sr.Comments,
                              SupervisorComments = sr.SupervisorComments,
                              SupervisorCommentsForUser = sr.SupervisorCommentsForUser,
                              ActionTaken = sr.ActionTaken,
                              RootCause = sr.RootCause,
                              UserRating = sr.UserRating,
                              UserFeedback = sr.UserFeedback,
                              SupportRequestStatus = sr.SupportRequestStatus,
                              CSSID = te == null ? 0 : te.ID,
                              CSSName = te == null ? "" : $"{te.FirstName} {te.LastName}",
                              SupportUserName = te == null ? "" : $"{te.FirstName} {te.LastName}",
                              SupportUserDesignation = td == null ? "" : td.DesignationName,
                              SupportUserPicture = te == null ? "" : te.Picture,
                              RecordDateTime = sr.RecordDateTime,
                              OpenDateTime = sr.OpenDateTime,
                              ResolveDateTime = sr.ResolveDateTime
                          }).FirstOrDefaultAsync();
        }

        public async Task<DashboardResponse> GetDashboardData(DashboardRequest request)
        {

            var data = await _dbContext.SupportRequests
                .Where(x => x.RecordDateTime >= request.StartDate && x.RecordDateTime <= request.EndDate).ToListAsync();

            if (data != null && data.Any())
            {
                DashboardResponse response = new DashboardResponse
                {
                    Pending = data.Count(x => x.SupportRequestStatus == 0),
                    Open = data.Count(x => x.SupportRequestStatus == 1),
                    Resolved = data.Count(x => x.SupportRequestStatus == 2),
                    CustomerSatisfaction = data.Average(x => x.UserRating)
                };

                response.GraphData = data
                                    .GroupBy(x => x.RecordDateTime)  // Group by date
                                    .Select(g => new CSSGraphData
                                    {
                                        Date = g.Key,
                                        AverageCustomerSatisfaction = g.Average(x => x.UserRating),
                                        AverageResponseTime = CalculateAverageResponseTime(g),
                                        AverageResolutionTime = CalculateAverageResolutionTime(g)
                                    })
                                    .ToList();

                return response;
            }
            return new DashboardResponse();
        }

        private decimal CalculateAverageResponseTime(IEnumerable<SupportRequest> supportRequests)
        {
            var validRequests = supportRequests.Where(x => x.OpenDateTime != null && x.RecordDateTime != null);
            if (validRequests.Any())
            {
                var totalHours = validRequests.Average(x => (x.OpenDateTime - x.RecordDateTime).TotalHours);
                return (decimal)totalHours;
            }
            return 0;
        }

        private decimal CalculateAverageResolutionTime(IEnumerable<SupportRequest> supportRequests)
        {
            var validRequests = supportRequests.Where(x => x.OpenDateTime != null && x.ResolveDateTime != null);
            if (validRequests.Any())
            {
                var totalHours = validRequests.Average(x => (x.ResolveDateTime - x.OpenDateTime).TotalHours);
                return (decimal)totalHours;
            }
            return 0;
        }

    }
}
