using Microsoft.EntityFrameworkCore;
using Nstech.Application.Abstractions.Data;
using Nstech.Application.Abstractions.Messaging;
using Nstech.Application.Common.Models;
using Nstech.Domain.Common;
using Nstech.Domain.Entities;
using Nstech.Domain.Enums;

namespace Nstech.Application.Features.Orders.Commands.CancelOrder;

public sealed class CancelOrderCommandHandler : ICommandHandler<CancelOrderCommand, OrderDto>
{
    private readonly IApplicationDbContext _dbContext;

    public CancelOrderCommandHandler(IApplicationDbContext dbContext) => _dbContext = dbContext;

    public async Task<OrderDto> HandleAsync(CancelOrderCommand command, CancellationToken cancellationToken)
    {
        var order = await _dbContext.Orders
            .Include(o => o.Items)
            .FirstOrDefaultAsync(o => o.Id == command.OrderId, cancellationToken);

        if (order is null)
            throw new BusinessException("Order not found.", "ORDER_NOT_FOUND");

        if (order.Status == OrderStatus.Canceled)
            return MapToDto(order);

        if (order.Status != OrderStatus.Placed && order.Status != OrderStatus.Confirmed)
            throw new BusinessException("Only orders in Placed or Confirmed status can be canceled.", "ORDER_CANNOT_CANCEL");

        if (order.Status == OrderStatus.Confirmed)
        {
            var productIds = order.Items.Select(item => item.ProductId).Distinct().ToList();
            var products = await _dbContext.Products
                .Where(p => productIds.Contains(p.ProductId))
                .ToListAsync(cancellationToken);

            var missingProductIds = productIds.Except(products.Select(p => p.ProductId)).ToList();
            if (missingProductIds.Any())
                throw new BusinessException($"Product(s) not found: {string.Join(", ", missingProductIds)}.", "PRODUCT_NOT_FOUND");

            foreach (var item in order.Items)
            {
                var product = products.Single(p => p.ProductId == item.ProductId);
                product.ReleaseStock(item.Quantity);
            }
        }

        order.Cancel();
        await _dbContext.SaveChangesAsync(cancellationToken);
        return MapToDto(order);
    }

    private static OrderDto MapToDto(Order order) => new(
        order.Id,
        order.CustomerId,
        order.Currency,
        order.Status.ToString(),
        order.Total,
        order.CreatedAt,
        order.Items.Select(item => new OrderItemDto(item.Id, item.ProductId, item.ProductName, item.Quantity, item.UnitPrice, item.LineTotal)).ToList());
}
