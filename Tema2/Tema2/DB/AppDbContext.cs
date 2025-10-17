namespace Tema2.DB;
using Tema2.BookInfo;
public sealed class AppDbContext : DbContext
{
    public DbSet<Book> Books => Set<Book>();
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Book>().ToTable("Books");
        base.OnModelCreating(modelBuilder);
    }
    
}