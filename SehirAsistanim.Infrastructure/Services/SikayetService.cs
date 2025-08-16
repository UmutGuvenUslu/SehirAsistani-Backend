using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using SehirAsistanim.Domain.Dto_s;
using SehirAsistanim.Domain.Entities;
using SehirAsistanim.Domain.Enums;
using SehirAsistanim.Domain.Interfaces;
using SehirAsistanim.Infrastructure.UnitOfWork;

namespace SehirAsistanim.Infrastructure.Services
{
    public class SikayetService : ISikayetService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IDuyguAnaliz _duyguAnalizService;

        public SikayetService(IUnitOfWork unitOfWork, IDuyguAnaliz duyguAnaliz)
        {
            _unitOfWork = unitOfWork;
            _duyguAnalizService = duyguAnaliz;
        }

        #region GetAll
        public async Task<List<SikayetDetayDto>> GetAll()
        {
            var query = _unitOfWork.Repository<Sikayet>()
                .GetQueryable()
                .Include(s => s.Kullanici)
                .Include(s => s.SikayetTuru)
                .Include(s => s.CozenBirim)
                .Include(s => s.SikayetCozumlar);

            var list = await query.Select(s => new SikayetDetayDto
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



            }).ToListAsync();

            return list;
        }
        #endregion

        #region GetById
        public async Task<Sikayet> GetById(int sikayetId)
        {
            return await _unitOfWork.Repository<Sikayet>().GetById(sikayetId);
        }
        #endregion

        #region AddSikayet
        public async Task<Sikayet> AddSikayet(Sikayet sikayet)
        {
            sikayet.DuyguPuani = await _duyguAnalizService.HesaplaDuyguPuani(sikayet.Aciklama);
            await _unitOfWork.Repository<Sikayet>().Add(sikayet);
            await _unitOfWork.CommitAsync();
            return sikayet;
        }
        #endregion

        #region UpdateDurumAsCozuldu
        public async Task<bool> UpdateDurumAsCozuldu(int sikayetId, sikayetdurumu durum)
        {
            try
            {


                var sikayet = await _unitOfWork.Repository<Sikayet>().GetById(sikayetId);
                if (sikayet == null) return false;

                sikayet.Durum = durum;
                if (durum == sikayetdurumu.Cozuldu)
                {
                    sikayet.CozulmeTarihi = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc);

                }
                else
                {
                    sikayet.CozulmeTarihi = null;

                }

                await _unitOfWork.Repository<Sikayet>().Update(sikayet);
                await _unitOfWork.CommitAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }
        #endregion

        #region SoftDelete (İstendiğinde açılabilir)
        //public async Task<bool> SoftDelete(int sikayetId)
        //{
        //    var sikayet = await _unitOfWork.Repository<Sikayet>().GetById(sikayetId);
        //    if (sikayet == null) return false;

        //    sikayet.Silindimi = true;

        //    await _unitOfWork.Repository<Sikayet>().Update(sikayet);
        //    await _unitOfWork.Commit();

        //    return true;
        //}
        #endregion

        #region TotalSikayet
        public async Task<int> TotalSikayetSayisi()
        {
            var allComplaint = await _unitOfWork.Repository<Sikayet>().GetAll();
            return allComplaint.Count();
        }
        #endregion

        #region UpdateSikayet
        public async Task<bool> UpdateSikayet(Sikayet updated)
        {
            var updateSikayet = _unitOfWork.Repository<Sikayet>().GetById(updated.Id).Result;
            if (updateSikayet == null) return false;

            updateSikayet.Baslik = updated.Baslik;
            updateSikayet.Aciklama = updated.Aciklama;
            updateSikayet.FotoUrl = updated.FotoUrl;
            updateSikayet.Latitude = updated.Latitude;
            updateSikayet.Longitude = updated.Longitude;
            updateSikayet.SikayetTuruId = updated.SikayetTuruId;
            updateSikayet.GonderilmeTarihi = updated.GonderilmeTarihi;

            await _unitOfWork.Repository<Sikayet>().Update(updateSikayet);
            await _unitOfWork.CommitAsync();
            return true;
        }
        #endregion

        #region DeleteSikayet
        public async Task<bool> DeleteSikayet(int sikayetId)
        {
            var sikayet = await _unitOfWork.Repository<Sikayet>().GetById(sikayetId);
            if (sikayet == null) return false;

            await _unitOfWork.Repository<Sikayet>().Delete(sikayet.Id);
            await _unitOfWork.CommitAsync();
            return true;
        }
        #endregion

        #region CozulenSikayetler
        public async Task<int> CozulenSikayetSayisi()
        {
            var sikayetler = await _unitOfWork.Repository<Sikayet>().GetAll();
            return sikayetler.Count(s => s.Durum == sikayetdurumu.Cozuldu);
        }
        #endregion

        #region BekleyenSikayetler
        public async Task<int> BekleyenSikayetSayisi()
        {
            var sikayetler = await _unitOfWork.Repository<Sikayet>().GetAll();
            return sikayetler.Count(s => s.Durum == sikayetdurumu.Inceleniyor);
        }
        #endregion

        #region KullanıcınınSikayetleri
        public async Task<List<SikayetDetayDto>> GetAllByUser(int userId)
        {
            var query = _unitOfWork.Repository<Sikayet>()
               .GetQueryable()
               .Include(s => s.Kullanici)
               .Include(s => s.SikayetTuru)
               .Include(s => s.CozenBirim);

            var list = await query.Select(s => new SikayetDetayDto
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
                CozenBirimAdi = s.CozenBirim != null ? s.CozenBirim.BirimAdi : null
            }).ToListAsync();
            return list.Where(x => x.KullaniciId == userId).ToList();

        }
        #endregion
    }
}
