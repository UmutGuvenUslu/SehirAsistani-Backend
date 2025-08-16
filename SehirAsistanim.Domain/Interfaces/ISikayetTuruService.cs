using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SehirAsistanim.Domain.Entities;

namespace SehirAsistanim.Domain.Interfaces
{
    public interface ISikayetTuruService
    {
        Task<List<SikayetTuru>> GetAll();
        Task<SikayetTuru> GetById(int sikayetTuruId);
        Task<SikayetTuru> AddSikayetTuru(SikayetTuru sikayetTuru);
        Task<SikayetTuru> UpdateSikayetTuru(SikayetTuru sikayetTuru);
        Task<bool> DeleteSikayetTuru(int sikayetTuruId);
        
    }
}
