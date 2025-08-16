using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SehirAsistanim.Domain.Interfaces
{
    public interface ISikayetDogrulamaService
    {
        Task<bool> IncrementDogrulama(int sikayetId,int kullaniciId);
        Task<bool> SikayetDogrulamaAddKullanici(int sikayetId, int kullaniciId);


    }
}
