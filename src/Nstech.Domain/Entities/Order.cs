using Nstech.Domain.Common;
using Nstech.Domain.Enums;

namespace Nstech.Domain.Entities;

public sealed class Order : Entity
{
    private readonly List<OrderItem> _items = new();
    private Order() { }

    public Order(Guid customerId, string currency)
    {
        if (customerId == Guid.Empty) throw new BusinessException("CustomerId is required.", "INVALID_CUSTOMER_ID");
        if (string.IsNullOrWhiteSpace(currency)) throw new BusinessException("Currency is required.", "INVALID_CURRENCY");

        Id = Guid.NewGuid();
        CustomerId = customerId;
        Status = OrderStatus.Draft;
        Currency = currency.Trim().ToUpperInvariant();
        CreatedAt = DateTime.UtcNow;
        Total = 0m;
    }

    public Guid CustomerId { get; private set; }
    public OrderStatus Status { get; private set; }
    public string Currency { get; private set; } = string.Empty;
    public decimal Total { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public IReadOnlyCollection<OrderItem> Items => _items.AsReadOnly();

    public void AddItem(string productId, string productName, int quantity, decimal unitPrice)
    {
        var item = new OrderItem(Id, productId, productName, quantity, unitPrice);
        _items.Add(item);
        Total += item.LineTotal;
    }

    public void RemoveItem(Guid itemId)
    {
        var item = _items.FirstOrDefault(x => x.Id == itemId);
        if (item is null) return;

        _items.Remove(item);
        Total -= item.LineTotal;
    }

    public void Place() => Status = OrderStatus.Placed;
    public void Confirm() => Status = OrderStatus.Confirmed;
    public void Cancel() => Status = OrderStatus.Canceled;
}
