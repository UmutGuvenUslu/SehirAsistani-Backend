using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SehirAsistanim.Domain.Interfaces;

namespace SehirAsistanim.Domain.Entities
{
    [Table("sikayetcozumleri")]
    public class SikayetCozum : IEntitiy
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("sikayetid")]
        public int SikayetId { get; set; }

        [Column("cozenkullaniciid")]
        public int CozenKullaniciId { get; set; }

        [Column("cozumaciklamasi")]
        public string? CozumAciklamasi { get; set; }

        [Column("cozumfotourl")]
        public string? CozumFotoUrl { get; set; }

        [Column("cozumetarihi")]
        public DateTime CozumeTarihi { get; set; }

        [ForeignKey(nameof(SikayetId))]
        public Sikayet? Sikayet { get; set; }

        [ForeignKey(nameof(CozenKullaniciId))]
        public Kullanici? CozenKullanici { get; set; }
    }
}
