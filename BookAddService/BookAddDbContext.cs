using Microsoft.EntityFrameworkCore;

namespace BookAddService;

public class BookAddDbContext : DbContext
{
    public BookAddDbContext(DbContextOptions<BookAddDbContext> options) : base(options) { }

    public DbSet<BookListing> BookListings { get; set; }
    public DbSet<Seller> Sellers { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<BookListing>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Isbn).HasMaxLength(20).IsRequired();
            entity.Property(e => e.Title).HasMaxLength(500).IsRequired();
            entity.Property(e => e.Author).HasMaxLength(200).IsRequired();
            entity.Property(e => e.SellerId).IsRequired();
            entity.Property(e => e.Price).HasColumnType("decimal(10,2)");
            entity.Property(e => e.ListedDate).IsRequired();
        });

        modelBuilder.Entity<Seller>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(255);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
        });
    }
}

public class BookListing
{
    public int Id { get; set; }
    public string Isbn { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
    public string Publisher { get; set; } = string.Empty;
    public DateTime PublicationDate { get; set; }
    public decimal Price { get; set; }
    public string SellerId { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public string Condition { get; set; } = "Good";
    public DateTime ListedDate { get; set; }
}

public class Seller
{
    public string Id { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; }
}
