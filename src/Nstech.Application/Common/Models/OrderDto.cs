namespace Nstech.Application.Common.Models;

public sealed record OrderDto(Guid Id, Guid CustomerId, string Currency, string Status, decimal Total, DateTime CreatedAt, IReadOnlyCollection<OrderItemDto> Items);
