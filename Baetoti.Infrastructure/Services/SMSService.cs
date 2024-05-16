using Baetoti.Core.Interface.Services;
using System.Net.Http;
using System.Threading.Tasks;

namespace Baetoti.Infrastructure.Services
{
    public class SMSService : ISMSService
    {
        public async Task<string> SendSMS(string CellNumber, string SMSText)
        {
            var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Get, $"https://gw14.tawasolsms.com:8592/websmpp/websms?accesskey=HctomPLXLTWLegk&sid=AFAAQ&mno={CellNumber}&text={SMSText}");
            request.Headers.Add("Cookie", "JSESSIONID=99F835A3AC1C41DA6C4A4FF91421BE69");
            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }

    }
}