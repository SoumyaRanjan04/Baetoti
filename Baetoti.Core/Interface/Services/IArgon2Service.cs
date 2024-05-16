namespace Baetoti.Core.Interface.Services
{
    public interface IArgon2Service
    {
        string GenerateHash(string password);

        bool VerifyHash(string password, string hashPassword);

    }
}
