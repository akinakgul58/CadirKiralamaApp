using System;
using System.ComponentModel.DataAnnotations;

namespace CadirKiralamaApp.Models
{
    public class HataBildirimi
    {
        public int Id { get; set; }

        [Required]
        public string Baslik { get; set; } = string.Empty;

        [Required]
        public string Aciklama { get; set; } = string.Empty;

        public DateTime Tarih { get; set; } = DateTime.Now;
    }
}