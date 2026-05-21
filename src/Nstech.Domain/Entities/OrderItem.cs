using Nstech.Domain.Common;

namespace Nstech.Domain.Entities;

public sealed class OrderItem : Entity
{
    private OrderItem() { }

    internal OrderItem(Guid orderId, string productId, string productName, int quantity, decimal unitPrice)
    {
        if (orderId == Guid.Empty) throw new BusinessException("OrderId is required.", "INVALID_ORDER_ID");
        if (string.IsNullOrWhiteSpace(productId)) throw new BusinessException("ProductId is required.", "INVALID_PRODUCT_ID");
        if (string.IsNullOrWhiteSpace(productName)) throw new BusinessException("ProductName is required.", "INVALID_PRODUCT_NAME");
        if (quantity <= 0) throw new BusinessException("Quantity must be greater than zero.", "INVALID_QUANTITY");
        if (unitPrice <= 0) throw new BusinessException("UnitPrice must be greater than zero.", "INVALID_UNIT_PRICE");

        Id = Guid.NewGuid();
        OrderId = orderId;
        ProductId = productId.Trim();
        ProductName = productName.Trim();
        Quantity = quantity;
        UnitPrice = unitPrice;
        LineTotal = quantity * unitPrice;
    }

    public Guid OrderId { get; private set; }
    public string ProductId { get; private set; } = string.Empty;
    public string ProductName { get; private set; } = string.Empty;
    public int Quantity { get; private set; }
    public decimal UnitPrice { get; private set; }
    public decimal LineTotal { get; private set; }

    public Order? Order { get; private set; }
}
