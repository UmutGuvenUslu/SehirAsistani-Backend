using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SehirAsistanim.Domain.Entities;
using SehirAsistanim.Domain.Interfaces;
using System;
using System.Threading.Tasks;

namespace SehirAsistani.Api.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class SikayetDogrulamaController : ControllerBase
    {
        private readonly ISikayetDogrulamaService _service;
        private readonly ISikayetLoglariService _logService;

        public SikayetDogrulamaController(ISikayetDogrulamaService service, ISikayetLoglariService logService)
        {
            _service = service;
            _logService = logService;
        }

        #region Doğrulama Sayısı Arttır
        [Authorize]
        [HttpPut]
        public async Task<bool> IncrementDogrulama(int sikayetId, int kullanciId)
        {
            try
            {
                bool result = await _service.IncrementDogrulama(sikayetId, kullanciId);

                if (result)
                {
                    await _logService.LogAsync(new SikayetLog
                    {
                        SikayetId = sikayetId,
                        KullaniciId = kullanciId,
                        Tarih = DateTime.UtcNow,
                        Aciklama = $"Kullanıcı #{kullanciId}, şikayet #{sikayetId} için doğrulama sayısını artırdı."
                    });
                }

                return result;
            }
            catch (Exception ex)
            {
                await _logService.LogAsync(new SikayetLog
                {
                    SikayetId = sikayetId,
                    KullaniciId = kullanciId,
                    Tarih = DateTime.UtcNow,
                    Aciklama = $"Doğrulama artırma işlemi başarısız oldu. Kullanıcı: #{kullanciId}, Şikayet: #{sikayetId}, Hata: {ex.Message}"
                });

                return false;
            }
        }
        #endregion
    }
}
