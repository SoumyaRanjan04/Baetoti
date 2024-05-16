using Baetoti.Core.Interface.Services;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Configuration;

namespace Baetoti.Infrastructure.Services
{
    public class EncryptionService : IEncryptionService
    {

        private readonly IDataProtectionProvider _dataProtectionProvider;

        private string SecretKey { get; set; }

        public EncryptionService(IConfiguration configuration, IDataProtectionProvider dataProtectionProvider)
        {
            _dataProtectionProvider = dataProtectionProvider;
            SecretKey = configuration["EncryptionKey"];

        }

        public EncryptionService(IDataProtectionProvider dataProtectionProvider)
        {
            _dataProtectionProvider = dataProtectionProvider;
        }

        public string Encrypt(string valueToEncypt)
        {
            var protector = _dataProtectionProvider.CreateProtector(SecretKey);
            return protector.Protect(valueToEncypt);
        }

        public string Decrypt(string valueToDecrypt)
        {
            var protector = _dataProtectionProvider.CreateProtector(SecretKey);
            return protector.Unprotect(valueToDecrypt);
        }

    }
}
