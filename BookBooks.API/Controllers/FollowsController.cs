using System.Security.Claims;
using BookBooks.API.Common;
using BookBooks.Application.Features.Follows.Commands;
using BookBooks.Application.Features.Follows.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookBooks.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public sealed class FollowsController : ControllerBase
{
    private readonly IMediator _mediator;

    public FollowsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("me")]
    public async Task<IActionResult> GetMyFollowing()
    {
        var userId = GetCurrentUserId();
        if (string.IsNullOrWhiteSpace(userId))
        {
            return this.ToFailureActionResult("User claim not found.");
        }

        var result = await _mediator.Send(new GetMyFollowingQuery(userId));
        return this.ToActionResult(result, Ok);
    }

    [HttpPost("{followedUserId}")]
    public async Task<IActionResult> Follow(string followedUserId)
    {
        var userId = GetCurrentUserId();
        if (string.IsNullOrWhiteSpace(userId))
        {
            return this.ToFailureActionResult("User claim not found.");
        }

        var result = await _mediator.Send(new FollowUserCommand(userId, followedUserId));
        return this.ToActionResult(result, NoContent);
    }

    [HttpDelete("{followedUserId}")]
    public async Task<IActionResult> Unfollow(string followedUserId)
    {
        var userId = GetCurrentUserId();
        if (string.IsNullOrWhiteSpace(userId))
        {
            return this.ToFailureActionResult("User claim not found.");
        }

        var result = await _mediator.Send(new UnfollowUserCommand(userId, followedUserId));
        return this.ToActionResult(result, NoContent);
    }

    private string? GetCurrentUserId()
    {
        return User.FindFirstValue(ClaimTypes.NameIdentifier)
               ?? User.FindFirstValue("sub");
    }
}
