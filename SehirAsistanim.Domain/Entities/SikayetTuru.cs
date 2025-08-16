using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using SehirAsistanim.Domain.Interfaces;


namespace SehirAsistanim.Domain.Entities
{
    [Table("sikayetturleri")]
    public class SikayetTuru : IEntitiy
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }
        [Column("ad")]
        public string Ad { get; set; }
        [Column("varsayilanbirimid")]
        public int VarsayilanBirimId { get; set; }
        [Column("icon")]
        public string icon { get; set; }
    }
}
