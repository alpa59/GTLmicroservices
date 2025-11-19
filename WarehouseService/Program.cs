using MassTransit;
using Contracts;
using WarehouseService;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<WarehouseDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register MassTransit
builder.Services.AddMassTransit(config =>
{
    config.UsingRabbitMq((ctx, cfg) =>
    {
        cfg.Host(builder.Configuration.GetValue<string>("RabbitMQ:Host") ?? "localhost");
        cfg.ConfigureEndpoints(ctx);
    });
});

var app = builder.Build();

// Auto-create database and tables
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<WarehouseDbContext>();
    await dbContext.Database.EnsureCreatedAsync();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

// Map API endpoints
app.MapGet("/books/{isbn}", async (string isbn, WarehouseDbContext db) =>
{
    var book = await db.Books.FindAsync(isbn);
    return book is not null ? Results.Ok(book) : Results.NotFound();
});

app.MapGet("/stock/{isbn}", async (string isbn, WarehouseDbContext db) =>
{
    var stock = await db.StockLevels.FirstOrDefaultAsync(s => s.Isbn == isbn);
    return stock is not null ? Results.Ok(stock) : Results.NotFound();
});

app.MapPost("/books", async (Book book, WarehouseDbContext db) =>
{
    db.Books.Add(book);
    await db.SaveChangesAsync();
    return Results.Created($"/books/{book.Isbn}", book);
});

app.MapPut("/stock/{isbn}", async (string isbn, int quantity, WarehouseDbContext db, IPublishEndpoint publishEndpoint) =>
{
    var stock = await db.StockLevels.FirstOrDefaultAsync(s => s.Isbn == isbn);
    if (stock is null)
    {
        stock = new StockLevel { Isbn = isbn, Quantity = quantity, LastUpdated = DateTime.UtcNow };
        db.StockLevels.Add(stock);
    }
    else
    {
        stock.Quantity = quantity;
        stock.LastUpdated = DateTime.UtcNow;
        db.StockLevels.Update(stock);
    }
    await db.SaveChangesAsync();

    // Publish StockChanged event
    await publishEndpoint.Publish(new StockChanged(isbn, quantity, "WarehouseService", DateTime.UtcNow));

    return Results.Ok(stock);
});

app.MapGet("/", () => "Warehouse Service");

app.Run();
