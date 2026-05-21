using Nstech.Api.Contracts;
using Nstech.Application.Abstractions.Messaging;
using Nstech.Application.Common.Models;
using Nstech.Application.Features.Orders.Commands.CancelOrder;
using Nstech.Application.Features.Orders.Commands.ConfirmOrder;
using Nstech.Application.Features.Orders.Commands.CreateOrder;
using Nstech.Application.Features.Orders.Queries.GetOrders;
using Nstech.Domain.Common;
using Nstech.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Nstech.Api.Controllers;

[ApiController]
[Route("orders")]
public sealed class OrdersController : ControllerBase
{
    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Create(
        [FromServices] ICommandHandler<CreateOrderCommand, OrderDto> handler,
        [FromBody] CreateOrderRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var command = new CreateOrderCommand(
                request.CustomerId,
                request.Currency,
                (request.Items ?? Array.Empty<CreateOrderItemRequest>()).Select(x => new CreateOrderItem(x.ProductId, x.Quantity)).ToList());

            var created = await handler.HandleAsync(command, cancellationToken);
            return Created($"/orders/{created.Id}", created);
        }
        catch (BusinessException exception)
        {
            return BadRequest(new { error = new { code = exception.Code, message = exception.Message } });
        }
    }

    [Authorize]
    [HttpPost("{id:guid}/confirm")]
    public async Task<IActionResult> Confirm(
        [FromServices] ICommandHandler<ConfirmOrderCommand, OrderDto> handler,
        Guid id,
        CancellationToken cancellationToken)
    {
        try
        {
            var confirmed = await handler.HandleAsync(new ConfirmOrderCommand(id), cancellationToken);
            return Ok(confirmed);
        }
        catch (BusinessException exception)
        {
            return BadRequest(new { error = new { code = exception.Code, message = exception.Message } });
        }
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> Get(
        [FromServices] IQueryHandler<GetOrdersQuery, OrderListDto> handler,
        [FromQuery] Guid? customerId,
        [FromQuery] OrderStatus? status,
        [FromQuery] DateTime? from,
        [FromQuery] DateTime? to,
        [FromQuery] int? page,
        [FromQuery] int? pageSize,
        CancellationToken cancellationToken)
    {
        if (!page.HasValue || !pageSize.HasValue)
            throw new BusinessException("Pagination parameters page and pageSize are required.", "PAGINATION_REQUIRED");

        var query = new GetOrdersQuery(customerId, status, from, to, page.Value, pageSize.Value);
        var result = await handler.HandleAsync(query, cancellationToken);
        return Ok(result);
    }

    [Authorize]
    [HttpPost("{id:guid}/cancel")]
    public async Task<IActionResult> Cancel(
        [FromServices] ICommandHandler<CancelOrderCommand, OrderDto> handler,
        Guid id,
        CancellationToken cancellationToken)
    {
        try
        {
            var canceled = await handler.HandleAsync(new CancelOrderCommand(id), cancellationToken);
            return Ok(canceled);
        }
        catch (BusinessException exception)
        {
            return BadRequest(new { error = new { code = exception.Code, message = exception.Message } });
        }
    }
}
