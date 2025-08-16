namespace SehirAsistanim.Domain.Interfaces
{
    public interface ISmtpService
    {
        Task Gonder(string toEmail, string subject, string htmlBody);
    }
}
