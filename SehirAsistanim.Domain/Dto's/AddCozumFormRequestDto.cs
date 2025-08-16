using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SehirAsistanim.Domain.Dto_s
{
    public class AddCozumFormRequestDto
    {
        [JsonPropertyName("sikayetid")]
        public int SikayetId { get; set; }

        [JsonPropertyName("cozenkullaniciid")]
        public int CozenKullaniciId { get; set; }

        [JsonPropertyName("cozumaciklamasi")]
        public string Aciklama { get; set; }

        [JsonPropertyName("cozumfotourl")]
        public string? FotoUrl { get; set; }

    }
}
