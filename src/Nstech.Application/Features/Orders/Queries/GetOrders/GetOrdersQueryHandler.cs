using Microsoft.EntityFrameworkCore;
using Nstech.Application.Abstractions.Data;
using Nstech.Application.Abstractions.Messaging;
using Nstech.Application.Common.Models;
using Nstech.Domain.Common;
using Nstech.Domain.Entities;
using Nstech.Domain.Enums;

namespace Nstech.Application.Features.Orders.Queries.GetOrders;

public sealed class GetOrdersQueryHandler : IQueryHandler<GetOrdersQuery, OrderListDto>
{
    private readonly IApplicationDbContext _dbContext;

    public GetOrdersQueryHandler(IApplicationDbContext dbContext) => _dbContext = dbContext;

    public async Task<OrderListDto> HandleAsync(GetOrdersQuery query, CancellationToken cancellationToken)
    {
        if (query.Page <= 0) throw new BusinessException("Page must be greater than zero.", "INVALID_PAGE");
        if (query.PageSize <= 0) throw new BusinessException("PageSize must be greater than zero.", "INVALID_PAGE_SIZE");

        var dbQuery = _dbContext.Orders
            .AsNoTracking()
            .Include(o => o.Items)
            .AsQueryable();

        if (query.CustomerId.HasValue)
            dbQuery = dbQuery.Where(o => o.CustomerId == query.CustomerId.Value);

        if (query.Status.HasValue)
            dbQuery = dbQuery.Where(o => o.Status == query.Status.Value);

        if (query.From.HasValue)
            dbQuery = dbQuery.Where(o => o.CreatedAt >= query.From.Value);

        if (query.To.HasValue)
            dbQuery = dbQuery.Where(o => o.CreatedAt <= query.To.Value);

        var totalItems = await dbQuery.CountAsync(cancellationToken);
        var totalPages = totalItems == 0 ? 1 : (int)Math.Ceiling(totalItems / (double)query.PageSize);

        var orders = await dbQuery
            .OrderByDescending(o => o.CreatedAt)
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToListAsync(cancellationToken);

        return new OrderListDto(
            orders.Select(MapToDto).ToList(),
            query.Page,
            query.PageSize,
            totalItems,
            totalPages);
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
