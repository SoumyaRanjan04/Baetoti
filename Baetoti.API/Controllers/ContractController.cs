using Baetoti.API.Controllers.Base;
using Baetoti.Core.Interface.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System;
using Baetoti.Shared.Request.Contract;
using Microsoft.AspNetCore.Hosting;
using Baetoti.Shared.Enum;
using Baetoti.Core.Entites;
using Baetoti.Shared.Response.Driver;
using Provider = Baetoti.Core.Entites.Provider;
using Baetoti.Shared.Response.Shared;
using System.Text;
using Baetoti.Shared.Response.Dashboard;

namespace Baetoti.API.Controllers
{
    public class ContractController : ApiBaseController
    {

        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly IUserRepository _userRepository;
        private readonly IDriverRepository _driverRepository;
        private readonly IProviderRepository _providerRepository;
        private readonly IProviderBusinessRepository _providerBusinessRepository;

        public ContractController(
            IWebHostEnvironment hostingEnvironment,
            IUserRepository userRepository,
            IDriverRepository driverRepository,
            IProviderRepository providerRepository,
            IProviderBusinessRepository providerBusinessRepository
            )
        {
            _hostingEnvironment = hostingEnvironment;
            _userRepository = userRepository;
            _driverRepository = driverRepository;
            _providerRepository = providerRepository;
            _providerBusinessRepository = providerBusinessRepository;
        }

