
using Microsoft.EntityFrameworkCore;
using MVW_Proyecto_Mesas_Comida.Data;
using MVW_Proyecto_Mesas_Comida.Models;
using MVW_Proyecto_Mesas_Comida.Services;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();


builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Connection")));
// Configurar el servicio de usuario
builder.Services.AddScoped<IUsuarioService, UsuarioService>(); // Agrega esta línea
builder.Services.AddScoped<IAuthService, AuthService>();

// Añadir servicios de sesión
builder.Services.AddSession(options =>
{
	options.IdleTimeout = TimeSpan.FromMinutes(30); // Establecer el tiempo de espera de la sesión
	options.Cookie.HttpOnly = true; // Asegurar que la cookie de sesión no sea accesible por JavaScript
	options.Cookie.IsEssential = true; // Hacer que la cookie de sesión sea esencial
});
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
// Habilitar la sesión
app.UseSession();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
