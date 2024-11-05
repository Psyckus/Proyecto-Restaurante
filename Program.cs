using Microsoft.EntityFrameworkCore;
using MVW_Proyecto_Mesas_Comida.Data;
using MVW_Proyecto_Mesas_Comida.Models;
using MVW_Proyecto_Mesas_Comida.Services;

var builder = WebApplication.CreateBuilder(args);

// Configuraci�n de servicios
builder.Services.AddControllersWithViews();

// Configuraci�n de la base de datos
builder.Services.AddDbContext<ApplicationDbContext>(options =>
	options.UseSqlServer(builder.Configuration.GetConnectionString("Connection")));

// Configuraci�n de Redis
builder.Services.AddStackExchangeRedisCache(options =>
{
	options.Configuration = "localhost:6379"; // La direcci�n de tu servidor Redis
	options.InstanceName = "RedisCacheInstance";
});

// Registro de servicios personalizados
builder.Services.AddSingleton<ShoppingCartService>(); // Registra el servicio de carrito de compras
builder.Services.AddScoped<IReservaService, ReservaService>();
builder.Services.AddScoped<IUsuarioService, UsuarioService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IRestauranteService, RestauranteService>();
builder.Services.AddScoped<IPlatilloService, PlatilloService>();
builder.Services.AddScoped<SessionAuthorizeAttribute>(); // Registro del filtro de autorizaci�n

var app = builder.Build();

// Configuraci�n del pipeline de middleware
if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Home/Error");
	app.UseHsts(); // Configuraci�n de HSTS para producci�n
}

app.UseHttpsRedirection();
app.UseStaticFiles(); // Solo necesitas llamar a UseStaticFiles una vez

app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
	name: "default",
	pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
