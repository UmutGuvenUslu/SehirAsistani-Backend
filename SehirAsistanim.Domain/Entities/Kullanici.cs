using SehirAsistanim.Domain.Enums;
using SehirAsistanim.Domain.Interfaces;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SehirAsistanim.Domain.Entities
{
    [Table("kullanicilar")]
    public class Kullanici : IEntitiy
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("isim")]
        public string Isim { get; set; } = null!;

        [Column("soyisim")]
        public string Soyisim { get; set; } = null!;

        [Column("tc")]
        public string TC { get; set; } = null!;

        [Column("email")]
        public string Email { get; set; } = null!;

        [Column("telefonno")]
        public string? TelefonNo { get; set; }

        [Column("cinsiyet")]
        public string? Cinsiyet { get; set; }

        [Column("dogumtarihi")]
        public DateTime? DogumTarihi { get; set; }

        [Column("kayittarihi")]
        public DateTime KayitTarihi { get; set; }

        [Column("sifre")]
        public string Sifre { get; set; } = null!;

        [Column("rol")]
        public rolturu Rol { get; set; }
    }
}
