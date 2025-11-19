using System.Collections.Concurrent;
using SearchService.Models;
using StackExchange.Redis;

namespace SearchService;

public interface ISearchService
{
    Task<SearchResponse> SearchBooksAsync(SearchRequest request);
    Task UpdateBookStockAsync(string bookId, int stockCount);
    Task AddBookAsync(string bookId, string title, string? condition = null);
}

public class InMemorySearchService : ISearchService
{
    // Thread-safe concurrent dictionary for book data
    private readonly ConcurrentDictionary<string, BookData> _books = new();

    private record BookData(string Title, int StockCount, string? Condition = null);

    public InMemorySearchService()
    {
        // Add sample data for testing
        AddSampleBooks();
    }

    private void AddSampleBooks()
    {
        var sampleBooks = new[]
        {
            ("ISBN-001", "Introduction to Algorithms", "new", 5),
            ("ISBN-002", "Design Patterns", "used", 3),
            ("ISBN-003", "Clean Code", "new", 10),
            ("ISBN-004", "Database Systems", "used", 2),
            ("ISBN-005", "Artificial Intelligence: A Modern Approach", "new", 8),
            ("ISBN-006", "Computer Networks", "used", 6),
            ("ISBN-007", "Operating System Concepts", "new", 4),
            ("ISBN-008", "Data Structures and Algorithms", "used", 7),
            ("ISBN-009", "Machine Learning", "new", 12),
            ("ISBN-010", "Software Engineering", "used", 1)
        };

        foreach (var (bookId, title, condition, stock) in sampleBooks)
        {
            _books[bookId] = new BookData(title, stock, condition);
        }
    }

    public Task<SearchResponse> SearchBooksAsync(SearchRequest request)
    {
        // Simple substring search (can be upgraded to full-text later)
        var query = request.Query?.Trim() ?? "";
        var allMatchingBooks = _books
            .Where(book => string.IsNullOrEmpty(query) ||
                          book.Value.Title.Contains(query, StringComparison.OrdinalIgnoreCase))
            .Select(book => new BookSearchResult(
                BookId: book.Key,
                Title: book.Value.Title,
                StockCount: book.Value.StockCount,
                Condition: book.Value.Condition))
            .ToList();

        var totalCount = allMatchingBooks.Count;
        var paginatedBooks = allMatchingBooks
            .Skip((request.Page - 1) * request.Size)
            .Take(request.Size)
            .ToList();

        return Task.FromResult(new SearchResponse(paginatedBooks, totalCount, request.Page, request.Size));
    }

    public Task UpdateBookStockAsync(string bookId, int stockCount)
    {
        if (_books.TryGetValue(bookId, out var existingBook))
        {
            var updatedBook = existingBook with { StockCount = stockCount };
            _books[bookId] = updatedBook;
        }
        return Task.CompletedTask;
    }

    public Task AddBookAsync(string bookId, string title, string? condition = null)
    {
        _books[bookId] = new BookData(title, 0, condition);
        return Task.CompletedTask;
    }
}

public class RedisSearchService : ISearchService
{
    private readonly IDatabase _redisDb;
    private readonly ILogger<RedisSearchService> _logger;

    private const string BookKeyPrefix = "book:";

    public RedisSearchService(IConnectionMultiplexer redisConn, ILogger<RedisSearchService> logger)
    {
        _redisDb = redisConn.GetDatabase();
        _logger = logger;
        // Seed sample data for development
        Task.Run(() => SeedSampleDataAsync());
    }

    private async Task SeedSampleDataAsync()
    {
        var sampleBooks = new[]
        {
            ("ISBN-001", "Introduction to Algorithms", "new", 5),
            ("ISBN-002", "Design Patterns", "used", 3),
            ("ISBN-003", "Clean Code", "new", 10),
            ("ISBN-004", "Database Systems", "used", 2),
            ("ISBN-005", "Artificial Intelligence: A Modern Approach", "new", 8),
            ("ISBN-006", "Computer Networks", "used", 6),
            ("ISBN-007", "Operating System Concepts", "new", 4),
            ("ISBN-008", "Data Structures and Algorithms", "used", 7),
            ("ISBN-009", "Machine Learning", "new", 12),
            ("ISBN-010", "Software Engineering", "used", 1)
        };

        bool hasData = false;
        var keys = GetAllBookKeys();
        if (keys.Count > 0)
        {
            hasData = true;
        }

        if (!hasData)
        {
            foreach (var (bookId, title, condition, stock) in sampleBooks)
            {
                await AddBookAsync(bookId, title, condition);
                await UpdateBookStockAsync(bookId, stock);
            }
            _logger.LogInformation("Seeded sample books into Redis");
        }
    }

    public async Task<SearchResponse> SearchBooksAsync(SearchRequest request)
    {
        var query = request.Query?.Trim() ?? "";
        _logger.LogInformation("Searching books with query '{Query}'", query);

        var bookKeys = GetAllBookKeys();

        var matchingBooks = new List<BookSearchResult>();

        foreach (var bookKey in bookKeys)
        {
            var hashEntries = await _redisDb.HashGetAllAsync(bookKey);
            if (!hashEntries.Any()) continue;

            var title = hashEntries.FirstOrDefault(e => e.Name == "title").Value.ToString() ?? "";
            var stockCount = int.Parse(hashEntries.FirstOrDefault(e => e.Name == "stock").Value.ToString() ?? "0");
            var condition = hashEntries.FirstOrDefault(e => e.Name == "condition").Value.ToString();

            if (string.IsNullOrEmpty(query) || title.Contains(query, StringComparison.OrdinalIgnoreCase))
            {
                var bookId = bookKey.ToString().Replace(BookKeyPrefix, "");
                matchingBooks.Add(new BookSearchResult(bookId, title, stockCount, condition));
            }
        }

        var totalCount = matchingBooks.Count;

        // Apply pagination
        var paginatedResults = matchingBooks
            .Skip((request.Page - 1) * request.Size)
            .Take(request.Size)
            .ToList();

        return new SearchResponse(paginatedResults, totalCount, request.Page, request.Size);
    }

    public async Task UpdateBookStockAsync(string bookId, int stockCount)
    {
        var bookKey = $"{BookKeyPrefix}{bookId}";
        await _redisDb.HashSetAsync(bookKey, "stock", stockCount);
        _logger.LogInformation("Updated stock for book {BookId} to {StockCount}", bookId, stockCount);
    }

    public async Task AddBookAsync(string bookId, string title, string? condition = null)
    {
        var bookKey = $"{BookKeyPrefix}{bookId}";
        var hashEntries = new HashEntry[]
        {
            new HashEntry("title", title),
            new HashEntry("stock", 0),
            new HashEntry("condition", condition ?? "")
        };

        await _redisDb.HashSetAsync(bookKey, hashEntries);
        _logger.LogInformation("Added book {BookId}: {Title}", bookId, title);
    }

    private List<RedisKey> GetAllBookKeys()
    {
        var keys = new List<RedisKey>();
        var endpoints = _redisDb.Multiplexer.GetEndPoints();
        foreach (var endpoint in endpoints)
        {
            var server = _redisDb.Multiplexer.GetServer(endpoint);
            keys.AddRange(server.Keys(pattern: $"{BookKeyPrefix}*"));
        }
        return keys;
    }
}
