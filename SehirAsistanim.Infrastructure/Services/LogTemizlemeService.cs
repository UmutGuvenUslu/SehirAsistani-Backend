using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using SehirAsistanim.Domain.Interfaces;

public class LogTemizlemeService : BackgroundService
{
    private readonly ISikayetLoglariService _logService;
    private readonly ILogger<LogTemizlemeService> _logger;

    public LogTemizlemeService(ISikayetLoglariService logService, ILogger<LogTemizlemeService> logger)
    {
        _logService = logService;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("LogTemizlemeService başlatıldı.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var silinecekTarih = DateTime.UtcNow.AddDays(-30);
                await _logService.DeleteLogsOlderThan(silinecekTarih);
                _logger.LogInformation("Log temizleme işlemi başarılı. {Tarih} öncesindeki loglar silindi.", silinecekTarih);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Log temizleme işlemi sırasında bir hata oluştu.");
            }

            await Task.Delay(TimeSpan.FromHours(24), stoppingToken);
        }

        _logger.LogInformation("LogTemizlemeService durduruldu.");
    }
}
