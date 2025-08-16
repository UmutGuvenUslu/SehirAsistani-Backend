using Microsoft.EntityFrameworkCore;
using SehirAsistanim.Domain.Dto_s;
using SehirAsistanim.Domain.Entities;
using SehirAsistanim.Domain.Enums;
using SehirAsistanim.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SehirAsistanim.Infrastructure.Services
{
    public class SikayetCozumService : ISikayetCozumService
    {
        private readonly IUnitOfWork _unitOfWork;

        public SikayetCozumService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        // Normalize fonksiyonu: küçük harf yapar, boşlukları ve Türkçe karakterleri sadeleştirir
        private static string Normalize(string text)
        {
            return string.Concat(text
                .ToLowerInvariant()
                .Replace("ı", "i")
                .Replace("ç", "c")
                .Replace("ş", "s")
                .Replace("ö", "o")
                .Replace("ü", "u")
                .Replace("ğ", "g")
                .Where(c => !char.IsWhiteSpace(c)));
        }

        public async Task<List<SikayetDetayDto>> GetSikayetlerForBirimAsync(string birimAdi)
        {
            var normalizedInput = Normalize(birimAdi ?? "");

            var query = _unitOfWork.Repository<Sikayet>()
                  .GetQueryable()
                  .Include(s => s.Kullanici)
                  .Include(s => s.SikayetTuru)
                  .Include(s => s.CozenBirim)
                  .Include(s => s.SikayetCozumlar)
                  .AsEnumerable()  // Bellekte filtreleme için veriyi çekiyoruz
                  .Where(s =>
                      string.IsNullOrEmpty(normalizedInput) ||
                      Normalize(s.SikayetTuru.Ad).Contains(normalizedInput)
                  );

            var list = query.Select(s => new SikayetDetayDto
            {
                Id = s.Id,
                Baslik = s.Baslik,
                Aciklama = s.Aciklama,
                Latitude = s.Latitude,
                Longitude = s.Longitude,
                FotoUrl = s.FotoUrl,
                GonderilmeTarihi = s.GonderilmeTarihi,
                CozulmeTarihi = s.CozulmeTarihi,
                Durum = s.Durum.ToString(),
                DogrulanmaSayisi = s.DogrulanmaSayisi,
                Silindimi = s.Silindimi,
                DuyguPuani = s.DuyguPuani,

                KullaniciId = s.KullaniciId,
                KullaniciAdi = s.Kullanici.Isim + " " + s.Kullanici.Soyisim,
                KullaniciEmail = s.Kullanici.Email,

                SikayetTuruId = s.SikayetTuruId,
                SikayetTuruAdi = s.SikayetTuru.Ad,

                CozenBirimId = s.CozenBirimId,
                CozenBirimAdi = s.CozenBirim != null ? s.CozenBirim.BirimAdi : null,

                SikayetCozumlar = s.SikayetCozumlar.Select(c => new SikayetCozum
                {
                    Id = c.Id,
                    SikayetId = c.SikayetId,
                    CozenKullaniciId = c.CozenKullaniciId,
                    CozumAciklamasi = c.CozumAciklamasi,
                    CozumFotoUrl = c.CozumFotoUrl,
                    CozenKullanici = c.CozenKullanici
                }).ToList()

            }).ToList();

            return list;
        }

        public async Task<bool> AddCozumFormAsync(int sikayetId, int cozenKullaniciId, string aciklama, string? fotoUrl)
        {
            var sikayet = await _unitOfWork.Repository<Sikayet>().GetById(sikayetId);
            if (sikayet == null)
                return false;

            // Eğer zaten çözüm varsa false dönebiliriz
            var existingSolution =  _unitOfWork.Repository<SikayetCozum>().GetAll().Result.ToList().
                Find(c => c.SikayetId == sikayetId);
            if (existingSolution != null)
                return false;

            var newSolution = new SikayetCozum
            {
                SikayetId = sikayetId,
                CozenKullaniciId = cozenKullaniciId,
                CozumAciklamasi = aciklama,
                CozumFotoUrl = fotoUrl,
                CozumeTarihi = DateTime.UtcNow
            };

            await _unitOfWork.Repository<SikayetCozum>().Add(newSolution);

            // İstersen şikayetin durumunu güncelle
            
            sikayet.CozulmeTarihi = DateTime.UtcNow;

            var degisecek = _unitOfWork.Repository<Sikayet>().GetById(sikayetId).Result;

            degisecek.CozenBirimId = cozenKullaniciId;
            degisecek.CozulmeTarihi = DateTime.UtcNow;
             degisecek.SikayetCozumlar.Add(newSolution);

            await _unitOfWork.CommitAsync();

            return true;
        }

        public async Task<bool> SetSikayetTurDogruMuAsync(int sikayetId, bool dogruMu)
        {
            var sikayet = await _unitOfWork.Repository<Sikayet>().GetById(sikayetId);
            if (sikayet == null)
                return false;

            sikayet.turdogrumu = dogruMu;
            await _unitOfWork.Repository<Sikayet>().Update(sikayet);
            await _unitOfWork.CommitAsync();
            return true;
        }
    }
}
