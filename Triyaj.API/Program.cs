using Microsoft.EntityFrameworkCore;
using Triyaj.Application.Services;
using Triyaj.Infrastructure;
using Triyaj.Infrastructure.Repositories;
using Triyaj.Infrastructure.Seed;

var builder = WebApplication.CreateBuilder(args);

//DB
builder.Services.AddDbContext<TriyajDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("TriyajDb")));

//Repository
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

//Services
builder.Services.AddScoped<ITriageService, TriageService>();

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddControllersWithViews(); // MVC View desteği için
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

var app = builder.Build();
// Seed Data - Veritabanını başlangıç verileriyle doldur
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<TriyajDbContext>();
        context.Database.Migrate(); // Migration'ları uygula
        DbSeeder.Seed(context); // Seed data'yı yükle
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Seed data yüklenirken bir hata oluştu: {Message}", ex.Message);
    }
}


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseStaticFiles(); // Static files için

app.UseCors("AllowAll"); // CORS'u aktifleştir

app.UseRouting();
app.UseAuthorization();

// Root endpoint - Ana sayfaya gidince Encounters Index'e yönlendir
app.MapGet("/", () => Results.Redirect("/Encounters")).ExcludeFromDescription();

app.MapControllers();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Encounters}/{action=Index}/{id?}"); // MVC route için

app.Run();
