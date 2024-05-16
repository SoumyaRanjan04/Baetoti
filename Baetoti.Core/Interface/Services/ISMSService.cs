using System.Threading.Tasks;

namespace Baetoti.Core.Interface.Services
{
    public interface ISMSService
    {
        public Task<string> SendSMS(string CellNumber, string SMSText);

    }
}
