using Microsoft.EntityFrameworkCore;
using MemberManagementSystem.Web.Models;

namespace MemberManagementSystem.Web.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users => Set<User>();
        public DbSet<Member> Members => Set<Member>();
        public DbSet<Document> Documents => Set<Document>();
        public DbSet<Payment> Payments => Set<Payment>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Member configuration
            modelBuilder.Entity<Member>(entity =>
            {
                entity.HasKey(e => e.MemberId);
                entity.HasIndex(e => e.MemberCode).IsUnique();
                entity.HasIndex(e => e.MobileNumber);
                entity.HasIndex(e => e.FullName);
                entity.HasIndex(e => e.District);
                entity.HasIndex(e => e.Status);

                entity.Property(e => e.Status).HasDefaultValue("Active");
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETDATE()");
            });

            // Document configuration
            modelBuilder.Entity<Document>(entity =>
            {
                entity.HasKey(e => e.DocumentId);
                entity.HasIndex(e => e.MemberId);

                entity.HasOne(d => d.Member)
                      .WithMany(m => m.Documents)
                      .HasForeignKey(d => d.MemberId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.Property(e => e.UploadDate).HasDefaultValueSql("GETDATE()");
            });

            // Payment configuration
            modelBuilder.Entity<Payment>(entity =>
            {
                entity.HasKey(e => e.PaymentId);
                entity.HasIndex(e => e.InvoiceNo).IsUnique();
                entity.HasIndex(e => e.MemberId);
                entity.Property(e => e.Amount).HasColumnType("decimal(18,2)");
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETDATE()");

                entity.HasOne(p => p.Member)
                      .WithMany(m => m.Payments)
                      .HasForeignKey(p => p.MemberId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // User configuration
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.UserId);
                entity.HasIndex(e => e.Username).IsUnique();
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETDATE()");
            });
        }
    }
}

