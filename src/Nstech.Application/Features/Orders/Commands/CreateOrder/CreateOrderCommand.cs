using Nstech.Application.Abstractions.Messaging;
using Nstech.Application.Common.Models;

namespace Nstech.Application.Features.Orders.Commands.CreateOrder;

public sealed record CreateOrderItem(string ProductId, int Quantity);

public sealed record CreateOrderCommand(Guid CustomerId, string Currency, IReadOnlyCollection<CreateOrderItem> Items) : ICommand<OrderDto>;
