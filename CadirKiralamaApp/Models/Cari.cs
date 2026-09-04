using System.Text.Json;
using System.ComponentModel.DataAnnotations.Schema; // [NotMapped] için gerekli

namespace CadirKiralamaApp.Models
{
    public class Cari
    {
        public int Id { get; set; }
        public string FirmaUnvani { get; set; } = string.Empty;
        public string? CariTipi { get; set; }
        public string? YetkiliKisi { get; set; }
        public string? Telefon { get; set; }
        public string? Eposta { get; set; }
        public string? VergiDairesi { get; set; }
        public string? VergiNo { get; set; }
        public string? Adres { get; set; }
        public string? Sehir { get; set; }
        public string? Sektor { get; set; }
        public string? WebSitesi { get; set; }
        public string? Fax { get; set; }
        public string? Ulke { get; set; }
        public string? Ilce { get; set; }
        public string? TemsilcilerJson { get; set; }

        // 🚀 ANA TEMSİLCİ ADINI DÖNDÜREN OTOMATİK MANTIKSAL ALAN
        [NotMapped]
        public string AnaTemsilci
        {
            get
            {
                if (string.IsNullOrWhiteSpace(TemsilcilerJson))
                    return !string.IsNullOrEmpty(YetkiliKisi) ? YetkiliKisi : "-";

                try
                {
                    using var doc = JsonDocument.Parse(TemsilcilerJson);
                    var root = doc.RootElement;
                    if (root.ValueKind == JsonValueKind.Array && root.GetArrayLength() > 0)
                    {
                        // İlk temsilciyi Ana Temsilci kabul ediyoruz
                        if (root[0].TryGetProperty("name", out var nameProp))
                        {
                            return nameProp.GetString() ?? "-";
                        }
                    }
                }
                catch
                {
                    // JSON okunamazsa fallback
                }

                return !string.IsNullOrEmpty(YetkiliKisi) ? YetkiliKisi : "-";
            }
        }
    }
}