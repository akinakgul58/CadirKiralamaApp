using Microsoft.EntityFrameworkCore;
using CadirKiralamaApp.Data;
using CadirKiralamaApp.Services;

var builder = WebApplication.CreateBuilder(args);

// DbContext Kaydı
builder.Services.AddDbContext<UygulamaDbContext>(options =>
    options.UseInMemoryDatabase("CadirKiralamaDb"));

// Servis Kaydı (Dependency Injection)
builder.Services.AddScoped<ITeklifServisi, TeklifServisi>();

builder.Services.AddControllersWithViews();

var app = builder.Build();

// Veritabanı ve Seed verilerin otomatik yüklenmesi
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<UygulamaDbContext>();
    context.Database.EnsureCreated();
}

app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Dashboard}/{action=Index}/{id?}");

app.Run();