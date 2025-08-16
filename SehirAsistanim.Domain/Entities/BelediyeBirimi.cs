using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SehirAsistanim.Domain.Interfaces;

namespace SehirAsistanim.Domain.Entities
{
    [Table("belediyebirimleri")]
    public class BelediyeBirimi :IEntitiy
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }
        [Column("birimadi")]
        public string BirimAdi { get; set; } = null!;
        [Column("emailadresi")]
        public string? EmailAdresi { get; set; }
    }
}
