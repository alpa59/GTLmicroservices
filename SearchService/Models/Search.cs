namespace SearchService.Models;

public record SearchRequest(string? Query, int Page = 1, int Size = 20);

public record BookSearchResult(string BookId, string Title, int StockCount, string? Condition = null);

public record SearchResponse(List<BookSearchResult> Results, int TotalCount, int Page, int Size);
