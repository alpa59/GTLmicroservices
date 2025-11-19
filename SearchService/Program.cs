using Serilog;
using SearchService;
using StackExchange.Redis;
using MassTransit;
using SearchService.Consumers;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
builder.Host.UseSerilog((context, configuration) =>
    configuration.ReadFrom.Configuration(context.Configuration));

// Add services to the container.
builder.Services.AddControllers();

// Register Redis - with retry configuration for container startup
var redisConnectionString = builder.Configuration.GetValue<string>("Redis:ConnectionString") ?? "localhost:6379";
var redisConfig = ConfigurationOptions.Parse(redisConnectionString);
redisConfig.AbortOnConnectFail = false; // Allow retrying until Redis is available
builder.Services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(redisConfig));

// Register MassTransit
builder.Services.AddMassTransit(config =>
{
    config.AddConsumer<BookAddedConsumer>();
    config.AddConsumer<StockChangedConsumer>();

    config.UsingRabbitMq((ctx, cfg) =>
    {
        cfg.Host(builder.Configuration.GetValue<string>("RabbitMQ:Host") ?? "localhost");
        cfg.ConfigureEndpoints(ctx);
    });
});

// Register search service (Redis implementation)
builder.Services.AddScoped<ISearchService, RedisSearchService>();

// Add health checks
builder.Services.AddHealthChecks()
    .AddRedis(redisConnectionString, name: "redis");

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();

// Map health checks
app.MapHealthChecks("/health");

app.MapControllers();

app.Run();
