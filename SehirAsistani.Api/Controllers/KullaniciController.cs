using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SehirAsistanim.Domain.Dto_s;
using SehirAsistanim.Domain.Entities;
using SehirAsistanim.Domain.Interfaces;
using SehirAsistanim.Infrastructure.Services;
using SehirAsistanim.Infrastructure.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SehirAsistani.Api.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class KullaniciController : ControllerBase
    {
        private readonly IKullaniciService _service;
        private readonly ISikayetLoglariService _logService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly EmailService _emailService;

        public KullaniciController(IKullaniciService service, ISikayetLoglariService logService, IUnitOfWork unitOfWork, EmailService emailService)
        {
            _service = service;
            _logService = logService;
            _unitOfWork = unitOfWork;
            _emailService = emailService;
        }

        private int? GetUserIdFromClaims()
        {
            if (User.Identity.IsAuthenticated)
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
                {
                    return userId;
                }
            }
            return null;
        }

        #region GetAll Kullanici
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<List<Kullanici>> GetAll()
        {
            try
            {
                var result = await _service.GetAll();

                await _logService.LogAsync(new SikayetLog
                {
                    KullaniciId = GetUserIdFromClaims(),
                    Aciklama = $"Tüm kullanıcılar getirildi. Toplam: {result.Count}",
                    Tarih = DateTime.UtcNow
                });

                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"GetAll hata: {ex.Message}");
                return new List<Kullanici>();
            }
        }
        #endregion

        #region GetById
        [HttpGet("{id}")]
        public async Task<Kullanici> GetById(int id)
        {
            try
            {
                var result = await _service.GetById(id);

                await _logService.LogAsync(new SikayetLog
                {
                    KullaniciId = GetUserIdFromClaims(),
                    Aciklama = result != null
                        ? $"Kullanıcı ID={id} getirildi."
                        : $"Kullanıcı ID={id} bulunamadı.",
                    Tarih = DateTime.UtcNow
                });

                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"GetById hata: {ex.Message}");
                return null;
            }
        }
        #endregion

        #region Add Kullanici
        [HttpPost]
        public async Task<Kullanici?> Add([FromBody] Kullanici kullanici)
        {
            try
            {
                var added = await _service.AddKullanici(kullanici);

                if (added != null)
                {
                    await _logService.LogAsync(new SikayetLog
                    {
                        KullaniciId = GetUserIdFromClaims(),
                        Aciklama = $"Yeni kullanıcı eklendi. ID={added.Id}, Email={added.Email}",
                        Tarih = DateTime.UtcNow
                    });
                }

                return added;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Add hata: {ex.Message}");
                return null;
            }
        }
        #endregion

        #region Update Kullanici
        [HttpPut]
        public async Task<Kullanici?> Update([FromBody] Kullanici kullanici)
        {
            try
            {
                var beforeUpdate = await _service.GetById(kullanici.Id);
                var result = await _service.UpdateKullanici(kullanici);

                if (result != null)
                {
                    string changes = GetChangesDescription(beforeUpdate, result);

                    await _logService.LogAsync(new SikayetLog
                    {
                        KullaniciId = GetUserIdFromClaims(),
                        Aciklama = $"Kullanıcı güncellendi. ID={result.Id}. Değişiklikler: {changes}",
                        Tarih = DateTime.UtcNow
                    });
                }

                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Update hata: {ex.Message}");
                return null;
            }
        }
        #endregion

        #region Delete Kullanici
        [HttpDelete("{id}")]
        public async Task<bool> Delete(int id)
        {
            try
            {
                var beforeDelete = await _service.GetById(id);
                var result = await _service.DeleteKullanici(id);

                if (result)
                {
                    await _logService.LogAsync(new SikayetLog
                    {
                        KullaniciId = GetUserIdFromClaims(),
                        Aciklama = $"Kullanıcı silindi. ID={id}, Email={beforeDelete?.Email}",
                        Tarih = DateTime.UtcNow
                    });
                }

                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Delete hata: {ex.Message}");
                return false;
            }
        }
        #endregion

        #region Total Kullanici Sayisi
        [HttpGet]
        public async Task<int> TotalKullaniciSayisi()
        {
            try
            {
                var count = await _service.TotalKullaniciSayisi();

                await _logService.LogAsync(new SikayetLog
                {
                    KullaniciId = GetUserIdFromClaims(),
                    Aciklama = $"Toplam kullanıcı sayısı sorgulandı. Sonuç: {count}",
                    Tarih = DateTime.UtcNow
                });

                return count;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"TotalKullaniciSayisi hata: {ex.Message}");
                return 0;
            }
        }
        #endregion




        #region Şikayet Çözüm Bildirimi Gönder
        [HttpPost("sikayet-cozum-bildirimi")]
        public async Task<IActionResult> SendComplaintSolutionNotification([FromBody] ComplaintSolutionNotificationRequest request)
        {
            try
            {
                var currentUserId = GetUserIdFromClaims();
                if (!currentUserId.HasValue)
                {
                    return Unauthorized(new { Success = false, Message = "Kullanıcı kimliği belirlenemedi" });
                }

                // Kullanıcı ve şikayet bilgilerini al
                var kullanici = await _service.GetById(request.KullaniciId);
                var sikayet = await _unitOfWork.Repository<Sikayet>().GetById(request.SikayetId);

                if (kullanici == null || sikayet == null)
                {
                    return NotFound(new { Success = false, Message = "Kullanıcı veya şikayet bulunamadı" });
                }

                // Email gönder
                await _emailService.SendComplaintSolvedNotification(
                    request.KullaniciId,
                    request.SikayetId
                );

                // Log kaydı
                await _logService.LogAsync(new SikayetLog
                {
                    KullaniciId = currentUserId,
                    Aciklama = $"{kullanici.Email} kullanıcısına şikayet çözüm bildirimi gönderildi. Şikayet ID: {request.SikayetId}",
                    Tarih = DateTime.UtcNow
                });

                return Ok(new { Success = true, Message = "Bildirim e-postası başarıyla gönderildi" });
            }
            catch (Exception ex)
            {
                await _logService.LogAsync(new SikayetLog
                {
                    KullaniciId = GetUserIdFromClaims(),
                    Aciklama = $"Şikayet çözüm bildirimi gönderilirken hata: {ex.Message}",
                    Tarih = DateTime.UtcNow
                   
                });

                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { Success = false, Message = "Bildirim gönderilirken bir hata oluştu" });
            }
        }
        #endregion






        private string GetChangesDescription(Kullanici before, Kullanici after)
        {
            if (before == null || after == null)
                return "Önceki veya sonraki kullanıcı bilgisi bulunamadı.";

            var changes = new List<string>();

            if (before.Email != after.Email)
                changes.Add($"Email: '{before.Email}' => '{after.Email}'");

            if (before.Isim != after.Isim)
                changes.Add($"AdSoyad: '{before.Isim}' => '{after.Isim}'");

            if (before.Soyisim != after.Soyisim)
                changes.Add($"AdSoyad: '{before.Soyisim}' => '{after.Soyisim}'");

            if (before.Rol != after.Rol)
                changes.Add($"Rol: '{before.Rol}' => '{after.Rol}'");

            if (changes.Count == 0)
                return "Değişiklik yok.";

            return string.Join("; ", changes);
        }
    }
}
