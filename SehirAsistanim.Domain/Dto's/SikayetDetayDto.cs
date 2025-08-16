using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SehirAsistanim.Domain.Entities;
using SehirAsistanim.Domain.Interfaces;

namespace SehirAsistanim.Domain.Dto_s
{
    public class SikayetDetayDto:IEntitiy
    {
        public int Id { get; set; }
        public string Baslik { get; set; }
        public string Aciklama { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string FotoUrl { get; set; }
        public DateTime GonderilmeTarihi { get; set; }
        public DateTime? CozulmeTarihi { get; set; }
        public string Durum { get; set; }
        public int DogrulanmaSayisi { get; set; }

        public double DuyguPuani { get; set; } = 0.0;
        public bool Silindimi { get; set; }

        // Kullanıcı Bilgileri
        public int KullaniciId { get; set; }
        public string KullaniciAdi { get; set; }
        public string KullaniciEmail { get; set; }

        // Şikayet Türü Bilgileri
        public int SikayetTuruId { get; set; }
        public string SikayetTuruAdi { get; set; }

        // Çözen Birim Bilgileri
        public int? CozenBirimId { get; set; }
        public string? CozenBirimAdi { get; set; }

        public ICollection<SikayetCozum>? SikayetCozumlar { get; set; }
    }

}
