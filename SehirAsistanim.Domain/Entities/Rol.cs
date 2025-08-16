using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SehirAsistanim.Domain.Enums;
using SehirAsistanim.Domain.Interfaces;

namespace SehirAsistanim.Domain.Entities
{
    [Table("rolturu")]
    public class Rol:IEntitiy
    {
        public int Id { get; set; }
        [Column("name")]
        public rolturu Tur { get; set; } 
    }
}
