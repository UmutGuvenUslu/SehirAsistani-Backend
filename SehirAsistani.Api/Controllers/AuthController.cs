using Microsoft.AspNetCore.Mvc;
using SehirAsistanim.Domain.Dto_s;
using SehirAsistanim.Domain.Interfaces;
using SehirAsistanim.Infrastructure.Services;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly EmailService _emailService;
    private readonly IAuthService _authService;

    public AuthController(EmailService emailService, IAuthService authService)
    {
        _emailService = emailService;
        _authService = authService;
    }

    #region Doğrulama Kodu Gönderme
    [HttpPost("send-verification-code")]
    public async Task<IActionResult> SendVerificationCode([FromQuery] string email)
    {
        await _emailService.SendVerificationCode(email);
        return Ok(new { message = "Doğrulama kodu gönderildi." });
    }
    #endregion

    #region Doğrulama ve Kayıt
    [HttpPost("verify-and-register")]
    public async Task<IActionResult> VerifyAndRegister([FromBody] RegisterDto dto)
    {
        try
        {
            if (!_emailService.IsVerified(dto.Email, dto.Kod))
            {
                return BadRequest(new { message = "Doğrulama kodu geçersiz veya süresi dolmuş." });
            }

            var result = await _authService.RegisterAsync(dto);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Sunucu hatası: " + ex.Message });
        }
    }
    #endregion

    #region Giriş
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        try
        {
            var result = await _authService.LoginAsync(dto);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
    #endregion

    #region Kullanıcı Email Var Mı
    [HttpGet("IsEmailRegistered")]
    public async Task<IActionResult> IsEmailRegistered([FromQuery] string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return BadRequest("Email boş olamaz.");

        var result = await _authService.IsEmailRegistered(email);
        return Ok(new { emailVar = result });
    }
    #endregion

    #region Kullanıcı TC Var Mı
    [HttpGet("tc-analiz")]
    public async Task<bool> TcAnaliz([FromQuery] string tc)
    {
        return await _authService.TcAnaliz(tc);
    }
    #endregion
}
