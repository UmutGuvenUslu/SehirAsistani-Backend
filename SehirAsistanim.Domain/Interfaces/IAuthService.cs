using SehirAsistanim.Domain.Dto_s;
using SehirAsistanim.Domain.Entities;

namespace SehirAsistanim.Domain.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResultDto> LoginAsync(LoginDto dto);
        Task<AuthResultDto> RegisterAsync(RegisterDto dto);
        Task<string> GenerateJwtToken(Kullanici user);
        Task<bool> VerifyPassword(string hash, string password);
        Task<bool> IsEmailRegistered(string email);
        Task<bool> TcAnaliz(string tc);



    }
}
