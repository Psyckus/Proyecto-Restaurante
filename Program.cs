
using Microsoft.EntityFrameworkCore;
using MVW_Proyecto_Mesas_Comida.Data;
using MVW_Proyecto_Mesas_Comida.Models;
using MVW_Proyecto_Mesas_Comida.Services;


var builder = WebApplication.CreateBuilder(args);
// Configuración de Redis
builder.Services.AddStackExchangeRedisCache(options =>
{
	options.Configuration = "localhost:6379"; // La dirección de tu servidor Redis
	options.InstanceName = "RedisCacheInstance";
});

// Add services to the container.
builder.Services.AddControllersWithViews();


builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Connection")));
// Configurar el servicio de usuario
builder.Services.AddScoped<IUsuarioService, UsuarioService>(); 
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IRestauranteService, RestauranteService>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
