using CadirKiralamaApp.Data;
using CadirKiralamaApp.Models;
using CadirKiralamaApp.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CadirKiralamaApp.Controllers
{
    public class TeklifController : Controller
    {
        private readonly UygulamaDbContext _context;

        public TeklifController(UygulamaDbContext context)
        {
            _context = context;
        }

        // GET: /Teklif veya /Teklif/Index
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var teklifler = await _context.Teklifler
                .Include(t => t.TeklifKalemleri)
                .Include(t => t.Cari)
                .OrderByDescending(t => t.TeklifTarihi)
                .ToListAsync();

            var viewModel = new TeklifListesiViewModel
            {
                Teklifler = teklifler
            };

            return View(viewModel);
        }

        // GET: /Teklif/Ekle veya /Teklif/Ekle/5
        [HttpGet]
        public async Task<IActionResult> Ekle(int? id)
        {
            // 🎯 1. Carileri VE Ürünleri View için Yüklüyoruz
            await CarileriYukleAsync();
            await UrunleriYukleAsync();

            // 2. DÜZENLEME MODU (ID Varsa)
            if (id.HasValue && id.Value > 0)
            {
                var teklif = await _context.Teklifler
                    .Include(t => t.TeklifKalemleri)
                    .FirstOrDefaultAsync(t => t.Id == id.Value);

                if (teklif == null)
                {
                    return NotFound();
                }

                if (teklif.TeklifKalemleri != null && teklif.TeklifKalemleri.Any())
                {
                    teklif.TeklifKalemleri = teklif.TeklifKalemleri
                        .Where(k => !string.IsNullOrWhiteSpace(k.UrunAdi))
                        .ToList();
                }

                return View(teklif);
            }

            // 3. YENİ EKLEME MODU (ID Yoksa)
            var count = await _context.Teklifler.CountAsync() + 1;
            var yeniTeklif = new Teklif
            {
                TeklifNo = $"TKL-{DateTime.Now:yyyyMM}-{count:D4}",
                TeklifTarihi = DateTime.Now,
                GecerlilikSuresiGun = 30,
                TeklifinTipi = "Kiralama",
                OdemeVadesi = "Peşin",
                BizdenIlgili = "GÜNER SOLMAZ",
                TeklifKalemleri = new List<TeklifKalemi>()
            };

            return View(yeniTeklif);
        }

        // POST: /Teklif/Kaydet
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Kaydet(Teklif teklif)
        {
            if (teklif.TeklifKalemleri != null)
            {
                teklif.TeklifKalemleri = teklif.TeklifKalemleri
                    .Where(k => !string.IsNullOrWhiteSpace(k.UrunAdi))
                    .ToList();

                // 🎯 TOPLAM TUTARLARI SUNUCUDA OTOMATİK HESAPLA VE TEKLİFE YAZ
                decimal araToplam = 0;
                decimal kdvToplam = 0;

                foreach (var k in teklif.TeklifKalemleri)
                {
                    decimal hamTutar = k.Miktar * k.Fiyat;
                    decimal indirimTutari = hamTutar * (k.IndirimOrani / 100m);
                    decimal netTutar = hamTutar - indirimTutari;
                    decimal kdvTutari = netTutar * (k.KdvOrani / 100m);

                    araToplam += netTutar;
                    kdvToplam += kdvTutari;
                }

                teklif.AraToplam = araToplam;
                teklif.KdvToplam = kdvToplam;
                teklif.GenelToplam = araToplam + kdvToplam;
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (teklif.Id == 0)
                    {
                        if (string.IsNullOrWhiteSpace(teklif.TeklifNo))
                        {
                            var count = await _context.Teklifler.CountAsync() + 1;
                            teklif.TeklifNo = $"TKL-{DateTime.Now:yyyyMM}-{count:D4}";
                        }

                        _context.Teklifler.Add(teklif);
                    }
                    else
                    {
                        var mevcutTeklif = await _context.Teklifler
                            .Include(t => t.TeklifKalemleri)
                            .FirstOrDefaultAsync(t => t.Id == teklif.Id);

                        if (mevcutTeklif == null)
                        {
                            return NotFound();
                        }

                        var mevcutDurum = mevcutTeklif.Durum;

                        if (mevcutTeklif.TeklifKalemleri != null && mevcutTeklif.TeklifKalemleri.Any())
                        {
                            _context.RemoveRange(mevcutTeklif.TeklifKalemleri);
                            mevcutTeklif.TeklifKalemleri.Clear();
                        }

                        _context.Entry(mevcutTeklif).CurrentValues.SetValues(teklif);
                        mevcutTeklif.Durum = mevcutDurum;

                        if (teklif.TeklifKalemleri != null && teklif.TeklifKalemleri.Any())
                        {
                            foreach (var kalem in teklif.TeklifKalemleri)
                            {
                                kalem.Id = 0;
                                kalem.TeklifId = teklif.Id;
                                _context.Add(kalem);
                            }
                        }
                    }

                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Teklif başarıyla kaydedildi.";
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Kaydetme hatası: " + ex.Message);
                }
            }

            await CarileriYukleAsync();
            await UrunleriYukleAsync();
            return View("Ekle", teklif);
        }

        // POST: /Teklif/DurumGuncelle
        [HttpPost]
        public async Task<IActionResult> DurumGuncelle(int id, string durum)
        {
            var teklif = await _context.Teklifler.FindAsync(id);

            if (teklif == null)
            {
                return Json(new { success = false, message = "Teklif bulunamadı." });
            }

            if (Enum.TryParse<TeklifDurumu>(durum, true, out var yeniDurum))
            {
                teklif.Durum = yeniDurum;
                await _context.SaveChangesAsync();
                return Json(new { success = true });
            }

            return Json(new { success = false, message = "Geçersiz durum değeri!" });
        }

        // 🚀 GÜNCELLENEN AKILLI ÜRÜN REHBERİ METODU (AJAX İçin)
        [HttpGet]
        [Route("Teklif/GetUrunRehberi")]
        [Route("Teklif/GetUrunlerJson")]
        [Route("Teklif/GetUrunler")]
        public async Task<IActionResult> GetUrunlerJson()
        {
            try
            {
                var rawUrunler = await _context.Urunler.ToListAsync();

                var urunler = rawUrunler.Select(u => new
                {
                    id = u.Id,
                    kod = u.GetType().GetProperty("UrunKodu")?.GetValue(u)?.ToString() ?? u.Id.ToString(),
                    urunAdi = u.UrunAdi ?? "İsimsiz Ürün",
                    aciklama = u.GetType().GetProperty("Aciklama")?.GetValue(u)?.ToString() ?? "", // Açıklama eklendi
                    fiyat = u.GetType().GetProperty("BirimFiyat")?.GetValue(u)
                            ?? u.GetType().GetProperty("Fiyat")?.GetValue(u)
                            ?? 0,
                    en = u.GetType().GetProperty("En")?.GetValue(u) ?? 0,
                    boy = u.GetType().GetProperty("Boy")?.GetValue(u) ?? 0,
                    m2 = u.GetType().GetProperty("M2")?.GetValue(u)
                         ?? u.GetType().GetProperty("Metrekare")?.GetValue(u)
                         ?? 0,
                    kdvOrani = u.GetType().GetProperty("KdvOrani")?.GetValue(u) ?? 20,
                    indirimOrani = 0,
                    birim = u.GetType().GetProperty("Birim")?.GetValue(u)?.ToString() ?? "m²"
                }).ToList();

                return Json(urunler);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // 🛠 YARDIMCI METODLAR
        private async Task CarileriYukleAsync()
        {
            var carilerList = await _context.Cariler
                .OrderBy(c => c.FirmaUnvani)
                .ToListAsync();

            ViewBag.Cariler = new SelectList(carilerList ?? new List<Cari>(), "Id", "FirmaUnvani");
        }

        private async Task UrunleriYukleAsync()
        {
            var urunlerList = await _context.Urunler
                .OrderBy(u => u.UrunAdi)
                .ToListAsync();

            ViewBag.Urunler = urunlerList ?? new List<Urun>();
        }
    }
}