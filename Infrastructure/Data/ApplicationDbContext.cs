using Dominio.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace Infrastructure.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Printer> Impresoras { get; set; }
        public DbSet<PrinterModel> Modelos { get; set; }
        public DbSet<Location> Ubicaciones { get; set; }
        public DbSet<OidConfiguration> Oids { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuración Printer
            modelBuilder.Entity<Printer>(entity =>
            {
                entity.ToTable("impresoras");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.IpAddress).HasColumnName("ip").IsRequired().HasMaxLength(15);
                entity.Property(e => e.LocationText).HasColumnName("ubicacion_texto").HasMaxLength(100);
                entity.Property(e => e.ModelId).HasColumnName("modelo_id");
                entity.Property(e => e.LocationId).HasColumnName("ubicacion_id");

                // Relaciones
                entity.HasOne(p => p.Model)
                    .WithMany(m => m.Printers)
                    .HasForeignKey(p => p.ModelId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(p => p.Location)
                    .WithMany(l => l.Printers)
                    .HasForeignKey(p => p.LocationId)
                    .OnDelete(DeleteBehavior.SetNull);

                // Ignorar propiedades calculadas (no persisten en BD)
                entity.Ignore(p => p.MacAddress);
                entity.Ignore(p => p.SerialNumber);
                entity.Ignore(p => p.PageCount);
                entity.Ignore(p => p.Status);
                entity.Ignore(p => p.TonerLevels);
                entity.Ignore(p => p.WasteContainerLevel);
                entity.Ignore(p => p.ImageUnitLevel);
            });

            // Configuración PrinterModel
            modelBuilder.Entity<PrinterModel>(entity =>
            {
                entity.ToTable("modelos");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Name).HasColumnName("nombre").IsRequired().HasMaxLength(100);
            });

            // Configuración Location
            modelBuilder.Entity<Location>(entity =>
            {
                entity.ToTable("ubicaciones");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Name).HasColumnName("nombre").IsRequired().HasMaxLength(100);
            });

            // Configuración OidConfiguration
            modelBuilder.Entity<OidConfiguration>(entity =>
            {
                entity.ToTable("oids");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.ModelId).HasColumnName("modelo_id");

                entity.Property(e => e.OidMac).HasColumnName("oid_mac").HasMaxLength(100);
                entity.Property(e => e.OidModel).HasColumnName("oid_model").HasMaxLength(100);
                entity.Property(e => e.OidSerial).HasColumnName("oid_serial").HasMaxLength(100);
                entity.Property(e => e.OidPageCount).HasColumnName("oid_page_count").HasMaxLength(100);

                entity.Property(e => e.OidBlackToner).HasColumnName("oid_black_toner").HasMaxLength(100);
                entity.Property(e => e.OidBlackTonerFull).HasColumnName("oid_black_toner_full").HasMaxLength(100);
                entity.Property(e => e.OidCyanToner).HasColumnName("oid_cyan_toner").HasMaxLength(100);
                entity.Property(e => e.OidCyanTonerFull).HasColumnName("oid_cyan_toner_full").HasMaxLength(100);
                entity.Property(e => e.OidMagentaToner).HasColumnName("oid_magenta_toner").HasMaxLength(100);
                entity.Property(e => e.OidMagentaTonerFull).HasColumnName("oid_magenta_toner_full").HasMaxLength(100);
                entity.Property(e => e.OidYellowToner).HasColumnName("oid_yellow_toner").HasMaxLength(100);
                entity.Property(e => e.OidYellowTonerFull).HasColumnName("oid_yellow_toner_full").HasMaxLength(100);

                entity.Property(e => e.OidWasteContainer).HasColumnName("oid_waste_container").HasMaxLength(100);
                entity.Property(e => e.OidUnitImage).HasColumnName("oid_unit_image").HasMaxLength(100);
                entity.Property(e => e.OidUnitImageFull).HasColumnName("oid_unit_image_full").HasMaxLength(100);

                // Relación uno a uno con PrinterModel
                entity.HasOne(o => o.Model)
                    .WithOne(m => m.OidConfiguration)
                    .HasForeignKey<OidConfiguration>(o => o.ModelId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}