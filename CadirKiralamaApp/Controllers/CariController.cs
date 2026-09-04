using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using CadirKiralamaApp.Data;
using CadirKiralamaApp.Models;

namespace CadirKiralamaApp.Controllers
{
    public class CariController : Controller
    {
        private readonly UygulamaDbContext _context;

        public CariController(UygulamaDbContext context)
        {
            _context = context;
        }

        // GET: /Cari/Index
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var cariler = await _context.Cariler.ToListAsync();
            return View(cariler);
        }

        // GET: /Cari/Ekle veya /Cari/Ekle/3
        [HttpGet]
        public async Task<IActionResult> Ekle(int? id)
        {
            // Eğer id yoksa yeni kayıt sayfası aç (Boş model)
            if (id == null || id == 0)
            {
                return View(new Cari());
            }

            // id varsa veritabanından bul ve formun içini doldur
            var cari = await _context.Cariler.FindAsync(id);
            if (cari == null)
            {
                return RedirectToAction(nameof(Index));
            }

            return View(cari);
        }

        // POST: /Cari/Ekle (Hem Ekleme Hem Güncelleme Yapar)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Ekle(Cari cari)
        {
            // Eğer Id 0 ise yepyeni bir kayıttır, "Add" yap.
            if (cari.Id == 0)
            {
                _context.Cariler.Add(cari);
            }
            // Eğer Id 0'dan büyükse zaten olan bir kayıttır, "Update" yap.
            else
            {
                _context.Cariler.Update(cari);
            }

            // Değişiklikleri kaydet
            await _context.SaveChangesAsync();

            // İşlem bitince listeye geri dön
            return RedirectToAction(nameof(Index));
        }

    } // Controller Sınıfının (Class) Kapanış Parantezi
} // Namespace Kapanış Parantezi