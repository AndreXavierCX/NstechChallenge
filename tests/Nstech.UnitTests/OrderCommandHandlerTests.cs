using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Nstech.Application.Features.Orders.Commands.CancelOrder;
using Nstech.Application.Features.Orders.Commands.ConfirmOrder;
using Nstech.Application.Features.Orders.Commands.CreateOrder;
using Nstech.Domain.Entities;
using Nstech.Domain.Enums;
using Nstech.Infrastructure.Persistence;
using Xunit;

namespace Nstech.UnitTests;

public sealed class OrderCommandHandlerTests
{
    private static ApplicationDbContext CreateInMemoryContext() =>
        new(new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options);

    [Fact]
    public async Task CreateOrder_Should_Persist_Order_With_Total_And_Placed_Status()
    {
        using var db = CreateInMemoryContext();

        var products = new[]
        {
            new Product("P1", 10m, 5),
            new Product("P2", 20m, 2)
        };

        db.Products.AddRange(products);
        await db.SaveChangesAsync();

        var handler = new CreateOrderCommandHandler(db);
        var customerId = Guid.NewGuid();
        var command = new CreateOrderCommand(
            customerId,
            "BRL",
            new[]
            {
                new CreateOrderItem("P1", 2),
                new CreateOrderItem("P2", 1)
            });

        var result = await handler.HandleAsync(command, CancellationToken.None);

        result.Should().NotBeNull();
        result.CustomerId.Should().Be(customerId);
        result.Status.Should().Be(OrderStatus.Placed.ToString());
        result.Total.Should().Be(40m);
        result.Items.Should().HaveCount(2);
        result.Items.Should().ContainSingle(i => i.ProductId == "P1" && i.Quantity == 2);
        result.Items.Should().ContainSingle(i => i.ProductId == "P2" && i.Quantity == 1);

        db.Orders.Should().HaveCount(1);
    }

    [Fact]
    public async Task CreateOrder_Should_Reject_Missing_Product()
    {
        using var db = CreateInMemoryContext();
        db.Products.Add(new Product("P1", 10m, 5));
        await db.SaveChangesAsync();

        var handler = new CreateOrderCommandHandler(db);
        var command = new CreateOrderCommand(
            Guid.NewGuid(),
            "USD",
            new[] { new CreateOrderItem("UNKNOWN", 1) });

        var act = async () => await handler.HandleAsync(command, CancellationToken.None);

        await act.Should().ThrowAsync<Exception>().Where(ex => ex.Message.Contains("Product(s) not found") || ex.Message.Contains("PRODUCT_NOT_FOUND"));
    }

    [Fact]
    public async Task CreateOrder_Should_Reject_Quantity_Zero()
    {
        using var db = CreateInMemoryContext();
        db.Products.Add(new Product("P1", 10m, 5));
        await db.SaveChangesAsync();

        var handler = new CreateOrderCommandHandler(db);
        var command = new CreateOrderCommand(
            Guid.NewGuid(),
            "USD",
            new[] { new CreateOrderItem("P1", 0) });

        var act = async () => await handler.HandleAsync(command, CancellationToken.None);

        await act.Should().ThrowAsync<Exception>().Where(ex => ex.Message.Contains("quantity must be greater than zero") || ex.Message.Contains("INVALID_ITEM_QUANTITY"));
    }

    [Fact]
    public async Task ConfirmOrder_Should_Reserve_Stock_And_Mark_Confirmed_Once()
    {
        using var db = CreateInMemoryContext();

        var product = new Product("P1", 10m, 5);
        db.Products.Add(product);

        var order = new Order(Guid.NewGuid(), "USD");
        order.AddItem("P1", "P1", 2, 10m);
        order.Place();
        db.Orders.Add(order);
        await db.SaveChangesAsync();

        var handler = new ConfirmOrderCommandHandler(db);

        var firstResult = await handler.HandleAsync(new ConfirmOrderCommand(order.Id), CancellationToken.None);
        firstResult.Status.Should().Be(OrderStatus.Confirmed.ToString());
        product.AvailableQuantity.Should().Be(3);

        var secondResult = await handler.HandleAsync(new ConfirmOrderCommand(order.Id), CancellationToken.None);
        secondResult.Status.Should().Be(OrderStatus.Confirmed.ToString());
        product.AvailableQuantity.Should().Be(3);
    }

    [Fact]
    public async Task CancelOrder_Should_Release_Stock_When_Confirmed_And_Be_Idempotent()
    {
        using var db = CreateInMemoryContext();

        var product = new Product("P1", 10m, 5);
        db.Products.Add(product);

        var order = new Order(Guid.NewGuid(), "USD");
        order.AddItem("P1", "P1", 2, 10m);
        order.Place();
        order.Confirm();
        db.Orders.Add(order);
        product.ReserveStock(2);

        await db.SaveChangesAsync();

        var handler = new CancelOrderCommandHandler(db);

        var result = await handler.HandleAsync(new CancelOrderCommand(order.Id), CancellationToken.None);
        result.Status.Should().Be(OrderStatus.Canceled.ToString());
        product.AvailableQuantity.Should().Be(5);

        var secondResult = await handler.HandleAsync(new CancelOrderCommand(order.Id), CancellationToken.None);
        secondResult.Status.Should().Be(OrderStatus.Canceled.ToString());
        product.AvailableQuantity.Should().Be(5);
    }

    [Fact]
    public async Task CancelOrder_Should_Cancel_Placed_Order_Without_Releasing_Stock()
    {
        using var db = CreateInMemoryContext();

        var product = new Product("P1", 10m, 5);
        db.Products.Add(product);

        var order = new Order(Guid.NewGuid(), "USD");
        order.AddItem("P1", "P1", 2, 10m);
        order.Place();
        db.Orders.Add(order);
        await db.SaveChangesAsync();

        var handler = new CancelOrderCommandHandler(db);
        var result = await handler.HandleAsync(new CancelOrderCommand(order.Id), CancellationToken.None);

        result.Status.Should().Be(OrderStatus.Canceled.ToString());
        product.AvailableQuantity.Should().Be(5);
    }
}
