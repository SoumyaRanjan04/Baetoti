namespace Baetoti.Core.Interface.Services
{
    public interface IEncryptionService
    {
        string Encrypt(string valueToEncypt);

        string Decrypt(string valueToDecrypt);

    }
}
