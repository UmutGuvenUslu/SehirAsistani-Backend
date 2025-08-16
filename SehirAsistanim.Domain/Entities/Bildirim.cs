using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SehirAsistanim.Domain.Interfaces;

namespace SehirAsistanim.Domain.Entities
{
    [Table("bildirim")]
    public class Bildirim : IEntitiy
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }
        [Column("kullaniid")]
        public int KullaniciId { get; set; }
        [Column("baslik")]
        public string Baslik { get; set; }
        [Column("mesaj")]
        public string Mesaj { get; set; }
        [Column("gonderimtarihi")]
        public DateTime GonderimTarihi { get; set; }
        [Column("okundumu")]
        public bool OkunduMu { get; set; } = false;
    }
}
