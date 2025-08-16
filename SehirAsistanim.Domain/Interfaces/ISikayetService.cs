

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SehirAsistanim.Domain.Dto_s;
using SehirAsistanim.Domain.Entities;
using SehirAsistanim.Domain.Enums;

namespace SehirAsistanim.Domain.Interfaces
{
    public interface ISikayetService
    {

        Task<List<SikayetDetayDto>> GetAll();
        Task<Sikayet> GetById(int sikayetId);
        Task<Sikayet> AddSikayet(Sikayet sikayet);

        Task<bool> UpdateDurumAsCozuldu(int sikayetId, sikayetdurumu durum);
        
        Task<int> TotalSikayetSayisi();
        Task<bool> UpdateSikayet(Sikayet guncellenenSikayet);
        Task<bool> DeleteSikayet(int id);
        Task<int> CozulenSikayetSayisi();
        Task<int> BekleyenSikayetSayisi();
        Task<List<SikayetDetayDto>> GetAllByUser(int userId);

    }
}