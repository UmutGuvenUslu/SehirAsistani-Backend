namespace SehirAsistanim.Domain.Dto_s
{
    public class AuthResultDto
    {
        public bool Success { get; set; }
        public string Message { get; set; } = null!;
        public string Token { get; set; } = null!;
        public string? RefreshToken { get; set; } = null!;
        public DateTime ExpireAt { get; set; }

        public int UserId { get; set; }
        public string Email { get; set; } = null!;
        public string FullName { get; set; } = null!;
        public string? TelefonNo { get; set; }
        public string? Cinsiyet { get; set; }
        public DateTime? DogumTarihi { get; set; }
        public string Rol { get; set; } = null!;

    }
}
