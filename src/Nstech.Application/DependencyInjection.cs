using Nstech.Application.Abstractions.Messaging;
using Nstech.Application.Common.Models;
using Nstech.Application.Features.Orders.Commands.CancelOrder;
using Nstech.Application.Features.Orders.Commands.ConfirmOrder;
using Nstech.Application.Features.Orders.Commands.CreateOrder;
using Nstech.Application.Features.Orders.Queries.GetOrders;
using Microsoft.Extensions.DependencyInjection;

namespace Nstech.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddNstechApplication(this IServiceCollection services)
    {
        services.AddScoped<ICommandHandler<CreateOrderCommand, OrderDto>, CreateOrderCommandHandler>();
        services.AddScoped<ICommandHandler<ConfirmOrderCommand, OrderDto>, ConfirmOrderCommandHandler>();
        services.AddScoped<ICommandHandler<CancelOrderCommand, OrderDto>, CancelOrderCommandHandler>();
        services.AddScoped<IQueryHandler<GetOrdersQuery, OrderListDto>, GetOrdersQueryHandler>();
        return services;
    }
}
