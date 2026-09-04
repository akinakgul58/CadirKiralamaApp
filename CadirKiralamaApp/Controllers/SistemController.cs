using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using CadirKiralamaApp.Data;
using CadirKiralamaApp.Models;

namespace CadirKiralamaApp.Controllers
{
    public class SistemController : Controller
    {
        private readonly UygulamaDbContext _context;

        public SistemController(UygulamaDbContext context)
        {
            _context = context;
        }

        public IActionResult Ayarlar()
        {
            return View();
        }

        // Hata Bildirimi Kaydet (AJAX)
        [HttpPost]
        public async Task<IActionResult> HataBildir([FromBody] HataBildirimi model)
        {
            if (string.IsNullOrWhiteSpace(model.Baslik) || string.IsNullOrWhiteSpace(model.Aciklama))
            {
                return Json(new { success = false, message = "Lütfen başlık ve açıklama alanlarını doldurun." });
            }

            model.Tarih = DateTime.Now;
            _context.HataBildirimleri.Add(model);
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Hata bildirimi başarıyla iletildi!" });
        }

        // Kayıtlı Hataları Liste Olarak Getir (AJAX)
        [HttpGet]
        public async Task<IActionResult> HataListesiGetir()
        {
            var hatalar = await _context.HataBildirimleri
                .OrderByDescending(h => h.Tarih)
                .Select(h => new
                {
                    h.Id,
                    h.Baslik,
                    h.Aciklama,
                    Tarih = h.Tarih.ToString("dd.MM.yyyy HH:mm")
                })
                .ToListAsync();

            return Json(hatalar);
        }
        [HttpPost]
        public async Task<IActionResult> GeriBildirimKaydet(GeriBildirim model)
        {
            if (ModelState.IsValid)
            {
                _context.GeriBildirimler.Add(model);
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Geri bildiriminiz için teşekkür ederiz!" });
            }
            return Json(new { success = false, message = "Lütfen alanları eksiksiz doldurun." });
        }
    }
}