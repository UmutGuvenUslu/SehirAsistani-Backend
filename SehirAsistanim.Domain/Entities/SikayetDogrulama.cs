using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SehirAsistanim.Domain.Interfaces;

namespace SehirAsistanim.Domain.Entities
{
    [Table("sikayetdogrulamalari")]
    public class SikayetDogrulama:IEntitiy
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }
        [Column("sikayetid")]
        public int SikayetId { get; set; }
        [Column("kullaniciid")]
        public int KullaniciId { get; set; }
        [Column("dogrulamatarihi")]
        public DateTime DogrulamaTarihi { get; set; }
    }
}
