using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SehirAsistanim.Domain.Interfaces;

namespace SehirAsistanim.Infrastructure.Services
{
    public class DuyguAnalizService:IDuyguAnaliz
    {
        #region Sözlük
        private static readonly Dictionary<string, int> Sozluk = new()
{
    // === Olumlu Kelimeler (+1 ila +3 arası) ===
    { "iyi", 1 },
    { "güzel", 2 },
    { "temiz", 3 },
    { "bakımlı", 2 },
    { "hızlı", 2 },
    { "çalışkan", 1 },
    { "düzenli", 2 },
    { "hizmet", 1 },
    { "memnun", 2 },
    { "güler yüzlü", 2 },
    { "yardımcı", 2 },
    { "profesyonel", 3 },
    { "etkin", 2 },
    { "kapsamlı", 1 },
    { "çözüm odaklı", 3 },
    { "açık", 1 },
    { "kullanışlı", 1 },
    { "güvenli", 2 },
    { "ulaşılabilir", 1 },
    { "etkili", 2 },
    { "iyi organize", 2 },
    { "destek", 2 },
    { "teşekkür", 3 },
    { "takdir", 3 },
    { "halk dostu", 3 },
    { "sürdürülebilir", 2 },
    { "bilgilendirici", 1 },
    { "yeni", 1 },
    { "gelişmiş", 2 },

    // === Olumsuz Kelimeler (-1 ila -4 arası) ===
    { "kötü", -3 },
    { "berbat", -4 },
    { "rezalet", -4 },
    { "pis", -3 },
    { "kirli", -3 },
    { "temiz değil", -3 },
    { "çöplük", -4 },
    { "çaresiz", -3 },
    { "gecikme", -3 },
    { "ilgisiz", -3 },
    { "yetersiz", -3 },
    { "şikayet", -2 },
    { "bozuk", -3 },
    { "arızalı", -3 },
    { "kırık", -3 },
    { "çamur", -3 },
    { "su yok", -3 },
    { "arıza", -3 },
    { "sorun", -3 },
    { "problem", -3 },
    { "çözülmedi", -3 },
    { "mağdur", -3 },
    { "aldatıldım", -4 },
    { "kandırıldım", -4 },
    { "sokaklar kötü", -4 },
    { "kaldırım bozuk", -3 },
    { "yol bozuk", -3 },
    { "kazık", -4 },
    { "yağmur suyu birikiyor", -3 },
    { "tıkalı", -3 },
    { "pis kokuyor", -3 },
    { "kötü koku", -3 },
    { "gürültü", -3 },
    { "rahatsızlık", -2 },
    { "taciz", -4 },
    { "güvensiz", -3 },
    { "trafik sıkışıklığı", -3 },
    { "kaza", -3 },
    { "taşan", -3 },
    { "çamurlu", -3 },
    { "yol kapalı", -3 },
    { "bakımsız", -3 },
    { "kalabalık", -2 },
    { "karışık", -2 },
    { "zam", -2 },
    { "eksik", -2 },
    { "fazla", -2 },
    { "yavaş", -2 },
    { "beklemek zorunda kaldım", -2 },
    { "cevap yok", -3 },
    { "ilgilenmiyorlar", -3 },
    { "boş vaat", -3 },
    { "kötü hizmet", -3 },
    { "mağduriyet", -3 },
    { "çözüm yok", -3 },
    { "kötü davranış", -3 },
    { "yanlış", -2 },
    { "eksiklik", -2 },
    { "hatalı", -3 },
    { "şikayetçi", -2 },
    { "utanç verici", -4 },
    { "korkunç", -4 },
    { "tehlikeli", -3 },
    { "düzensiz", -3 },
    { "rahatsız edici", -3 },
    { "engellenme", -3 },
    { "sorunlu", -3 },
    { "boşa zaman kaybı", -4 },
    { "yetersiz ekip", -3 },
    { "ihmal", -4 },
    { "geç müdahale", -4 },
    { "çağrıya cevap yok", -4 },
    { "kötü iletişim", -3 },
    { "şikayetler dikkate alınmıyor", -4 },
    { "ilgilenilmiyor", -4 },
    { "saçma", -3 }
};
        #endregion

        #region Duygu Puanı Hesaplama
        public async Task<int> HesaplaDuyguPuani(string metin)
        {
            int puan = 0;
            foreach (var kelime in Sozluk)
                if (metin.Contains(kelime.Key, StringComparison.OrdinalIgnoreCase))
                    puan += kelime.Value;

            return  puan;
        }
        #endregion


    }
}
