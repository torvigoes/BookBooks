using BookBooks.Application.Features.Books.Commands;
using BookBooks.Application.Features.Books.Queries;
using BookBooks.API.Common;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BookBooks.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BooksController : ControllerBase
{
    private readonly IMediator _mediator;

    public BooksController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> CreateBook([FromBody] CreateBookCommand command)
    {
        var result = await _mediator.Send(command);
        return this.ToActionResult(result, id => CreatedAtAction(nameof(GetBookById), new { id }, id));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetBookById(string id)
    {
        var query = new GetBookByIdQuery(id);
        var result = await _mediator.Send(query);
        return this.ToActionResult(result, Ok);
    }
}
