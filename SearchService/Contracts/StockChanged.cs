namespace Contracts;

public record StockChanged(
    string Isbn,
    int NewStockQuantity,
    string ChangedBy,
    DateTime Timestamp
);
