using Baetoti.Core.Entites;
using Baetoti.Core.Interface.Services;
using Microsoft.Extensions.Configuration;
using System;

namespace Baetoti.Infrastructure.Services
{
    public class SiteConfigService : ISiteConfigService
    {

        private readonly SiteConfig _siteConfig;
        private readonly IConfiguration _configuration;

        public SiteConfigService(IConfiguration configuration)
        {
            _configuration = configuration;
            _siteConfig = GetSettingsFromDatabase();
        }

        public SiteConfig GetSiteConfigs()
        {
            return _siteConfig;
        }

        private SiteConfig GetSettingsFromDatabase()
        {
            return new SiteConfig
            {
                GovtAPIURL = _configuration?.GetSection("GovtAPIConfiguration")["BaseURL"],
                GovtAPIPassword = _configuration?.GetSection("GovtAPIConfiguration")["Password"],
                GovtAPICompanyName = _configuration?.GetSection("GovtAPIConfiguration")["Company"],
                UseLocalDataForGovtAPIs = _configuration?.GetSection("GovtAPIConfiguration")["UseLocalDataForGovtAPIs"],
                ChatAPIURL = _configuration?.GetSection("ChatAPIConfiguration")["BaseURL"],
                NotificationAPIURL = _configuration?.GetSection("NotificationAPIConfiguration")["BaseURL"],
                PaymentAPIURL = _configuration?.GetSection("PaymentAPIConfiguration")["BaseURL"],
                InstagramAPIURL = _configuration?.GetSection("InstagramAPIConfiguration")["BaseURL"],
                IsSMSEnabled = Convert.ToBoolean(_configuration?.GetSection("Generall")["IsSMSEnabled"])
            };
        }

    }
}
