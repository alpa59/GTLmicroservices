using SearchService;
using SearchService.Models;

namespace SearchService.Tests;

public class InMemorySearchServiceTests
{
    private readonly InMemorySearchService _service;

    public InMemorySearchServiceTests()
    {
        _service = new InMemorySearchService();
    }

    [Fact]
    public async Task SearchBooks_ReturnsAllBooks_WhenNoQuery()
    {
        // Arrange
        var request = new SearchRequest(null, 1, 20);

        // Act
        var result = await _service.SearchBooksAsync(request);

        // Assert
        Assert.Equal(10, result.TotalCount);
        Assert.Equal(10, result.Results.Count);
        Assert.Equal(1, result.Page);
        Assert.Equal(20, result.Size);
    }

    [Fact]
    public async Task SearchBooks_FiltersByQuery()
    {
        // Arrange
        var request = new SearchRequest("Algorithms", 1, 10);

        // Act
        var result = await _service.SearchBooksAsync(request);

        // Assert
        Assert.Equal(2, result.TotalCount); // Introduction to Algorithms and Data Structures and Algorithms
    }

    [Fact]
    public async Task SearchBooks_AppliesPagination()
    {
        // Arrange
        var request = new SearchRequest(null, 1, 5);

        // Act
        var result = await _service.SearchBooksAsync(request);

        // Assert
        Assert.Equal(10, result.TotalCount);
        Assert.Equal(5, result.Results.Count);
        Assert.Equal(1, result.Page);
    }

    [Fact]
    public async Task UpdateBookStock_UpdatesStockCount()
    {
        // Arrange
        var bookId = "ISBN-001";
        var newStock = 20;

        // Act
        await _service.UpdateBookStockAsync(bookId, newStock);
        var searchResult = await _service.SearchBooksAsync(new SearchRequest("Introduction", 1, 10));

        // Assert
        var book = searchResult.Results.First(b => b.BookId == bookId);
        Assert.Equal(newStock, book.StockCount);
    }

    [Fact]
    public async Task AddBookAsync_AddsNewBook()
    {
        // Arrange
        var newBookId = "ISBN-999";
        var title = "New Test Book";
        var condition = "new";

        // Act
        await _service.AddBookAsync(newBookId, title, condition);
        var result = await _service.SearchBooksAsync(new SearchRequest(title, 1, 10));

        // Assert
        Assert.Single(result.Results);
        var book = result.Results.First();
        Assert.Equal(newBookId, book.BookId);
        Assert.Equal(title, book.Title);
        Assert.Equal(condition, book.Condition);
    }
}
