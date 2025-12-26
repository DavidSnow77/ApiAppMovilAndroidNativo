using Abarrotes.Models;
using Microsoft.EntityFrameworkCore;

namespace Abarrotes.BaseDedatos
{
    public class AbarrotesReyesContext : DbContext
    {
        public AbarrotesReyesContext(DbContextOptions<AbarrotesReyesContext> options) : base(options) { }

        public virtual DbSet<UsuarioEntity> Usuarios { get; set; }

        public virtual DbSet<ProductoEntity> Productos { get; set; }

        public virtual DbSet<VentaEntity> Ventas { get; set; }

        public virtual DbSet<DetalleVentaEntity> DetalleVentas { get; set; }

        public virtual DbSet<VistaDetalleVentaEntity> VistaDetalleVentas { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<UsuarioEntity>(entity =>
            {
                entity.ToTable("Usuarios");

                entity.HasKey(u => u.UsuarioId).HasName("PK_usuarioId");

                entity.Property(u => u.UsuarioId)
                    .HasColumnType("varchar")
                    .HasDefaultValueSql("generar_clave_usuario()");
            });

            modelBuilder.Entity<ProductoEntity>(entity =>

            {
                entity.ToTable("Productos");
                entity.HasKey(p => p.ProductoId).HasName("PK_productoId");

                entity.Property(u => u.ProductoId)
                   .HasColumnType("varchar")
                   .HasDefaultValueSql("generar_clave_producto()");


                entity.Property(p => p.NombreProducto)
        .IsRequired();

                entity.Property(p => p.CodigoProducto)
                .IsRequired();

                entity.Property(p => p.Cantidad)
                    .IsRequired();

                entity.Property(p => p.Precio)
                    .HasColumnType("numeric(10,2)");
            }
          );
            modelBuilder.Entity<VentaEntity>(entity =>

            {
                entity.ToTable("Ventas");
                entity.HasKey(v => v.VentaId).HasName("PK_ventaId");

                entity.Property(u => u.VentaId)
                   .HasColumnType("varchar")
                   .HasDefaultValueSql("generar_clave_venta()");

                entity.Property(v => v.FechaVenta)
                  .IsRequired();

                entity.Property(v => v.Total)
          .IsRequired()
          .HasColumnType("decimal(10,2)");
            }

         );
             modelBuilder.Entity<DetalleVentaEntity>(entity =>
            {
                 entity.ToTable("DetalleVentas");

                 //Clave compuesta
                 entity.HasKey(dv => new { dv.VentaId, dv.ProductoId }).HasName("PK_detalleVenta");

                 //Relaciones
                 entity.HasOne(dv => dv.Venta)
                  .WithMany(v => v.Detalles)
                 .HasForeignKey(dv => dv.VentaId);

                 entity.HasOne(dv => dv.Producto)
                  .WithMany(p => p.Detalles)
                 .HasForeignKey(dv => dv.ProductoId);

                entity.Property(d => d.Cantidad).IsRequired();
                entity.Property(d => d.Estado).IsRequired();

            });

            modelBuilder.Entity<VistaDetalleVentaEntity>()
            .HasNoKey()
            .ToView("vista_detalle_ventas");

        }
    }
    }


