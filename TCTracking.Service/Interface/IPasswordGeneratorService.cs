

namespace TCTracking.Service.Interface
{
    public interface IPasswordGeneratorService
    {

        string Encrypt(string email, string pwd);

        bool CheckPassword(string email, string hashPassword, string pwd);

        string GetRandomPassword();

    }
}
