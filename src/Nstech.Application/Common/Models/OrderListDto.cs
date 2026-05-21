namespace Nstech.Application.Common.Models;

public sealed record OrderListDto(
    IReadOnlyCollection<OrderDto> Orders,
    int Page,
    int PageSize,
    int TotalItems,
    int TotalPages);
