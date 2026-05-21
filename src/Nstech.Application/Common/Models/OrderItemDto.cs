namespace Nstech.Application.Common.Models;

public sealed record OrderItemDto(Guid Id, string ProductId, string ProductName, int Quantity, decimal UnitPrice, decimal LineTotal);
