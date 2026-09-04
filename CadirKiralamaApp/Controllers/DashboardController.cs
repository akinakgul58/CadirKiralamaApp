using CadirKiralamaApp.Data;
using CadirKiralamaApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace CadirKiralamaApp.Controllers
{
    public class DashboardController : Controller
    {
        private readonly UygulamaDbContext _context;

        public DashboardController(UygulamaDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // Özet Rakamlar (Kartlar için)
            ViewBag.ToplamTeklif = await _context.Teklifler.CountAsync();
            ViewBag.BekleyenTeklifler = await _context.Teklifler.CountAsync(t => t.Durum == TeklifDurumu.Beklemede);
            ViewBag.OnaylananTeklifler = await _context.Teklifler.CountAsync(t => t.Durum == TeklifDurumu.KabulEdildi);
            ViewBag.CariSayisi = await _context.Cariler.CountAsync();

            // Son Oluşturulan Teklifler (Cari ve TeklifKalemleri dâhil ediliyor)
            var sonTeklifler = await _context.Teklifler
                .Include(t => t.Cari)
                .Include(t => t.TeklifKalemleri) // 🎯 Kalemler çekilerek tutarların tam hesaplanması sağlandı
                .OrderByDescending(t => t.TeklifTarihi)
                .Take(10)
                .ToListAsync();

            return View(sonTeklifler);
        }
    }
}