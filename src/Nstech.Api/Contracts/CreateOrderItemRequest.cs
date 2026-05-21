namespace Nstech.Api.Contracts;

public sealed record CreateOrderItemRequest(string ProductId, int Quantity);
