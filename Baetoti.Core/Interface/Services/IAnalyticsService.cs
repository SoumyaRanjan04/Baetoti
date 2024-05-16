using Baetoti.Shared.Request.Analytic;
using Baetoti.Shared.Request.Shared;
using Baetoti.Shared.Response.Analytic;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Baetoti.Core.Interface.Services
{
    public interface IAnalyticsService
    {
        Task<List<MapResponse>> GetMapData(FilterRequest filter);

        Task<MarkovResponse> GetMarkovData(RequestID filter);

        Task<StatisticResponse> GetStatisticData(FilterRequest filter);

        Task<FinanceResponse> GetFinanceData(FilterRequest filter);

        Task<List<RevenuePerDayResponse>> GetRevenuPerDayData(RevenuePerDayRequest filter);

        Task<List<CohortResponse>> GetCohortData(CohortFilter filter);

    }
}
