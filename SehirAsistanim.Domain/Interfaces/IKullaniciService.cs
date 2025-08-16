using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SehirAsistanim.Domain.Entities;

namespace SehirAsistanim.Domain.Interfaces
{
    public interface IKullaniciService
    {
        Task<List<Kullanici>> GetAll();
        Task<Kullanici> GetById(int kullaniciId);
        Task<Kullanici> AddKullanici(Kullanici kullanici);
        Task<Kullanici> UpdateKullanici(Kullanici kullanici);
        Task<bool> DeleteKullanici(int kullaniciId);
        //Task<bool> SoftDeleteKullanici(int kullaniciId);
        Task<int> TotalKullaniciSayisi();
    }
}
