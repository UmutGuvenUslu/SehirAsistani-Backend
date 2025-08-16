using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using SehirAsistanim.Domain.Entities;
using SehirAsistanim.Domain.Interfaces;
using System.Security.Cryptography;

namespace SehirAsistanim.Infrastructure.Services
{
    public class EmailService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ISmtpService _smtpService;
        private readonly IMemoryCache _memoryCache;

        #region Constructor
        public EmailService(ISmtpService smtpService, IMemoryCache memoryCache, IUnitOfWork unitOfWork)
        {
            _smtpService = smtpService;
            _memoryCache = memoryCache;
            _unitOfWork = unitOfWork;
        }
        #endregion

        #region Kod Üret
        public static string GenerateCode()
        {
            using var rng = RandomNumberGenerator.Create();
            var bytes = new byte[4];
            rng.GetBytes(bytes);
            int randomValue = BitConverter.ToInt32(bytes, 0) & 0x7FFFFFFF;
            return (randomValue % 900000 + 100000).ToString();
        }
        #endregion

        #region Kod Gönder
        public async Task SendVerificationCode(string email)
        {
            var code = GenerateCode();
            _memoryCache.Set($"EmailVerification:{email}", code, TimeSpan.FromMinutes(1));

            var mesaj = $"<b>Email doğrulama kodunuz: {code}</b><br/>Bu kod 1 dakika boyunca geçerlidir.";
            await _smtpService.Gonder(email, "Email Doğrulama", mesaj);
        }
        #endregion

        #region Kod Doğrula
        public bool IsVerified(string email, string userCode)
        {
            if (_memoryCache.TryGetValue($"EmailVerification:{email}", out string? storedCode))
            {
                return storedCode == userCode;
            }

            return false;
        }
        #endregion

        #region Çözüm Bildirim Maili Gönder
        public async Task SendComplaintSolvedNotification(int kullaniciId, int sikayetId)
        {
            // Kullanıcı ve şikayet bilgilerini veritabanından al
            var kullanici = await _unitOfWork.Repository<Kullanici>().GetById(kullaniciId);
            var sikayet = await _unitOfWork.Repository<Sikayet>().GetById(sikayetId);

            if (kullanici == null || sikayet == null)
            {
                throw new Exception("Kullanıcı veya şikayet bulunamadı");
            }

            // HTML email içeriği
            var mesaj = $@"
    <html>
    <body style='font-family: Arial, sans-serif;'>
        <h2 style='color: #4CAF50;'>Şikayetiniz Çözüldü!</h2>
        <p>Sayın <b>{kullanici.Isim} {kullanici.Soyisim}</b>,</p>
        <p><b>{sikayet.Baslik}</b> başlıklı şikayetiniz çözüme ulaşmıştır.</p>
        <p>Teşekkür ederiz.</p>
        <hr>
        <p style='font-size: 12px; color: #777;'>
            Bu e-posta otomatik olarak gönderilmiştir, lütfen yanıtlamayınız.
        </p>
    </body>
    </html>";

            // SMTP ile mail gönder
            await _smtpService.Gonder(
                toEmail: kullanici.Email,
                subject: $"{sikayet.Baslik} Şikayetiniz Çözüldü",
                htmlBody: mesaj
            );
        }
        #endregion
    }
}
