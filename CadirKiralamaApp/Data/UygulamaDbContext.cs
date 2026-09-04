using Microsoft.EntityFrameworkCore;
using System;
using CadirKiralamaApp.Models;

namespace CadirKiralamaApp.Data
{
    public class UygulamaDbContext : DbContext
    {
        public UygulamaDbContext(DbContextOptions<UygulamaDbContext> options) : base(options) { }

        public DbSet<Teklif> Teklifler { get; set; } = null!;
        public DbSet<TeklifKalem> TeklifKalemleri { get; set; } = null!;
        public DbSet<Cari> Cariler { get; set; } = null!;
        public DbSet<Stok> Stoklar { get; set; } = null!;
        public DbSet<HataBildirimi> HataBildirimleri { get; set; } = null!;
        public DbSet<Urun> Urunler { get; set; } = null!;
        public DbSet<GeriBildirim> GeriBildirimler { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Teklif & TeklifKalem İlişki Yapılandırması
            modelBuilder.Entity<Teklif>()
                .HasMany(t => t.Kalemler)
                .WithOne(k => k.Teklif)
                .HasForeignKey(k => k.TeklifId)
                .OnDelete(DeleteBehavior.Cascade);

            // Seed Teklifler
            modelBuilder.Entity<Teklif>().HasData(
                new Teklif { Id = 1, TeklifTarihi = new DateTime(2026, 7, 22), FirmaAdi = "Alp Organizasyon A.Ş.", MusteriYetkilisi = "Ali Yılmaz", CadirTuru = "HTS WZ 10 - Mezuniyet", Konum = "Ankara", Durum = TeklifDurumu.Beklemede },
                new Teklif { Id = 2, TeklifTarihi = new DateTime(2026, 7, 19), FirmaAdi = "Boğaziçi Etkinlik Ltd.", MusteriYetkilisi = "Burcu Demir", CadirTuru = "HTS GZ 20 - Fuar Çadırı", Konum = "İstanbul / Şişli", Durum = TeklifDurumu.KabulEdildi },
                new Teklif { Id = 3, TeklifTarihi = new DateTime(2026, 7, 15), FirmaAdi = "Cevher Lojistik", MusteriYetkilisi = "Canan Şahin", CadirTuru = "Depo Çadırı Kiralama", Konum = "Kocaeli / Gebze", Durum = TeklifDurumu.Reddedildi }
            );

            // Seed Cariler
            modelBuilder.Entity<Cari>().HasData(
                new Cari { Id = 1, FirmaUnvani = "Alp Organizasyon A.Ş.", YetkiliKisi = "Ali Yılmaz", Telefon = "0532 000 11 22", Eposta = "ali@alp.com", Sehir = "Ankara" },
                new Cari { Id = 2, FirmaUnvani = "Boğaziçi Etkinlik Ltd.", YetkiliKisi = "Burcu Demir", Telefon = "0533 111 22 33", Eposta = "burcu@bogazici.com", Sehir = "İstanbul" }
            );

            // Seed Stoklar
            modelBuilder.Entity<Stok>().HasData(
                new Stok { Id = 1, UrunKodu = "HTS-WZ10", UrunAdi = "HTS WZ 10m Etkinlik Çadırı", ToplamAdet = 15, SahadakiAdet = 8 },
                new Stok { Id = 2, UrunKodu = "HTS-GZ20", UrunAdi = "HTS GZ 20m Fuar Çadırı", ToplamAdet = 10, SahadakiAdet = 5 },
                new Stok { Id = 3, UrunKodu = "DP-30", UrunAdi = "30m Endüstriyel Depo Çadırı", ToplamAdet = 6, SahadakiAdet = 4 }
            );
        }
    }
}