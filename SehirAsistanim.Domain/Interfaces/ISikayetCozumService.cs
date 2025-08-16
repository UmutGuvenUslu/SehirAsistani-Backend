using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SehirAsistanim.Domain.Dto_s;
using SehirAsistanim.Domain.Entities;

namespace SehirAsistanim.Domain.Interfaces
{
    public interface ISikayetCozumService
    {
        Task<List<SikayetDetayDto>> GetSikayetlerForBirimAsync(string birimAdi);
        Task<bool> AddCozumFormAsync(int sikayetId, int cozenKullaniciId, string aciklama, string? fotoUrl);
        Task<bool> SetSikayetTurDogruMuAsync(int sikayetId, bool dogruMu);


    }
}
