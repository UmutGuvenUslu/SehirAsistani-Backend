using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using NetTopologySuite.Geometries;
using SehirAsistanim.Domain.Entities;
using SehirAsistanim.Domain.Enums;

public class SehirAsistaniDbContext : DbContext
{
    public SehirAsistaniDbContext(DbContextOptions<SehirAsistaniDbContext> options)
        : base(options)
    {
    }

    #region DbSets
    public DbSet<Kullanici> Kullanicilar { get; set; }
    public DbSet<BelediyeBirimi> BelediyeBirimleri { get; set; }
    public DbSet<SikayetTuru> SikayetTurleri { get; set; }
    public DbSet<Sikayet> Sikayetler { get; set; }
    public DbSet<SikayetDogrulama> SikayetDogrulamalari { get; set; }
    public DbSet<SikayetLog> SikayetLoglari { get; set; }
    public DbSet<SikayetCozum> SikayetCozumleri { get; set; }
    public DbSet<Bildirim> Bildirimler { get; set; }
    public DbSet<Rol> Roller { get; set; }
    #endregion

    #region OnModelCreating
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Enum converters
        var rolConverter = new EnumToStringConverter<rolturu>();
        var durumConverter = new EnumToStringConverter<sikayetdurumu>();

        modelBuilder.Entity<Kullanici>()
            .Property(e => e.Rol)
            .HasConversion(rolConverter);

        modelBuilder.Entity<Sikayet>()
            .Property(e => e.Durum)
            .HasConversion(durumConverter);

        // Sikayet tablosundaki datetime alanları timestamptz olarak ayarlama
        modelBuilder.Entity<Sikayet>(entity =>
        {
            entity.Property(e => e.GonderilmeTarihi)
                .HasColumnType("timestamp with time zone");

            entity.Property(e => e.CozulmeTarihi)
                .HasColumnType("timestamp with time zone");
        });

        // SikayetDogrulama tablosundaki datetime alanları için
        modelBuilder.Entity<SikayetDogrulama>(entity =>
        {
            entity.Property(e => e.DogrulamaTarihi)
                .HasColumnType("timestamp with time zone");
        });

        // İLİŞKİYİ BİRE ÇOK OLARAK TANIMLA
        modelBuilder.Entity<SikayetCozum>()
            .HasOne(sc => sc.Sikayet)
            .WithMany(s => s.SikayetCozumlar)
            .HasForeignKey(sc => sc.SikayetId)
            .OnDelete(DeleteBehavior.Cascade);  // istersen silebilirsin

        modelBuilder.Entity<Rol>(entity =>
        {
            entity.ToTable("roller");
            entity.Property(e => e.Tur).HasColumnType("rolturu");
        });
    }


    #endregion
}
