using MassTransit;
using Contracts;
using Serilog;

namespace SearchService.Consumers;

public class StockChangedConsumer : IConsumer<StockChanged>
{
    private readonly ISearchService _searchService;

    public StockChangedConsumer(ISearchService searchService)
    {
        _searchService = searchService;
    }

    public async Task Consume(ConsumeContext<StockChanged> context)
    {
        var evt = context.Message;
        Log.Information("Consuming StockChanged event for {Isbn}: new stock {NewStockQuantity}",
            evt.Isbn, evt.NewStockQuantity);

        await _searchService.UpdateBookStockAsync(evt.Isbn, evt.NewStockQuantity);
    }
}
