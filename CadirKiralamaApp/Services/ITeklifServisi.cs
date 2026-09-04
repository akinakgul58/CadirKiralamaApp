using System.Threading.Tasks;
using CadirKiralamaApp.Models;
using CadirKiralamaApp.ViewModels;

namespace CadirKiralamaApp.Services
{
    public interface ITeklifServisi
    {
        Task<TeklifListesiViewModel> SayfaliTeklifleriGetirAsync(string? aramaKelimesi, TeklifDurumu? durum, int sayfa, int sayfaBoyutu);
    }
}