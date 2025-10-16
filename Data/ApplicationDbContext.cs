using Microsoft.EntityFrameworkCore;
using FormEngineAPI.Models;

namespace FormEngineAPI.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Form> Forms { get; set; }
    public DbSet<FormSubmission> FormSubmissions { get; set; }
    public DbSet<Menu> Menus { get; set; }
    public DbSet<ActivityLog> ActivityLogs { get; set; }
    public DbSet<HistoricoFormSubmission> HistoricoFormSubmissions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // User configuration
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Email).IsUnique();
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(200);
            entity.Property(e => e.PasswordHash).IsRequired();
            entity.Property(e => e.Role).IsRequired().HasMaxLength(50);
        });

        // Form configuration
        modelBuilder.Entity<Form>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.SchemaJson).IsRequired().HasColumnType("text");
            entity.Property(e => e.RolesAllowed).HasMaxLength(500);
            entity.Property(e => e.Version).HasMaxLength(10);
            entity.Property(e => e.IsLatest).IsRequired().HasDefaultValue(true);
            
            entity.HasIndex(e => e.OriginalFormId);
            entity.HasIndex(e => new { e.OriginalFormId, e.IsLatest });
            
            entity.HasOne(e => e.OriginalForm)
                .WithMany(f => f.Versions)
                .HasForeignKey(e => e.OriginalFormId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // FormSubmission configuration
        modelBuilder.Entity<FormSubmission>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.DataJson).IsRequired().HasColumnType("JSON");
            entity.Property(e => e.Status).IsRequired();
            entity.Property(e => e.DataAtualizacao).IsRequired();
            entity.Property(e => e.Versao).IsRequired().HasDefaultValue(1);
            entity.Property(e => e.Excluido).IsRequired().HasDefaultValue(false);
            entity.Property(e => e.MotivoRejeicao).HasMaxLength(1000);
            entity.Property(e => e.EnderecoIp).HasMaxLength(45);
            entity.Property(e => e.UserAgent).HasMaxLength(500);
            
            // Indexes for performance
            entity.HasIndex(e => e.Status);
            entity.HasIndex(e => e.DataSubmissao);
            entity.HasIndex(e => e.DataAprovacao);
            entity.HasIndex(e => e.Excluido);
            entity.HasIndex(e => new { e.FormId, e.Status });
            entity.HasIndex(e => new { e.UserId, e.Status });
            
            entity.HasOne(e => e.Form)
                .WithMany(f => f.FormSubmissions)
                .HasForeignKey(e => e.FormId)
                .OnDelete(DeleteBehavior.Cascade);
                
            entity.HasOne(e => e.User)
                .WithMany(u => u.FormSubmissions)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.UsuarioAprovador)
                .WithMany()
                .HasForeignKey(e => e.UsuarioAprovadorId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Menu configuration
        modelBuilder.Entity<Menu>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.ContentType).IsRequired().HasMaxLength(50);
            entity.Property(e => e.UrlOrPath).HasMaxLength(500);
            entity.Property(e => e.RolesAllowed).HasMaxLength(500);
            entity.Property(e => e.Icon).HasMaxLength(100);
            entity.Property(e => e.FormVersion).HasMaxLength(10);
            entity.Property(e => e.UseLatestVersion).IsRequired().HasDefaultValue(true);
            
            entity.HasOne(e => e.Parent)
                .WithMany(m => m.Children)
                .HasForeignKey(e => e.ParentId)
                .OnDelete(DeleteBehavior.Restrict);
                
            entity.HasOne(e => e.OriginalForm)
                .WithMany()
                .HasForeignKey(e => e.OriginalFormId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // ActivityLog configuration
        modelBuilder.Entity<ActivityLog>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Action).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Entity).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Details).HasColumnType("text");
            
            entity.HasOne(e => e.User)
                .WithMany(u => u.ActivityLogs)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // HistoricoFormSubmission configuration
        modelBuilder.Entity<HistoricoFormSubmission>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Acao).IsRequired();
            entity.Property(e => e.DataAcao).IsRequired();
            entity.Property(e => e.Comentario).HasMaxLength(1000);
            entity.Property(e => e.EnderecoIp).HasMaxLength(45);
            entity.Property(e => e.UserAgent).HasMaxLength(500);
            entity.Property(e => e.DadosAlteracao).HasColumnType("JSON");
            
            // Indexes for performance
            entity.HasIndex(e => e.FormSubmissionId);
            entity.HasIndex(e => e.UsuarioId);
            entity.HasIndex(e => e.DataAcao);
            entity.HasIndex(e => e.Acao);
            entity.HasIndex(e => new { e.FormSubmissionId, e.DataAcao });
            
            entity.HasOne(e => e.FormSubmission)
                .WithMany(fs => fs.Historicos)
                .HasForeignKey(e => e.FormSubmissionId)
                .OnDelete(DeleteBehavior.Cascade);
                
            entity.HasOne(e => e.Usuario)
                .WithMany()
                .HasForeignKey(e => e.UsuarioId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Seed data
        SeedData(modelBuilder);
    }

    private void SeedData(ModelBuilder modelBuilder)
    {
        // Seed Admin User
        modelBuilder.Entity<User>().HasData(new User
        {
            Id = 1,
            Name = "Admin",
            Email = "admin@formengine.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
            Role = "admin",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        });
    }
}
