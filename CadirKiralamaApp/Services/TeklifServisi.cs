using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using CadirKiralamaApp.Data;
using CadirKiralamaApp.Models;
using CadirKiralamaApp.ViewModels;

namespace CadirKiralamaApp.Services
{
    public class TeklifServisi : ITeklifServisi
    {
        private readonly UygulamaDbContext _context;

        public TeklifServisi(UygulamaDbContext context)
        {
            _context = context;
        }

        public async Task<TeklifListesiViewModel> SayfaliTeklifleriGetirAsync(string? aramaKelimesi, TeklifDurumu? durum, int sayfa = 1, int sayfaBoyutu = 5)
        {
            var sorgu = _context.Teklifler.AsNoTracking().AsQueryable();

            // Arama Arayüzü Filtresi
            if (!string.IsNullOrWhiteSpace(aramaKelimesi))
            {
                var kelime = aramaKelimesi.ToLower();
                sorgu = sorgu.Where(t => t.FirmaAdi.ToLower().Contains(kelime) ||
                                         t.MusteriYetkilisi.ToLower().Contains(kelime) ||
                                         t.CadirTuru.ToLower().Contains(kelime) ||
                                         t.Konum.ToLower().Contains(kelime));
            }

            // Durum Filtresi
            if (durum.HasValue)
            {
                sorgu = sorgu.Where(t => t.Durum == durum.Value);
            }

            var toplamOge = await sorgu.CountAsync();

            var teklifler = await sorgu
                .OrderByDescending(t => t.TeklifTarihi)
                .Skip((sayfa - 1) * sayfaBoyutu)
                .Take(sayfaBoyutu)
                .ToListAsync();

            return new TeklifListesiViewModel
            {
                Teklifler = teklifler,
                AramaKelimesi = aramaKelimesi,
                DurumFiltresi = durum,
                MevcutSayfa = sayfa,
                SayfaBoyutu = sayfaBoyutu,
                ToplamOgeSayisi = toplamOge
            };
        }
    }
}