using System.Collections.Generic;
using CadirKiralamaApp.Models;

namespace CadirKiralamaApp.ViewModels
{
    public class TeklifListesiViewModel
    {
        public List<Teklif> Teklifler { get; set; } = new List<Teklif>();

        // Filtreleme ve Arama
        public string? AramaKelimesi { get; set; }
        public TeklifDurumu? DurumFiltresi { get; set; }

        // Sayfalama (Pagination)
        public int MevcutSayfa { get; set; } = 1;
        public int SayfaBoyutu { get; set; } = 5;
        public int ToplamOgeSayisi { get; set; }
        public int ToplamSayfaSayisi => (int)System.Math.Ceiling((double)ToplamOgeSayisi / SayfaBoyutu);

        public int BaslangicOgesi => ToplamOgeSayisi == 0 ? 0 : ((MevcutSayfa - 1) * SayfaBoyutu) + 1;
        public int BitisOgesi => System.Math.Min(MevcutSayfa * SayfaBoyutu, ToplamOgeSayisi);
    }
}