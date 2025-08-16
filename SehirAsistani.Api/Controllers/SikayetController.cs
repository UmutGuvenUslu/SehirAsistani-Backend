using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SehirAsistanim.Domain.Dto_s;
using SehirAsistanim.Domain.Entities;
using SehirAsistanim.Domain.Enums;
using SehirAsistanim.Domain.Interfaces;
using System.Security.Claims;

[ApiController]
[Route("[controller]/[action]")]
public class SikayetController : ControllerBase
{
    private readonly ISikayetService _service;
    private readonly ISikayetLoglariService _logService;

    public SikayetController(ISikayetService service, ISikayetLoglariService logService)
    {
        _service = service;
        _logService = logService;
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

    #region Tüm Şikayetleri Getir
    [HttpGet]
    public async Task<List<SikayetDetayDto>> GetAll()
    {
        try
        {
            var result = await _service.GetAll();

            await _logService.LogAsync(new SikayetLog
            {
                KullaniciId = GetUserIdFromClaims(),
                SikayetId = null,
                Tarih = DateTime.UtcNow,
                Aciklama = $"Tüm şikayetler getirildi. Toplam: {result.Count} şikayet."
            });

            return result;
        }
        catch
        {
            return new List<SikayetDetayDto>();
        }
    }
    #endregion

    #region Id ile Şikayet Getir
    [Authorize]
    [HttpGet("{id}")]
    public async Task<Sikayet> GetById(int id)
    {
        try
        {
            var result = await _service.GetById(id);

            await _logService.LogAsync(new SikayetLog
            {
                KullaniciId = GetUserIdFromClaims(),
                SikayetId = id,
                Tarih = DateTime.UtcNow,
                Aciklama = result != null
                    ? $"Şikayet ID={id} getirildi."
                    : $"Şikayet ID={id} bulunamadı."
            });

            return result;
        }
        catch
        {
            return null;
        }
    }
    #endregion

    #region Şikayet Ekle
    [Authorize]
    [HttpPost]
    public async Task<Sikayet> Add([FromBody] Sikayet sikayet)
    {
        try
        {
            if (sikayet == null)
                return null;

            var added = await _service.AddSikayet(sikayet);

            if (added != null)
            {
                await _logService.LogAsync(new SikayetLog
                {
                    KullaniciId = GetUserIdFromClaims(),
                    SikayetId = added.Id,
                    Tarih = DateTime.UtcNow,
                    Aciklama = $"Yeni şikayet eklendi. ID={added.Id}, Başlık='{added.Baslik}'."
                });
            }

            return added;
        }
        catch
        {
            return null;
        }
    }
    #endregion

    #region Şikayet Güncelle (Admin + BirimAdmin)
    [Authorize(Roles = "Admin,BirimAdmin")]
    [HttpPut]
    public async Task<bool> Update([FromBody] Sikayet sikayet)
    {
        try
        {
            var beforeUpdate = await _service.GetById(sikayet.Id);

            var result = await _service.UpdateSikayet(sikayet);

            if (result)
            {
                string changes = GetChangesDescription(beforeUpdate, sikayet);

                await _logService.LogAsync(new SikayetLog
                {
                    KullaniciId = GetUserIdFromClaims(),
                    SikayetId = sikayet.Id,
                    Tarih = DateTime.UtcNow,
                    Aciklama = $"Şikayet güncellendi. ID={sikayet.Id}. Değişiklikler: {changes}"
                });
            }

            return result;
        }
        catch
        {
            return false;
        }
    }
    #endregion

    #region Durum Güncelle (Çözüldü Yap)
    [Authorize(Roles = "Admin,BirimAdmin")]
    [HttpPut("{id}/{durum}")]
    public async Task<bool> UpdateDurum(int id, sikayetdurumu durum)
    {
        try
        {
            var beforeUpdate = await _service.GetById(id);
            var result = await _service.UpdateDurumAsCozuldu(id, durum);

            if (result)
            {
                await _logService.LogAsync(new SikayetLog
                {
                    KullaniciId = GetUserIdFromClaims(),
                    SikayetId = id,
                    Tarih = DateTime.UtcNow,
                    Aciklama = $"Şikayet durumu güncellendi. ID={id}. Eski durum: {beforeUpdate?.Durum}, Yeni durum: {durum}"
                });
            }

            return result;
        }
        catch
        {
            return false;
        }
    }
    #endregion

    #region İstatistikler (Admin + BirimAdmin)
    [Authorize(Roles = "Admin,BirimAdmin")]
    [HttpGet]
    public async Task<int> TotalSikayetSayisi()
    {
        try
        {
            var count = await _service.TotalSikayetSayisi();

            await _logService.LogAsync(new SikayetLog
            {
                KullaniciId = GetUserIdFromClaims(),
                SikayetId = null,
                Tarih = DateTime.UtcNow,
                Aciklama = $"Toplam şikayet sayısı sorgulandı. Sonuç: {count}"
            });

            return count;
        }
        catch
        {
            return 0;
        }
    }

    [Authorize(Roles = "Admin,BirimAdmin")] // GÜNCELLENDİ
    [HttpGet]
    public async Task<int> CozulenSikayetSayisi()
    {
        try
        {
            var count = await _service.CozulenSikayetSayisi();

            await _logService.LogAsync(new SikayetLog
            {
                KullaniciId = GetUserIdFromClaims(),
                SikayetId = null,
                Tarih = DateTime.UtcNow,
                Aciklama = $"Çözülen şikayet sayısı sorgulandı. Sonuç: {count}"
            });

            return count;
        }
        catch
        {
            return 0;
        }
    }

    [Authorize(Roles = "Admin,BirimAdmin")] // GÜNCELLENDİ
    [HttpGet]
    public async Task<int> BekleyenSikayetSayisi()
    {
        try
        {
            var count = await _service.BekleyenSikayetSayisi();

            await _logService.LogAsync(new SikayetLog
            {
                KullaniciId = GetUserIdFromClaims(),
                SikayetId = null,
                Tarih = DateTime.UtcNow,
                Aciklama = $"Bekleyen şikayet sayısı sorgulandı. Sonuç: {count}"
            });

            return count;
        }
        catch
        {
            return 0;
        }
    }
    #endregion

    #region Şikayet Sil (Admin + BirimAdmin)
    [Authorize(Roles = "Admin,BirimAdmin")]
    [HttpDelete("{id}")]
    public async Task<bool> Delete(int id)
    {
        try
        {
            var beforeDelete = await _service.GetById(id);

            var result = await _service.DeleteSikayet(id);

            if (result)
            {
                await _logService.LogAsync(new SikayetLog
                {
                    KullaniciId = GetUserIdFromClaims(),
                    SikayetId = id,
                    Tarih = DateTime.UtcNow,
                    Aciklama = $"Şikayet silindi. ID={id}, Başlık='{beforeDelete?.Baslik}'"
                });
            }

            return result;
        }
        catch
        {
            return false;
        }
    }
    #endregion

    #region Kullanıcıya Ait Şikayetleri Getir
    [Authorize]
    [HttpGet]
    public async Task<List<SikayetDetayDto>> GetAllByUser([FromQuery] int userId)
    {
        try
        {
            if (userId <= 0)
                return new List<SikayetDetayDto>();

            var complaints = await _service.GetAllByUser(userId);

            await _logService.LogAsync(new SikayetLog
            {
                KullaniciId = GetUserIdFromClaims(),
                SikayetId = null,
                Tarih = DateTime.UtcNow,
                Aciklama = $"Kullanıcı ID={userId} için şikayetler getirildi. Toplam: {complaints?.Count ?? 0}"
            });

            return complaints ?? new List<SikayetDetayDto>();
        }
        catch
        {
            return new List<SikayetDetayDto>();
        }
    }
    #endregion

    private string GetChangesDescription(Sikayet before, Sikayet after)
    {
        if (before == null || after == null)
            return "Önceki veya sonraki şikayet bilgisi bulunamadı.";

        var changes = new List<string>();

        if (before.Baslik != after.Baslik)
            changes.Add($"Başlık: '{before.Baslik}' => '{after.Baslik}'");

        if (before.Aciklama != after.Aciklama)
            changes.Add($"Açıklama değişti.");

        if (before.Durum != after.Durum)
            changes.Add($"Durum: {before.Durum} => {after.Durum}");

        if (before.CozenBirim != after.CozenBirim)
            changes.Add($"Çözen Birim: {before.CozenBirim} => {after.CozenBirim}");

        if (changes.Count == 0)
            return "Değişiklik yok.";

        return string.Join("; ", changes);
    }
}
