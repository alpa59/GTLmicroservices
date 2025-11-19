using Microsoft.AspNetCore.Mvc;
using SearchService.Models;

namespace SearchService.Controllers;

[ApiController]
[Route("api/search")]
[Produces("application/json")]
public class SearchController : ControllerBase
{
    private readonly ISearchService _searchService;
    private readonly ILogger<SearchController> _logger;

    public SearchController(ISearchService searchService, ILogger<SearchController> logger)
    {
        _searchService = searchService;
        _logger = logger;
    }

    /// <summary>
    /// Search for books by title or keyword
    /// </summary>
    /// <param name="request">Search parameters including query, page, and page size</param>
    /// <returns>Paginated search results with book details and stock counts</returns>
    /// <response code="200">Returns the search results</response>
    /// <response code="400">If the request is invalid</response>
    [HttpGet("books")]
    [ProducesResponseType(typeof(SearchResponse), 200)]
    [ProducesResponseType(400)]
    public async Task<ActionResult<SearchResponse>> SearchBooks(
        [FromQuery] string? query,
        [FromQuery] int page = 1,
        [FromQuery] int size = 20)
    {
        if (size <= 0 || size > 100)
            return BadRequest("Page size must be between 1 and 100");

        if (page < 1)
            return BadRequest("Page must be greater than 0");

        var request = new SearchRequest(query, page, size);

        _logger.LogInformation("Searching books with query '{Query}' (page {Page}, size {Size})",
            query, page, size);

        var response = await _searchService.SearchBooksAsync(request);

        return Ok(response);
    }
}
