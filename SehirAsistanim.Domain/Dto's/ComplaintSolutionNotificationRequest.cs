using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SehirAsistanim.Domain.Dto_s
{
    public class ComplaintSolutionNotificationRequest
    {

        [Required(ErrorMessage = "Kullanıcı ID zorunludur")]
        public int KullaniciId { get; set; }

        [Required(ErrorMessage = "Şikayet ID zorunludur")]
        public int SikayetId { get; set; }
    }
}
