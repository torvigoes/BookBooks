using System.Security.Claims;
using BookBooks.API.Common;
using BookBooks.Application.Features.ReadingStatus.Commands;
using BookBooks.Application.Features.ReadingStatus.DTOs;
using BookBooks.Application.Features.ReadingStatus.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookBooks.API.Controllers;

[ApiController]
[Route("api/books/{bookId}/reading-status")]
[Authorize]
public sealed class ReadingStatusesController : ControllerBase
{
    private readonly IMediator _mediator;

    public ReadingStatusesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetByBook(string bookId)
    {
        var userId = GetCurrentUserId();
        if (string.IsNullOrWhiteSpace(userId))
        {
            return this.ToFailureActionResult("User claim not found.");
        }

        var result = await _mediator.Send(new GetReadingStatusByBookQuery(bookId, userId));
        return this.ToActionResult(result, Ok);
    }

    [HttpPut]
    public async Task<IActionResult> Upsert(string bookId, [FromBody] UpdateReadingStatusRequest request)
    {
        var userId = GetCurrentUserId();
        if (string.IsNullOrWhiteSpace(userId))
        {
            return this.ToFailureActionResult("User claim not found.");
        }

        var command = new UpsertReadingStatusCommand(bookId, userId, request.Status);
        var result = await _mediator.Send(command);
        return this.ToActionResult(result, Ok);
    }

    private string? GetCurrentUserId()
    {
        return User.FindFirstValue(ClaimTypes.NameIdentifier)
               ?? User.FindFirstValue("sub");
    }
}
