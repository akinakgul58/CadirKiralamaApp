using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CadirKiralamaApp.Models;
using CadirKiralamaApp.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CadirKiralamaApp.Controllers
{
    public class StokController : Controller
    {
        private readonly UygulamaDbContext _context;

        public StokController(UygulamaDbContext context)
        {
            _context = context;
        }

        // 1. STOK DURUMU SAYFASI
        public async Task<IActionResult> Index()
        {
            var stoklar = await _context.Stoklar.ToListAsync();
            return View(stoklar ?? new List<Stok>());
        }

        // 2. ÜRÜN LİSTESİ (KATALOG) SAYFASI - (Sayfalama, PageSize ve Açıklama Araması Entegre Edildi)
        [HttpGet]
        public async Task<IActionResult> Urunler(string? search, int page = 1, int pageSize = 10)
        {
            var query = _context.Urunler.AsQueryable();

            // Arama filtresi (UrunAdi, UrunKodu, UrunModeli ve Aciklama üzerinden arama yapabilir)
            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(u =>
                    (u.UrunAdi != null && u.UrunAdi.Contains(search)) ||
                    (u.UrunKodu != null && u.UrunKodu.Contains(search)) ||
                    (u.UrunModeli != null && u.UrunModeli.Contains(search)) ||
                    (u.Aciklama != null && u.Aciklama.Contains(search))
                );
            }

            var totalItems = await query.CountAsync();

            // Sayfalama hesabı (Skip / Take)
            var urunler = await query
                .OrderByDescending(u => u.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // View tarafında kullanılacak sayfalama bilgileri
            ViewBag.CurrentPage = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalItems / pageSize);
            ViewBag.Search = search;

            return View(urunler ?? new List<Urun>());
        }

        // 3. REHBER MODAL AJAX ENDPOINT'I
        [HttpGet]
        public async Task<IActionResult> GetUrunRehberListesi()
        {
            var urunler = await _context.Urunler
                .Select(u => new
                {
                    id = u.Id,
                    kod = u.UrunKodu ?? "KOD-YOK",
                    ad = u.UrunAdi,
                    aciklama = u.Aciklama ?? "", // Arayüze aktarılmak üzere eklendi
                    fiyat = u.SatisFiyati,
                    stok = 0,
                    birim = u.Birim ?? "Adet"
                })
                .ToListAsync();

            return Json(urunler);
        }

        // 4. ÜRÜN EKLE/GÜNCELLE
        [HttpPost]
        public async Task<IActionResult> UrunKaydet(Urun model)
        {
            if (ModelState.IsValid)
            {
                if (model.Id == 0)
                    _context.Urunler.Add(model);
                else
                    _context.Urunler.Update(model);

                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Urunler");
        }
    }
}