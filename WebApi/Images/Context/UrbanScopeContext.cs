using Api.Images.Models;
using Microsoft.EntityFrameworkCore;

namespace Api.Images.Context;
public class UrbanScopeContext  : DbContext
{
    public UrbanScopeContext(DbContextOptions<UrbanScopeContext> options) : base (options) {}

    //Tabelas
    public DbSet<NeighborhoodModel> Neighborhoods { get; set; }
    public DbSet<ComplaintImageModel> ComplaintImages { get; set; }
    public DbSet<ComplaintModel> Complaints { get; set; }
    public DbSet<ComplaintStatusModel> ComplaintStatuses { get; set; }
    public DbSet<RoleModel> Roles { get; set; }
    public DbSet<StatusHistoryModel> StatusHistories { get; set; }
    public DbSet<UserDeviceModel> UserDevices { get; set; }
    public DbSet<UserModel> Users { get; set; }
    public DbSet<UserRoleModel> UserRoles { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // ------------------------------------------------------------------
        // O EF usa o NOME DO DbSet como nome de tabela por padrao.
        // Aqui dizemos o nome REAL de cada tabela no Postgres (singular).
        // ------------------------------------------------------------------
        modelBuilder.Entity<NeighborhoodModel>().ToTable("neighborhood")
            .Property(x => x.Id)
                .HasColumnName("id");
                
        modelBuilder.Entity<ComplaintModel>().ToTable("complaint")
            .Property(x => x.Id)
                .HasColumnName("id");

        modelBuilder.Entity<ComplaintImageModel>().ToTable("complaint_image")
            .Property(x => x.Id)
                .HasColumnName("id");

        modelBuilder.Entity<ComplaintStatusModel>().ToTable("complaint_status")
            .Property(x => x.Id)
                .HasColumnName("id");

        modelBuilder.Entity<RoleModel>().ToTable("role")
            .Property(x => x.Id)
                .HasColumnName("id");
                
        modelBuilder.Entity<StatusHistoryModel>().ToTable("status_history")
            .Property(x => x.Id)
                .HasColumnName("id");

        modelBuilder.Entity<UserModel>().ToTable("user")
            .Property(x => x.Id)
                .HasColumnName("id");

        modelBuilder.Entity<UserDeviceModel>().ToTable("user_device")
            .Property(x => x.Id)
                .HasColumnName("id");

        modelBuilder.Entity<UserRoleModel>().ToTable("user_role")
            .Property(x => x.Id)
                .HasColumnName("id");

        // ------------------------------------------------------------------
        // No Postgres, latitude/longitude sao numeric(10,7)
        // ------------------------------------------------------------------
        modelBuilder.Entity<ComplaintModel>().Property(c => c.latitude).HasPrecision(10, 7);
        modelBuilder.Entity<ComplaintModel>().Property(c => c.longitude).HasPrecision(10, 7);
    }
}