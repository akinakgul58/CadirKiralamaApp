namespace CadirKiralamaApp.Models
{
    public enum GeriBildirimTuru
    {
        Oneri = 1,
        Fikir = 2
    }

    public class GeriBildirim
    {
        public int Id { get; set; }
        public GeriBildirimTuru Tur { get; set; } // Öneri mi, Fikir mi?
        public string Mesaj { get; set; } = string.Empty;
        public DateTime Tarih { get; set; } = DateTime.Now;
        public string? Kullanici { get; set; }
    }
}