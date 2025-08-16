using Microsoft.AspNetCore.Mvc;
using SehirAsistanim.Domain.Interfaces;
using System.Threading.Tasks;
using System.Collections.Generic;
using SehirAsistanim.Domain.Dto_s;

namespace SehirAsistanim.API.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    public class SikayetCozumController : ControllerBase
    {
        private readonly ISikayetCozumService _cozumService;

        public SikayetCozumController(ISikayetCozumService cozumService)
        {
            _cozumService = cozumService;
        }

        // 1. Belirli bir birime ait şikayetleri getir
        [HttpGet]
        public async Task<IActionResult> GetSikayetlerForBirim(string roladi)
        {
            var result = await _cozumService.GetSikayetlerForBirimAsync(roladi);
            return Ok(result);
        }

        // 2. Şikayete çözüm formu ekle
        [HttpPost]
        public async Task<IActionResult> AddCozumForm([FromBody] AddCozumFormRequestDto request)
        {
            var result = await _cozumService.AddCozumFormAsync(request.SikayetId, request.CozenKullaniciId, request.Aciklama, request.FotoUrl);
            return Ok("Çözüm başarıyla eklendi.");
        }

        // 3. Tek bir şikayet için tür doğru mu bilgisini güncelle
        [HttpPut]
        public async Task<IActionResult> SetSikayetTurDogruMu(int sikayetId, [FromQuery] bool dogruMu)
        {
            var result = await _cozumService.SetSikayetTurDogruMuAsync(sikayetId, dogruMu);
            if (!result) return NotFound("Şikayet bulunamadı.");
            return Ok("Tür doğruluğu güncellendi.");
        }

       
    }

    
}
