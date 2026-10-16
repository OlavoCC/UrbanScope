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
        modelBuilder.Entity<NeighborhoodModel>().ToTable("neighborhood");
        modelBuilder.Entity<ComplaintModel>().ToTable("complaint");
        modelBuilder.Entity<ComplaintImageModel>().ToTable("complaint_image");
        modelBuilder.Entity<ComplaintStatusModel>().ToTable("complaint_status");
        modelBuilder.Entity<RoleModel>().ToTable("role");
        modelBuilder.Entity<StatusHistoryModel>().ToTable("status_history");
        modelBuilder.Entity<UserModel>().ToTable("user");
        modelBuilder.Entity<UserDeviceModel>().ToTable("user_device");
        modelBuilder.Entity<UserRoleModel>().ToTable("user_role");

        // ------------------------------------------------------------------
        // A propriedade "Id" (C#) mora na coluna "id" (Postgres).
        // Todas as tabelas seguem esse padrao, entao resolvemos num loop.
        // ------------------------------------------------------------------
        foreach (var entidade in modelBuilder.Model.GetEntityTypes())
        {
            var pk = entidade.FindPrimaryKey();
            if (pk is { Properties.Count: 1 } && pk.Properties[0].Name == "Id")
                pk.Properties[0].SetColumnName("id");
        }

        // ------------------------------------------------------------------
        // No Postgres, latitude/longitude sao numeric(10,7)
        // ------------------------------------------------------------------
        modelBuilder.Entity<ComplaintModel>().Property(c => c.latitude).HasPrecision(10, 7);
        modelBuilder.Entity<ComplaintModel>().Property(c => c.longitude).HasPrecision(10, 7);
    }
}