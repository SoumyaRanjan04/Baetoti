namespace Baetoti.Core.Interface.Services
{
    public interface IRijndaelEncryptionService
    {
        string EncryptString(string InputText);
        string DecryptString(string InputText);
        string EncryptPassword(string plainPassword, string saltValue);
        string DecryptPassword(string plainPassword, string saltValue);
        string GenerateSalt(int minLength, int maxLength);
    }
}
