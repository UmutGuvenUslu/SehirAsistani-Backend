namespace SehirAsistanim.Domain.Dto_s
{
    public class RegisterDto
    {
        public string Isim { get; set; } = null!;
        public string Soyisim { get; set; } = null!;
        public string TC { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string? TelefonNo { get; set; }
        public string? Cinsiyet { get; set; }
        public DateTime? DogumTarihi { get; set; }
        public string Sifre { get; set; } = null!;
        public string SifreTekrar { get; set; } = null!;
        public string Kod { get; set; }
    }
}
