using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SehirAsistanim.Domain.Interfaces;


namespace SehirAsistanim.Domain.Entities
{
    [Table("sikayetloglari")]
    public class SikayetLog:IEntitiy
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }
        [Column("kullaniciid")]
        public int? KullaniciId { get; set; }
        [Column("sikayetid")]
        public int? SikayetId { get; set; }
        [Column("aciklama")]
        public string Aciklama { get; set; }
        [Column("tarih")]
        public DateTime Tarih { get; set; }


    }
}
