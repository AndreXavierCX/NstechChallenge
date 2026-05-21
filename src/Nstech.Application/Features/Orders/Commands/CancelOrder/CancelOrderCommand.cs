using Nstech.Application.Abstractions.Messaging;
using Nstech.Application.Common.Models;

namespace Nstech.Application.Features.Orders.Commands.CancelOrder;

public sealed record CancelOrderCommand(Guid OrderId) : ICommand<OrderDto>;
