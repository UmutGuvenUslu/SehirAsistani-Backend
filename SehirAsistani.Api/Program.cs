using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Npgsql;
using SehirAsistanim.Domain.Entities;
using SehirAsistanim.Domain.Interfaces;
using SehirAsistanim.Infrastructure.Services;
using SehirAsistanim.Infrastructure.UnitOfWork;

var builder = WebApplication.CreateBuilder(args);

// 🌐 Port Ayarı (Railway, Render, Heroku vb. için)
var port = Environment.GetEnvironmentVariable("PORT") ?? "8888";
builder.WebHost.UseUrls($"http://*:{port}");

// ✅ HealthChecks
builder.Services.AddHealthChecks();

// 🌍 CORS Ayarları
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins(
            "https://sehir-asistanim-frontend.vercel.app",
            "http://localhost:5173"
        )
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials();
    });
});

// 🛢️ PostgreSQL Connection
string? databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL");
string connectionString;

if (!string.IsNullOrEmpty(databaseUrl))
{
    var databaseUri = new Uri(databaseUrl);
    var userInfo = databaseUri.UserInfo.Split(':');

    var npgsqlBuilder = new NpgsqlConnectionStringBuilder
    {
        Host = databaseUri.Host,
        Port = databaseUri.Port,
        Username = userInfo[0],
        Password = userInfo[1],
        Database = databaseUri.AbsolutePath.TrimStart('/'),
        SslMode = SslMode.Require,
        TrustServerCertificate = true,
    };

    connectionString = npgsqlBuilder.ToString();
}
else
{
    connectionString = builder.Configuration.GetConnectionString("DefaultConnection")!;
}

builder.Services.AddDbContext<SehirAsistaniDbContext>(options =>
    options.UseNpgsql(connectionString, npgsqlOptions =>
    {
        npgsqlOptions.UseNetTopologySuite();
        npgsqlOptions.EnableRetryOnFailure(); // 💡 Railway başlatma gecikmesi için
    })
);

// 💉 Dependency Injection
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IKullaniciService, KullaniciService>();
builder.Services.AddScoped<ISikayetTuruService, SikayetTuruService>();
builder.Services.AddScoped<IBelediyeBirimiService, BelediyeBirimiService>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.Configure<SmtpSettings>(builder.Configuration.GetSection("Smtp"));
builder.Services.AddScoped<ISmtpService, SmtpService>();
builder.Services.AddScoped<IDuyguAnaliz, DuyguAnalizService>();
builder.Services.AddScoped<ISikayetService, SikayetService>();
builder.Services.AddScoped<ISikayetDogrulamaService, SikayetDogrulamaService>();
builder.Services.AddScoped<ISikayetLoglariService, SikayetLoglariService>();
builder.Services.AddScoped<ISikayetCozumService, SikayetCozumService>();
builder.Services.AddScoped<IRolService, RolService>();
builder.Services.AddScoped<JwtService>();
builder.Services.AddScoped<EmailService>();
builder.Services.AddHostedService<LogTemizlemeService>();

builder.Services.AddMemoryCache();
builder.Services.AddControllers();

// 📘 Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 🔐 JWT Authentication
builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
    {
        var jwtSettings = builder.Configuration.GetSection("JwtSettings");
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtSettings["Key"]!)
            )
        };
    });

var app = builder.Build();

// 🚀 Middleware Pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// ⚠️ HTTPS yönlendirmesini kaldırdık — Railway içi HTTPS yok
// app.UseHttpsRedirection();

app.UseRouting();

// ⚙️ Preflight (OPTIONS) istekleri için hızlı 200 cevabı
app.Use(async (context, next) =>
{
    if (context.Request.Method == "OPTIONS")
    {
        context.Response.StatusCode = 200;
        await context.Response.CompleteAsync();
        return;
    }
    await next();
});

// 🌐 CORS aktif
app.UseCors("AllowFrontend");

app.UseAuthentication();
app.UseAuthorization();

// 🚫 Küfür Filtresi
app.UseMiddleware<ProfanityFilterMiddleware>();

// 🩺 Sağlık Kontrolü — Railway bu endpoint’e GET isteği atıyor
app.MapGet("/health", () => Results.Ok("OK"));

// 🧭 Controller yönlendirmeleri
app.MapControllers();

// 🪄 Log: Konsolda hangi portta dinlediğini görelim
Console.WriteLine($"✅ Server is running on port {port}");

app.Run();
