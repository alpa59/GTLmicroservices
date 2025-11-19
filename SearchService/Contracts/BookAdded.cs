namespace Contracts;

public record BookAdded(
    string Isbn,
    string Title,
    string Author,
    string Publisher,
    DateTime PublicationDate,
    decimal Price,
    string SellerId,
    int StockQuantity,
    DateTime Timestamp
);
