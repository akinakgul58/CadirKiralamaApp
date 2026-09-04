using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace CadirKiralamaApp.Models
{
    public class Teklif
    {
        public int Id { get; set; }

        [Display(Name = "Teklif No")]
        public string? TeklifNo { get; set; }

        public string? BizdenIlgili { get; set; }
        public string? MusteriYetkilisi { get; set; }
        public string? TeklifKonusu { get; set; }
        public string? MontajAdresi { get; set; }
        public string? CadirTuru { get; set; }
        public string? Konum { get; set; }

        public TeklifDurumu Durum { get; set; }

        public DateTime TeklifTarihi { get; set; } = DateTime.Now;

        public int? CariId { get; set; }

        [ValidateNever]
        public virtual Cari? Cari { get; set; }
        public string? FirmaAdi { get; set; }

        public DateTime? OrgBaslangicTarihi { get; set; }

        [Display(Name = "Montaj Tarihi")]
        public DateTime? MontajTarihi { get; set; }

        [Display(Name = "Organizasyon Bitiş Tarihi")]
        public DateTime? OrganizasyonBitisTarihi { get; set; }

        [Display(Name = "Demontaj Tarihi")]
        public DateTime? DemontajTarihi { get; set; }

        public string? TeklifinTipi { get; set; } = "Kiralama";
        public int GecerlilikSuresiGun { get; set; } = 30;
        public string? OdemeVadesi { get; set; } = "Peşin";
        public string? Notlar { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal GenelIndirimOrani { get; set; } = 0;

        [Column(TypeName = "decimal(18,2)")]
        public decimal AraToplam { get; set; } = 0;

        [Column(TypeName = "decimal(18,2)")]
        public decimal KdvToplam { get; set; } = 0;

        private decimal _genelToplam;

        [Column(TypeName = "decimal(18,2)")]
        public decimal GenelToplam
        {
            get
            {
                // 🎯 Veritabanında saklanan değer 0 olsa bile kalemler yüklüyse anlık doğru tutarı hesaplar
                if (_genelToplam == 0 && TeklifKalemleri != null && TeklifKalemleri.Any())
                {
                    return TeklifKalemleri.Sum(k =>
                    {
                        decimal hamTutar = k.Miktar * k.Fiyat;
                        decimal indirimTutari = hamTutar * (k.IndirimOrani / 100m);
                        decimal netTutar = hamTutar - indirimTutari;
                        decimal kdvTutari = netTutar * (k.KdvOrani / 100m);
                        return netTutar + kdvTutari;
                    });
                }
                return _genelToplam;
            }
            set => _genelToplam = value;
        }

        // --- LİSTE ALANLARI ---
        [ValidateNever]
        public List<TeklifKalemi> TeklifKalemleri { get; set; } = new List<TeklifKalemi>();

        [NotMapped]
        public List<TeklifKalemi> Kalemler
        {
            get => TeklifKalemleri;
            set => TeklifKalemleri = value;
        }

        // 🎯 TASARIM UYUMU İÇİN TAKMA ADLAR (ALIAS)
        [NotMapped]
        public string? FirmaIlgilisi
        {
            get => MusteriYetkilisi;
            set => MusteriYetkilisi = value;
        }

        [NotMapped]
        public DateTime? OrgBitisTarihi
        {
            get => OrganizasyonBitisTarihi;
            set => OrganizasyonBitisTarihi = value;
        }

        [NotMapped]
        public string? TeklifMetni
        {
            get => Notlar;
            set => Notlar = value;
        }
    }

    public class TeklifKalemi
    {
        public int Id { get; set; }

        public int TeklifId { get; set; }

        [ForeignKey("TeklifId")]
        [ValidateNever]
        public Teklif? Teklif { get; set; }

        public string? Kod { get; set; }
        public string? UrunAdi { get; set; }
        public string? Aciklama { get; set; }

        public string Birim { get; set; } = "m²";

        [Column(TypeName = "decimal(18,2)")]
        public decimal Miktar { get; set; } = 1;

        [Column(TypeName = "decimal(18,2)")]
        public decimal Fiyat { get; set; } = 0;

        // 🎯 TASARIM UYUMU İÇİN ALIAS
        [NotMapped]
        public decimal BirimFiyat
        {
            get => Fiyat;
            set => Fiyat = value;
        }

        public bool KdvDahil { get; set; } = false;

        [Column(TypeName = "decimal(18,2)")]
        public decimal KdvOrani { get; set; } = 20;

        [Column(TypeName = "decimal(18,2)")]
        public decimal IndirimOrani { get; set; } = 0;

        [Column(TypeName = "decimal(18,2)")]
        public decimal En { get; set; } = 0;

        [Column(TypeName = "decimal(18,2)")]
        public decimal Boy { get; set; } = 0;

        [Column(TypeName = "decimal(18,2)")]
        public decimal M2 { get; set; } = 0;
    }

    public class TeklifKalem : TeklifKalemi
    {
    }
}