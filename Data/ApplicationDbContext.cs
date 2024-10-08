using Microsoft.EntityFrameworkCore;
using MVW_Proyecto_Mesas_Comida.Models;

namespace MVW_Proyecto_Mesas_Comida.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Rol> Roles { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Restaurante> Restaurantes { get; set; }
        public DbSet<Platillos> Platillos { get; set; }
        public DbSet<Mesas> Mesas { get; set; }
        public DbSet<Reservas> Reservas { get; set; }
        public DbSet<Compra> Compras { get; set; }
        public DbSet<DetalleCompras> DetalleCompras { get; set; }
        public DbSet<Pagos> Pagos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuraciones adicionales si es necesario

            // Por ejemplo, para establecer nombres de tablas personalizados
            modelBuilder.Entity<Rol>().ToTable("Roles");
            modelBuilder.Entity<Usuario>().ToTable("Usuarios");
            modelBuilder.Entity<Restaurante>().ToTable("Restaurantes");
            modelBuilder.Entity<Platillos>().ToTable("Platillos");
            modelBuilder.Entity<Mesas>().ToTable("Mesas");
            modelBuilder.Entity<Reservas>().ToTable("Reservas");
            modelBuilder.Entity<Compra>().ToTable("Compras");
            modelBuilder.Entity<DetalleCompras>().ToTable("DetalleCompras");
            modelBuilder.Entity<Pagos>().ToTable("Pagos");
        }
    }
}
