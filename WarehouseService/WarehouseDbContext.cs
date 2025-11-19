using Microsoft.EntityFrameworkCore;

namespace WarehouseService;

public class WarehouseDbContext : DbContext
{
    public WarehouseDbContext(DbContextOptions<WarehouseDbContext> options) : base(options) { }

    public DbSet<Book> Books { get; set; }
    public DbSet<StockLevel> StockLevels { get; set; }
    public DbSet<InventoryEvent> InventoryEvents { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Book>(entity =>
        {
            entity.HasKey(e => e.Isbn);
            entity.Property(e => e.Isbn).HasMaxLength(20);
            entity.Property(e => e.Title).IsRequired().HasMaxLength(500);
            entity.Property(e => e.Author).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Publisher).HasMaxLength(200);
        });

        modelBuilder.Entity<StockLevel>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Isbn).HasMaxLength(20).IsRequired();
            entity.Property(e => e.Quantity).IsRequired();
            entity.Property(e => e.LastUpdated).IsRequired();
        });

        modelBuilder.Entity<InventoryEvent>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.EventType).IsRequired();
            entity.Property(e => e.AggregateId).IsRequired();
            entity.Property(e => e.EventData).IsRequired();
            entity.Property(e => e.Timestamp).IsRequired();
        });
    }
}

public class Book
{
    public string Isbn { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
    public string Publisher { get; set; } = string.Empty;
    public DateTime PublicationDate { get; set; }
}

public class StockLevel
{
    public int Id { get; set; }
    public string Isbn { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public DateTime LastUpdated { get; set; }
}

public class InventoryEvent
{
    public int Id { get; set; }
    public string AggregateId { get; set; } = string.Empty; // ISBN for books, or book listing ID
    public string EventType { get; set; } = string.Empty; // "StockChanged", "BookAdded", etc.
    public string EventData { get; set; } = string.Empty; // JSON serialized event
    public DateTime Timestamp { get; set; }
}
