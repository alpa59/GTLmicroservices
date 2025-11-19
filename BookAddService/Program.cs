using MassTransit;
using Contracts;
using BookAddService;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<BookAddDbContext>(options =>
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
    var dbContext = scope.ServiceProvider.GetRequiredService<BookAddDbContext>();
    await dbContext.Database.EnsureCreatedAsync();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

// Map API endpoints
app.MapGet("/listings/{id}", async (int id, BookAddDbContext db) =>
{
    var listing = await db.BookListings.FindAsync(id);
    return listing is not null ? Results.Ok(listing) : Results.NotFound();
});

app.MapGet("/listings", async (BookAddDbContext db) =>
{
    var listings = await db.BookListings.ToListAsync();
    return Results.Ok(listings);
});

app.MapPost("/listings", async (BookListing listing, BookAddDbContext db, IPublishEndpoint publishEndpoint) =>
{
    listing.ListedDate = DateTime.UtcNow;
    db.BookListings.Add(listing);
    await db.SaveChangesAsync();

    // Publish BookAdded event
    await publishEndpoint.Publish(new BookAdded(
        listing.Isbn,
        listing.Title,
        listing.Author,
        listing.Publisher,
        listing.PublicationDate,
        listing.Price,
        listing.SellerId,
        listing.Quantity,
        DateTime.UtcNow));

    return Results.Created($"/listings/{listing.Id}", listing);
});

app.MapGet("/", () => "BookAdd Service");

app.Run();
