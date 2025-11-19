namespace SearchService.Events;

// Event contracts for inter-service communication
public record BookAddedEvent(string BookId, string Title, string? Condition = null, int InitialStock = 0);

public record StockChangedEvent(string BookId, int OldCount, int NewCount, DateTimeOffset Timestamp);
