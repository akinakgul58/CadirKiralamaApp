using System.ComponentModel.DataAnnotations;

namespace CadirKiralamaApp.Models
{
    public class Stok
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Ürün Kodu")]
        public string UrunKodu { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Ürün / Çadır Adı")]
        public string UrunAdi { get; set; } = string.Empty;

        [Display(Name = "Toplam Stok")]
        public int ToplamAdet { get; set; }

        [Display(Name = "Sahada / Kirada")]
        public int SahadakiAdet { get; set; }

        [Display(Name = "Depodaki Adet")]
        public int DepodakiAdet => ToplamAdet - SahadakiAdet;
    }
}