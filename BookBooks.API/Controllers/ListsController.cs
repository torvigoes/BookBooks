using System.Security.Claims;
using BookBooks.API.Common;
using BookBooks.Application.Features.Lists.Commands;
using BookBooks.Application.Features.Lists.DTOs;
using BookBooks.Application.Features.Lists.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookBooks.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public sealed class ListsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ListsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateListRequest request)
    {
        var userId = GetCurrentUserId();
        if (string.IsNullOrWhiteSpace(userId))
        {
            return this.ToFailureActionResult("User claim not found.");
        }

        var command = new CreateListCommand(userId, request.Name, request.Description, request.Visibility);
        var result = await _mediator.Send(command);
        return this.ToActionResult(result, list => CreatedAtAction(nameof(GetById), new { listId = list.Id }, list));
    }

    [HttpGet]
    public async Task<IActionResult> GetMine()
    {
        var userId = GetCurrentUserId();
        if (string.IsNullOrWhiteSpace(userId))
        {
            return this.ToFailureActionResult("User claim not found.");
        }

        var result = await _mediator.Send(new GetUserListsQuery(userId));
        return this.ToActionResult(result, Ok);
    }

    [HttpGet("{listId}")]
    public async Task<IActionResult> GetById(string listId)
    {
        var userId = GetCurrentUserId();
        if (string.IsNullOrWhiteSpace(userId))
        {
            return this.ToFailureActionResult("User claim not found.");
        }

        var result = await _mediator.Send(new GetListByIdQuery(listId, userId));
        return this.ToActionResult(result, Ok);
    }

    [HttpPost("{listId}/books")]
    public async Task<IActionResult> AddBook(string listId, [FromBody] AddBookToListRequest request)
    {
        var userId = GetCurrentUserId();
        if (string.IsNullOrWhiteSpace(userId))
        {
            return this.ToFailureActionResult("User claim not found.");
        }

        var command = new AddBookToListCommand(listId, userId, request.BookId, request.Notes);
        var result = await _mediator.Send(command);
        return this.ToActionResult(result, Ok);
    }

    [HttpDelete("{listId}/books/{bookId}")]
    public async Task<IActionResult> RemoveBook(string listId, string bookId)
    {
        var userId = GetCurrentUserId();
        if (string.IsNullOrWhiteSpace(userId))
        {
            return this.ToFailureActionResult("User claim not found.");
        }

        var command = new RemoveBookFromListCommand(listId, userId, bookId);
        var result = await _mediator.Send(command);
        return this.ToActionResult(result, Ok);
    }

    [HttpDelete("{listId}")]
    public async Task<IActionResult> Delete(string listId)
    {
        var userId = GetCurrentUserId();
        if (string.IsNullOrWhiteSpace(userId))
        {
            return this.ToFailureActionResult("User claim not found.");
        }

        var result = await _mediator.Send(new DeleteListCommand(listId, userId));
        return this.ToActionResult(result, NoContent);
    }

    private string? GetCurrentUserId()
    {
        return User.FindFirstValue(ClaimTypes.NameIdentifier)
               ?? User.FindFirstValue("sub");
    }
}
