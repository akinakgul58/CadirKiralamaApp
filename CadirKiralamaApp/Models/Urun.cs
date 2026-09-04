using System.ComponentModel.DataAnnotations;

namespace CadirKiralamaApp.Models
{
    public class Urun
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Ürün Kodu zorunludur.")]
        public string UrunKodu { get; set; } = string.Empty;

        [Required(ErrorMessage = "Ürün Adı zorunludur.")]
        public string UrunAdi { get; set; } = string.Empty;

        public string? UrunTipi { get; set; } = "Ürün";
        public string? UrunModeli { get; set; } = string.Empty;
        public string? Birim { get; set; } = "Adet";

        public decimal AlisFiyati { get; set; } = 0;
        public decimal SatisFiyati { get; set; } = 0;

        public string? AlisParaBirimi { get; set; } = "TL";
        public string? SatisParaBirimi { get; set; } = "TL";
        public string? Aciklama { get; set; }

        public string? Durum { get; set; } = "Aktif";
    }
}