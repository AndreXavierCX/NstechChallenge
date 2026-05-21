using Nstech.Application.Abstractions.Messaging;
using Nstech.Application.Common.Models;

namespace Nstech.Application.Features.Orders.Commands.ConfirmOrder;

public sealed record ConfirmOrderCommand(Guid OrderId) : ICommand<OrderDto>;
