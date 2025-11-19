using MassTransit;
using Contracts;
using Serilog;

namespace SearchService.Consumers;

public class BookAddedConsumer : IConsumer<BookAdded>
{
    private readonly ISearchService _searchService;

    public BookAddedConsumer(ISearchService searchService)
    {
        _searchService = searchService;
    }

    public async Task Consume(ConsumeContext<BookAdded> context)
    {
        var evt = context.Message;
        Log.Information("Consuming BookAdded event for {Isbn}: {Title}", evt.Isbn, evt.Title);

        await _searchService.AddBookAsync(evt.Isbn, evt.Title, $"{evt.Author} - {evt.Publisher}");
        await _searchService.UpdateBookStockAsync(evt.Isbn, evt.StockQuantity);
    }
}
