using System.Text.Json;
using System.Text;

public class ProfanityFilterMiddleware
{
    private readonly RequestDelegate _next;
    private readonly List<string> _bannedWords;

    #region Constructor ve Dosya Yükleme
    public ProfanityFilterMiddleware(RequestDelegate next)
    {
        _next = next;

        var currentDirectory = Directory.GetCurrentDirectory();
        var filePath = Path.Combine(currentDirectory, "Middlewares", "kotusoz.txt");

        if (File.Exists(filePath))
        {
            _bannedWords = File.ReadAllLines(filePath)
                .Select(w => w.Trim().ToLower())
                .Where(w => !string.IsNullOrWhiteSpace(w))
                .ToList();
        }
        else
        {
            _bannedWords = new List<string>();
            Console.WriteLine("kotusoz.txt dosyası bulunamadı.");
        }
    }
    #endregion

    #region Request Denetimi ve Filtreleme
    public async Task InvokeAsync(HttpContext context)
    {
        if (context.Request.Method == HttpMethods.Post &&
            context.Request.ContentType != null &&
            context.Request.ContentType.Contains("application/json"))
        {
            context.Request.EnableBuffering();

            using var reader = new StreamReader(
                context.Request.Body,
                encoding: Encoding.UTF8,
                detectEncodingFromByteOrderMarks: false,
                bufferSize: 1024,
                leaveOpen: true);

            var body = await reader.ReadToEndAsync();
            context.Request.Body.Position = 0;

            var json = JsonDocument.Parse(body);

            bool baslikKufurlu = false;
            bool aciklamaKufurlu = false;

            // Başlık kontrolü
            if (json.RootElement.TryGetProperty("Baslik", out var baslik))
            {
                var kelimeler = baslik.ToString()
                    .ToLower()
                    .Split(new[] { ' ', '.', ',', '!', '?', ';', ':', '\r', '\n', '\t', '-', '_' }, StringSplitOptions.RemoveEmptyEntries);

                baslikKufurlu = kelimeler.Any(kelime => _bannedWords.Contains(kelime));
            }

            // Açıklama kontrolü
            if (json.RootElement.TryGetProperty("Aciklama", out var aciklama))
            {
                var kelimeler = aciklama.ToString()
                    .ToLower()
                    .Split(new[] { ' ', '.', ',', '!', '?', ';', ':', '\r', '\n', '\t', '-', '_' }, StringSplitOptions.RemoveEmptyEntries);

                aciklamaKufurlu = kelimeler.Any(kelime => _bannedWords.Contains(kelime));
            }

            if (baslikKufurlu || aciklamaKufurlu)
            {
                context.Response.StatusCode = 400;

                if (baslikKufurlu && aciklamaKufurlu)
                    await context.Response.WriteAsync("Başlık ve açıklama alanlarında uygunsuz kelimeler tespit edildi.");
                else if (baslikKufurlu)
                    await context.Response.WriteAsync("Başlık alanında uygunsuz kelimeler tespit edildi.");
                else
                    await context.Response.WriteAsync("Açıklama alanında uygunsuz kelimeler tespit edildi.");

                return;
            }
        }

        await _next(context);
    }
    #endregion
}
