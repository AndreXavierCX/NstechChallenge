using Microsoft.EntityFrameworkCore;
using Nstech.Application.Abstractions.Data;
using Nstech.Application.Abstractions.Messaging;
using Nstech.Application.Common.Models;
using Nstech.Domain.Common;
using Nstech.Domain.Entities;

namespace Nstech.Application.Features.Orders.Commands.CreateOrder;

public sealed class CreateOrderCommandHandler : ICommandHandler<CreateOrderCommand, OrderDto>
{
    private readonly IApplicationDbContext _dbContext;

    public CreateOrderCommandHandler(IApplicationDbContext dbContext) => _dbContext = dbContext;

    public async Task<OrderDto> HandleAsync(CreateOrderCommand command, CancellationToken cancellationToken)
    {
        if (command.Items is null || !command.Items.Any())
            throw new BusinessException("Order must contain at least one item.", "ORDER_EMPTY");

        if (command.Items.Any(item => item.Quantity <= 0))
            throw new BusinessException("Each item quantity must be greater than zero.", "INVALID_ITEM_QUANTITY");

        var productIds = command.Items.Select(item => item.ProductId).Distinct().ToList();
        var products = await _dbContext.Products
            .Where(product => productIds.Contains(product.ProductId))
            .ToListAsync(cancellationToken);

        var missingProductIds = productIds.Except(products.Select(p => p.ProductId)).ToList();
        if (missingProductIds.Any())
            throw new BusinessException($"Product(s) not found: {string.Join(", ", missingProductIds)}.", "PRODUCT_NOT_FOUND");

        var order = new Order(command.CustomerId, command.Currency);

        foreach (var item in command.Items)
        {
            var product = products.Single(p => p.ProductId == item.ProductId);
            if (item.Quantity > product.AvailableQuantity)
                throw new BusinessException($"Insufficient stock for product '{item.ProductId}'.", "INSUFFICIENT_STOCK");

            order.AddItem(product.ProductId, product.ProductId, item.Quantity, product.UnitPrice);
        }

        order.Place();
        _dbContext.Orders.Add(order);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return new OrderDto(
            order.Id,
            order.CustomerId,
            order.Currency,
            order.Status.ToString(),
            order.Total,
            order.CreatedAt,
            order.Items.Select(item => new OrderItemDto(item.Id, item.ProductId, item.ProductName, item.Quantity, item.UnitPrice, item.LineTotal)).ToList());
    }
}
