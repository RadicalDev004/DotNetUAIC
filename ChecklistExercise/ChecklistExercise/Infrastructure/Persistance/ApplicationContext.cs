using ChecklistExercise.Domain.Entities.Orders;
using Microsoft.EntityFrameworkCore;

public class ApplicationContext : DbContext
    {
        public ApplicationContext(DbContextOptions<ApplicationContext> options)
            : base(options)
        {
        }

        public DbSet<Order> Orders { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Order>(entity =>
            {
                entity.HasKey(o => o.Id);
                entity.Property(o => o.Title).IsRequired().HasMaxLength(200);
                entity.Property(o => o.Author).IsRequired().HasMaxLength(100);
                entity.Property(o => o.ISBN).IsRequired().HasMaxLength(20);
                entity.HasIndex(o => o.ISBN).IsUnique();
            });
        }
    }