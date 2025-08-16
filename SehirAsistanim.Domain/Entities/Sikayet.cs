using SehirAsistanim.Domain.Enums;
using SehirAsistanim.Domain.Interfaces;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace SehirAsistanim.Domain.Entities
{
    [Table("sikayetler")]
    public class Sikayet : IEntitiy
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }
        [Column("kullaniciid")]
        public int KullaniciId { get; set; }
        [Column("baslik")]
        public string Baslik { get; set; }
        [Column("aciklama")]
        public string Aciklama { get; set; }
        [Column("sikayetturuid")]
        public int SikayetTuruId { get; set; }
        [Column("enlem")]
        public double Latitude { get; set; }
        [Column("boylam")]
        public double Longitude { get; set; }
        [Column("fotourl")]
        public string FotoUrl { get; set; }
        [Column("gonderilmetarihi")]
        public DateTime GonderilmeTarihi { get; set; } = DateTime.UtcNow;
        [Column("cozulmetarihi")]
        public DateTime? CozulmeTarihi { get; set; }
        [Column("durum")]
        public sikayetdurumu Durum { get; set; }
        [Column("dogrulanmasayisi")]
        public int DogrulanmaSayisi { get; set; } = 1;
        [Column("duygupuani")]
        public double DuyguPuani { get; set; } = 0.0;
        [Column("silindimi")]
        public bool Silindimi { get; set; } = false;
        [Column("cozenbirimid")]
        public int? CozenBirimId { get; set; }
        [Column("turdogrumu")]
        public bool turdogrumu { get; set; } = true;

        public Kullanici? Kullanici { get; set; }
        public SikayetTuru? SikayetTuru { get; set; }
        public BelediyeBirimi? CozenBirim { get; set; }

        // Burada bire çok ilişki için ICollection kullandık
        public ICollection<SikayetCozum>? SikayetCozumlar { get; set; }
    }
}