        [HttpPost("Get")]
        public async Task<IActionResult> Get([FromBody] ContractRequest request)
        {
            var projectRoot = _hostingEnvironment.ContentRootPath;

            string host = HttpContext.Request.Host.Host;

            string logoUrl = host + "/" + "Contracts/logo.jpg";
            string logo1Url = host + "/" + "Contracts/logo-1.jpg";
            string logo2Url = host + "/" + "Contracts/logo-2.jpg";

            User user = await _userRepository.GetByIdAsync(request.UserID);
            if (request.ContractType == (int)ContractType.Driver)
            {
                GetDriverByIDResponse driver = await _driverRepository.GetByUserID(request.UserID);
                string result = $"{projectRoot}\\wwwroot\\Contracts\\Driver.html";
                string builder = System.IO.File.ReadAllText(result);
                StringBuilder contract = new StringBuilder(builder);

                contract.Replace("/Contracts/logo-1.jpg", logo1Url);
                contract.Replace("/Contracts/logo-2.jpg", logo2Url);
                contract.Replace("/Contracts/logo.jpg", logoUrl);

                contract.Replace("{Contract ID}", "");
                contract.Replace("{User ID}", "User ID : " + driver.ID.ToString());
                contract.Replace("{UserPhonenumber}", "User phone number : " + user.Phone);

                contract.Replace("{ContractNumber}", driver.ID.ToString());
                contract.Replace("{ContractNumber}", driver.ID.ToString());
                contract.Replace("{Day}", DateTime.Now.DayOfWeek.ToString());
                contract.Replace("{Today Date} ", DateTime.Now.ToString("yyyy/MM/dd"));
                contract.Replace("{FullName}", $"{user.FirstName} {user.LastName}");
                contract.Replace("{GovtID}", driver.Nationality);
                contract.Replace("{FreelanceCategory}", "Captain");
                contract.Replace("{FreelanceID}", driver.IDNumber);
                contract.Replace("{IssuanceDate}", driver.RegistrationDate.ToString("yyyy/MM/dd"));
                contract.Replace("{ExpirationDate}", driver.IDExpiryDate);
                contract.Replace("{Email}", user.Email);
                contract.Replace("{Signature}", "Siraj");
                contract.Replace("{CaptainSignature}", $"{user.FirstName}");
                contract.Replace("{CaptainName}", $"{user.FirstName} {user.LastName}");

                return Ok(new SharedResponse(true, 200, "", contract.ToString()));
            }
            else
            {
                Provider provider = await _providerRepository.GetByUserID(request.UserID);
                ProviderBusiness providerBusiness = await _providerBusinessRepository.GetByProviderID(provider.ID);
                if (providerBusiness.IsCorpoarate)
                {
                    string result = $"{projectRoot}\\wwwroot\\Contracts\\ProviderCorporate.html";
                    string builder = System.IO.File.ReadAllText(result);
                    StringBuilder contract = new StringBuilder(builder);

                    contract.Replace("/Contracts/logo-1.jpg", logo1Url);
                    contract.Replace("/Contracts/logo-2.jpg", logo2Url);
                    contract.Replace("/Contracts/logo.jpg", logoUrl);

                    contract.Replace("{Contract ID}", "");
                    contract.Replace("{User ID}", "User ID : " + provider.ID.ToString());
                    contract.Replace("{UserPhonenumber}", "User phone number : " + user.Phone);

                    contract.Replace("{ContractNumber}", provider.ID.ToString());
                    contract.Replace("{Day}", DateTime.Now.DayOfWeek.ToString());
                    contract.Replace("{TodayDate}", DateTime.Now.ToString("yyyy/MM/dd"));
                    contract.Replace("{CompanyName}", providerBusiness.CompanyName);
                    contract.Replace("{Adress}", providerBusiness.CompanyAddress);
                    contract.Replace("{CRNumber}", providerBusiness.CommercialNumber);
                    contract.Replace("{CRDate}", providerBusiness.CommercialExpiry.ToString("yyyy/MM/dd"));
                    contract.Replace("{FullName}", $"{user.FirstName} {user.LastName}");
                    contract.Replace("{Nationality}", provider.GovernmentID);
                    contract.Replace("{GovtID}", provider.GovernmentID);
                    contract.Replace("{Position}", providerBusiness.Position);
                    contract.Replace("{Email}", user.Email);
                    contract.Replace("{FreelanceCategory}", providerBusiness.IsCorpoarate ? "Corporate" : "Individual");
                    contract.Replace("{Signature}", "Siraj");
                    contract.Replace("{ProviderSignature}", $"{user.FirstName}");
                    contract.Replace("{ProviderFullName}", $"{providerBusiness.FirstName} {providerBusiness.MiddleName} {providerBusiness.LastName}");

                    return Ok(new SharedResponse(true, 200, "", contract.ToString()));
                }
                else
                {
                    string result = $"{projectRoot}\\wwwroot\\Contracts\\ProviderIndividual.html";
                    string templatePath = System.IO.File.ReadAllText(result);
                    StringBuilder contract = new StringBuilder(templatePath);

                    contract.Replace("/Contracts/logo-1.jpg", logo1Url);
                    contract.Replace("/Contracts/logo-2.jpg", logo2Url);
                    contract.Replace("/Contracts/logo.jpg", logoUrl);

                    contract.Replace("{Contract ID}", "");
                    contract.Replace("{User ID}", "User ID : " + provider.ID.ToString());
                    contract.Replace("{UserPhonenumber}", "User phone number : " + user.Phone);

                    contract.Replace("{ContractNumber}", provider.ID.ToString());
                    contract.Replace("{Day}", DateTime.Now.DayOfWeek.ToString());
                    contract.Replace("{TodayDate}", DateTime.Now.ToString("yyyy/MM/dd"));
                    contract.Replace("{FullName}", $"{user.FirstName} {user.LastName}");
                    contract.Replace("{GovtID}", provider.GovernmentID);
                    contract.Replace("{FreelanceCategory}", "Individual");
                    contract.Replace("{CertificateNumber}", providerBusiness.CommercialNumber);
                    contract.Replace("{StartDate}", providerBusiness.CommercialStartDate.ToString("yyyy/MM/dd"));
                    contract.Replace("{EndDate}", providerBusiness.CommercialExpiry.ToString("yyyy/MM/dd"));
                    contract.Replace("{Email}", user.Email);
                    contract.Replace("{Signature}", "Siraj");
                    contract.Replace("{ProviderSignature}", $"{user.FirstName}");
                    contract.Replace("{ProviderSignature}", $"{user.FirstName}");
                    contract.Replace("{ProviderName}", $"{providerBusiness.FirstName} {providerBusiness.LastName}");

                    return Ok(new SharedResponse(true, 200, "", contract.ToString()));
                }
            }
        }

    }
}
