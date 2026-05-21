namespace Nstech.Api.Contracts;

public sealed record CreateOrderRequest(Guid CustomerId, string Currency, IReadOnlyCollection<CreateOrderItemRequest> Items);
