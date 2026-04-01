using System.Security.Claims;
using BookBooks.API.Common;
using BookBooks.Application.Features.Feed.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookBooks.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public sealed class FeedController : ControllerBase
{
    private readonly IMediator _mediator;

    public FeedController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetMyFeed([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        var userId = GetCurrentUserId();
        if (string.IsNullOrWhiteSpace(userId))
        {
            return this.ToFailureActionResult("User claim not found.");
        }

        var result = await _mediator.Send(new GetMyFeedQuery(userId, page, pageSize));
        return this.ToActionResult(result, Ok);
    }

    private string? GetCurrentUserId()
    {
        return User.FindFirstValue(ClaimTypes.NameIdentifier)
               ?? User.FindFirstValue("sub");
    }
}
