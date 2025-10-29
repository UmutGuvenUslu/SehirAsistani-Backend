using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Npgsql;
using SehirAsistanim.Domain.Entities;
using SehirAsistanim.Domain.Interfaces;
using SehirAsistanim.Infrastructure.Services;
using SehirAsistanim.Infrastructure.UnitOfWork;

var builder = WebApplication.CreateBuilder(args);

// 🌐 Port Ayarı (Railway, Heroku vb. için)
var port = Environment.GetEnvironmentVariable("PORT") ?? "8888";
builder.WebHost.UseUrls($"http://*:{port}");

// ✅ HealthChecks
builder.Services.AddHealthChecks();

#region 🔓 CORS (Frontend + Localhost izinli)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins(
            "https://sehir-asistanim-frontend.vercel.app", // Vercel frontend
            "http://localhost:5173" // local geliştirme
        )
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials();
    });
});
#endregion

#region 🛢️ PostgreSQL Connection
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
    options.UseNpgsql(
        connectionString,
        npgsqlOptions =>
        {
            npgsqlOptions.UseNetTopologySuite(); // Harita desteği
        }));
#endregion

#region 💉 Dependency Injection
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
#endregion

builder.Services.AddMemoryCache();
builder.Services.AddControllers();

#region 📘 Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
#endregion

#region 🔐 JWT Authentication
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
#endregion

var app = builder.Build();

#region 🚀 Middleware Pipeline

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

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

// 🩺 Sağlık Kontrolü
app.UseHealthChecks("/health");

// 🧭 Controller yönlendirmeleri
app.MapControllers();

app.Run();

#endregion
