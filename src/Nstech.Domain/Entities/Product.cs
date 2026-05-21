using Nstech.Domain.Common;

namespace Nstech.Domain.Entities;

public sealed class Product : Entity
{
    private Product() { }

    public Product(string productId, decimal unitPrice, int availableQuantity)
    {
        if (string.IsNullOrWhiteSpace(productId)) throw new BusinessException("ProductId is required.", "INVALID_PRODUCT_ID");
        if (unitPrice <= 0) throw new BusinessException("UnitPrice must be greater than zero.", "INVALID_UNIT_PRICE");
        if (availableQuantity < 0) throw new BusinessException("AvailableQuantity cannot be negative.", "INVALID_AVAILABLE_QUANTITY");

        Id = Guid.NewGuid();
        ProductId = productId.Trim();
        UnitPrice = unitPrice;
        AvailableQuantity = availableQuantity;
    }

    public string ProductId { get; private set; } = string.Empty;
    public decimal UnitPrice { get; private set; }
    public int AvailableQuantity { get; private set; }

    public void ReserveStock(int quantity)
    {
        if (quantity <= 0) throw new BusinessException("Quantity must be greater than zero.", "INVALID_QUANTITY");
        if (quantity > AvailableQuantity) throw new BusinessException("Insufficient stock.", "INSUFFICIENT_STOCK");

        AvailableQuantity -= quantity;
    }

    public void ReleaseStock(int quantity)
    {
        if (quantity <= 0) throw new BusinessException("Quantity must be greater than zero.", "INVALID_QUANTITY");
        AvailableQuantity += quantity;
    }
}
