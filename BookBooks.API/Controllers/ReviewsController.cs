using System.Security.Claims;
using BookBooks.Application.Features.Reviews.Commands;
using BookBooks.Application.Features.Reviews.DTOs;
using BookBooks.Application.Features.Reviews.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookBooks.API.Controllers;

[ApiController]
[Route("api")]
public class ReviewsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ReviewsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("books/{bookId}/reviews")]
    [Authorize]
    public async Task<IActionResult> CreateReview(string bookId, [FromBody] CreateReviewRequest request)
    {
        var userId = GetCurrentUserId();

        if (string.IsNullOrWhiteSpace(userId))
        {
            return Unauthorized(new { Error = "User claim not found." });
        }

        var command = new CreateReviewCommand(
            bookId,
            userId,
            request.Rating,
            request.Content,
            request.ContainsSpoiler);

        var result = await _mediator.Send(command);

        if (!result.IsSuccess)
        {
            return BadRequest(new { Error = result.Error });
        }

        return CreatedAtAction(nameof(GetReviewsByBook), new { bookId }, new { reviewId = result.Value });
    }

    [HttpPut("reviews/{reviewId}")]
    [Authorize]
    public async Task<IActionResult> UpdateReview(string reviewId, [FromBody] UpdateReviewRequest request)
    {
        var userId = GetCurrentUserId();

        if (string.IsNullOrWhiteSpace(userId))
        {
            return Unauthorized(new { Error = "User claim not found." });
        }

        var command = new UpdateReviewCommand(
            reviewId,
            userId,
            request.Rating,
            request.Content,
            request.ContainsSpoiler);

        var result = await _mediator.Send(command);

        if (!result.IsSuccess)
        {
            return BadRequest(new { Error = result.Error });
        }

        return NoContent();
    }

    [HttpDelete("reviews/{reviewId}")]
    [Authorize]
    public async Task<IActionResult> DeleteReview(string reviewId)
    {
        var userId = GetCurrentUserId();

        if (string.IsNullOrWhiteSpace(userId))
        {
            return Unauthorized(new { Error = "User claim not found." });
        }

        var command = new DeleteReviewCommand(reviewId, userId);
        var result = await _mediator.Send(command);

        if (!result.IsSuccess)
        {
            return BadRequest(new { Error = result.Error });
        }

        return NoContent();
    }

    [HttpPost("reviews/{reviewId}/like")]
    [Authorize]
    public async Task<IActionResult> ToggleLikeReview(string reviewId)
    {
        var userId = GetCurrentUserId();

        if (string.IsNullOrWhiteSpace(userId))
        {
            return Unauthorized(new { Error = "User claim not found." });
        }

        var command = new ToggleReviewLikeCommand(reviewId, userId);
        var result = await _mediator.Send(command);

        if (!result.IsSuccess)
        {
            return BadRequest(new { Error = result.Error });
        }

        return Ok(result.Value);
    }

    [HttpGet("books/{bookId}/reviews")]
    [AllowAnonymous]
    public async Task<IActionResult> GetReviewsByBook(string bookId, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        var query = new GetReviewsByBookQuery(bookId, page, pageSize);
        var result = await _mediator.Send(query);

        if (!result.IsSuccess)
        {
            return BadRequest(new { Error = result.Error });
        }

        return Ok(result.Value);
    }

    private string? GetCurrentUserId()
    {
        return User.FindFirstValue(ClaimTypes.NameIdentifier)
               ?? User.FindFirstValue("sub");
    }
}