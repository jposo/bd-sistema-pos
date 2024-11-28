using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace The_bear_proyecto_final.Models;

public partial class RestauranteContext : DbContext
{
    public RestauranteContext()
    {
    }

    public RestauranteContext(DbContextOptions<RestauranteContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Chef> Chefs { get; set; }

    public virtual DbSet<Mesa> Mesas { get; set; }

    public virtual DbSet<Pedido> Pedidos { get; set; }

    public virtual DbSet<PedidoPlatillo> PedidoPlatillos { get; set; }

    public virtual DbSet<Platillo> Platillos { get; set; }

    public virtual DbSet<VentasMeseroMesa> VentasMeseroMesas { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            //#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
            //=> optionsBuilder.UseSqlServer("server=LAPTOP-37HOSF9K\\SQLEXPRESS; database=restaurante; integrated security=true; TrustServerCertificate=True;");
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Chef>(entity =>
        {
            entity.HasKey(e => e.IdChef).HasName("PK__chef__68D7E078584E829F");

            entity.ToTable("chef");

            entity.Property(e => e.IdChef).HasColumnName("id_chef");
            entity.Property(e => e.Apellido)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("apellido");
            entity.Property(e => e.FechaIngreso)
                .HasColumnType("datetime")
                .HasColumnName("fecha_ingreso");
            entity.Property(e => e.FechaSalida)
                .HasColumnType("datetime")
                .HasColumnName("fecha_salida");
            entity.Property(e => e.Nombre)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("nombre");
            entity.Property(e => e.Puesto)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("puesto");
            entity.Property(e => e.Sueldo)
                .HasColumnType("money")
                .HasColumnName("sueldo");
        });

        modelBuilder.Entity<Mesa>(entity =>
        {
            entity.HasKey(e => e.IdMesa).HasName("PK__mesa__68A1E1599FD8DF2C");

            entity.ToTable("mesa");

            entity.HasIndex(e => e.Ubicacion, "idx_ubicacion_mesa");

            entity.Property(e => e.IdMesa).HasColumnName("id_mesa");
            entity.Property(e => e.Capacidad).HasColumnName("capacidad");
            entity.Property(e => e.Ubicacion)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("ubicacion");
        });

        modelBuilder.Entity<Pedido>(entity =>
        {
            entity.HasKey(e => e.IdPedido).HasName("PK__pedido__6FF0148993518E90");

            entity.ToTable("pedido");

            entity.Property(e => e.IdPedido).HasColumnName("id_pedido");
            entity.Property(e => e.IdMesa).HasColumnName("id_mesa");
            entity.Property(e => e.IdMesero).HasColumnName("id_mesero");
            entity.Property(e => e.Fecha).HasColumnName("fecha");
            entity.Property(e => e.Pagado).HasColumnName("pagado");
            entity.Property(e => e.Subtotal)
                .HasColumnType("money")
                .HasColumnName("subtotal");

            entity.HasOne(d => d.IdMesaNavigation).WithMany(p => p.Pedidos)
                .HasForeignKey(d => d.IdMesa)
                .HasConstraintName("FK__pedido__id_mesa__5165187F");

            entity.HasOne(d => d.IdMeseroNavigation).WithMany(p => p.Pedidos)
                .HasForeignKey(d => d.IdMesero)
                .HasConstraintName("FK__pedido__id_meser__52593CB8");
        });

        modelBuilder.Entity<PedidoPlatillo>(entity =>
        {
            entity.HasKey(e => new { e.IdPedido, e.IdPlatillo }).HasName("pk_pedido_menu");

            entity.ToTable("pedido_platillo", tb => tb.HasTrigger("actualizar_estado_pedido"));

            entity.HasIndex(e => e.Estado, "idx_estado_pedido_platillo");

            entity.Property(e => e.IdPedido).HasColumnName("id_pedido");
            entity.Property(e => e.IdPlatillo).HasColumnName("id_platillo");
            entity.Property(e => e.CantidadPedido).HasColumnName("cantidad_pedido");
            entity.Property(e => e.Especificaciones)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("especificaciones");
            entity.Property(e => e.Estado)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("estado");

            entity.HasOne(d => d.IdPedidoNavigation).WithMany(p => p.PedidoPlatillos)
                .HasForeignKey(d => d.IdPedido)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__pedido_pl__id_pe__5535A963");

            entity.HasOne(d => d.IdPlatilloNavigation).WithMany(p => p.PedidoPlatillos)
                .HasForeignKey(d => d.IdPlatillo)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__pedido_pl__id_pl__5629CD9C");
        });

        modelBuilder.Entity<Platillo>(entity =>
        {
            entity.HasKey(e => e.IdPlatillo).HasName("PK__platillo__A73C821FE0DABF8C");

            entity.ToTable("platillo");

            entity.HasIndex(e => e.Tipo, "idx_tipo_platillo");

            entity.Property(e => e.IdPlatillo).HasColumnName("id_platillo");
            entity.Property(e => e.Calorias).HasColumnName("calorias");
            entity.Property(e => e.Descripcion)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("descripcion");
            entity.Property(e => e.FechaIngreso)
                .HasColumnType("datetime")
                .HasColumnName("fecha_ingreso");
            entity.Property(e => e.FechaSalida)
                .HasColumnType("datetime")
                .HasColumnName("fecha_salida");
            entity.Property(e => e.Gramos).HasColumnName("gramos");
            entity.Property(e => e.Nombre)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("nombre");
            entity.Property(e => e.Precio)
                .HasColumnType("money")
                .HasColumnName("precio");
            entity.Property(e => e.Tipo)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("tipo");
        });

        modelBuilder.Entity<VentasMeseroMesa>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("ventas_mesero_mesa");

            entity.Property(e => e.Mesero)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.Pagado).HasColumnName("pagado");
            entity.Property(e => e.Subtotal)
                .HasColumnType("money")
                .HasColumnName("subtotal");
            entity.Property(e => e.UbicacionMesa)
                .HasMaxLength(20)
                .IsUnicode(false);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
