using Nstech.Application.Abstractions.Messaging;
using Nstech.Domain.Enums;
using Nstech.Application.Common.Models;

namespace Nstech.Application.Features.Orders.Queries.GetOrders;

public sealed record GetOrdersQuery(
    Guid? CustomerId,
    OrderStatus? Status,
    DateTime? From,
    DateTime? To,
    int Page,
    int PageSize) : IQuery<OrderListDto>;
