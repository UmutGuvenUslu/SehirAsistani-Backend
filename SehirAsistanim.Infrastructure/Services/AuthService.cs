using SehirAsistanim.Domain.Dto_s;
using SehirAsistanim.Domain.Entities;
using SehirAsistanim.Domain.Enums;
using SehirAsistanim.Domain.Interfaces;

namespace SehirAsistanim.Infrastructure.Services
{
    public class AuthService: IAuthService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly JwtService _jwtService;

        public AuthService(IUnitOfWork unitOfWork, JwtService jwtService)
        {
            _unitOfWork = unitOfWork;
            _jwtService = jwtService;
        }

        #region Giriş Yap
        public async Task<AuthResultDto> LoginAsync(LoginDto dto)
        {
            var kullanici = _unitOfWork.Repository<Kullanici>().GetAll().Result
                .FirstOrDefault(k => k.Email == dto.Email);

            if (kullanici == null)
            {
                throw new Exception("Kullanıcı bulunamadı.");
            }

            if (!BCrypt.Net.BCrypt.Verify(dto.Password, kullanici.Sifre))
            {
                throw new Exception("Şifre yanlış.");
            }

            var token = await GenerateJwtToken(kullanici);

            return new AuthResultDto
            {
                Success = true,
                Message = "Giriş başarılı",
                Token = token,
                RefreshToken = null,
                ExpireAt = DateTime.UtcNow.AddHours(1),
                UserId = kullanici.Id,
                Email = kullanici.Email,
                FullName = $"{kullanici.Isim} {kullanici.Soyisim}",
                TelefonNo = kullanici.TelefonNo,
                Cinsiyet = kullanici.Cinsiyet,
                DogumTarihi = kullanici.DogumTarihi,
                Rol = kullanici.Rol.ToString()
            };
        }
        #endregion

        #region Kayıt Ol
        public async Task<AuthResultDto> RegisterAsync(RegisterDto dto)
        {
            if (!(_unitOfWork.Repository<Kullanici>().GetAll().Result.FirstOrDefault(k => k.Email == dto.Email) == null))
            {
                throw new Exception("Bu e-posta zaten kayıtlı.");
            }

            if (dto.Sifre != dto.SifreTekrar)
            {
                throw new Exception("Şifreler eşleşmiyor.");
            }

            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(dto.Sifre);

            var kullanici = new Kullanici
            {
                Isim = dto.Isim,
                Soyisim = dto.Soyisim,
                TC = dto.TC,
                Email = dto.Email,
                TelefonNo = dto.TelefonNo,
                Cinsiyet = dto.Cinsiyet,
                DogumTarihi = dto.DogumTarihi.HasValue ? dto.DogumTarihi.Value.ToUniversalTime() : (DateTime?)null,
                Sifre = hashedPassword,
                KayitTarihi = DateTime.UtcNow,
                Rol = rolturu.Vatandas
            };

            await _unitOfWork.Repository<Kullanici>().Add(kullanici);
            _unitOfWork.CommitAsync();

            var token = await GenerateJwtToken(kullanici);

            return new AuthResultDto
            {
                Success = true,
                Message = "Kayıt başarılı",
                Token = token,
                RefreshToken = null,
                ExpireAt = DateTime.UtcNow.AddHours(1),
                UserId = kullanici.Id,
                Email = kullanici.Email,
                FullName = $"{kullanici.Isim} {kullanici.Soyisim}",
                TelefonNo = kullanici.TelefonNo,
                Cinsiyet = kullanici.Cinsiyet,
                DogumTarihi = kullanici.DogumTarihi,
                Rol = kullanici.Rol.ToString()
            };
        }
        #endregion

        #region JWT Üretimi
        public async Task<string> GenerateJwtToken(Kullanici kullanici)
        {
            return await _jwtService.GenerateJwtToken(kullanici);
        }
        #endregion

        #region Şifre Doğrulama
        public Task<bool> VerifyPassword(string hash, string password)
        {
            bool result = BCrypt.Net.BCrypt.Verify(password, hash);
            return Task.FromResult(result);
        }
        #endregion

        #region KullaniciEmailVarMi
        public async Task<bool> IsEmailRegistered(string email)
        {
            var kullanici = await _unitOfWork.Repository<Kullanici>().GetAll();
            return kullanici.Any(k => k.Email == email);
        }
        #endregion

        #region TCAnaliz

        public async Task<bool> TcAnaliz(string tc)
        {
            if (string.IsNullOrWhiteSpace(tc) || tc.Length != 11 || !tc.All(char.IsDigit))
                return false;

            if (tc.StartsWith("0"))
                return false;

            int[] digits = tc.Select(c => int.Parse(c.ToString())).ToArray();

            int oddSum = digits[0] + digits[2] + digits[4] + digits[6] + digits[8];
            int evenSum = digits[1] + digits[3] + digits[5] + digits[7];

            int digit10 = ((oddSum * 7) - evenSum) % 10;
            int digit11 = digits.Take(10).Sum() % 10;

            return digit10 == digits[9] && digit11 == digits[10];
        }

        #endregion


    }
}

