using BookBooks.Application.Features.Books.DTOs;
using BookBooks.Domain.Common;
using BookBooks.Domain.Interfaces;
using Mapster;
using MediatR;

namespace BookBooks.Application.Features.Books.Queries;

/// <summary>
/// Query to retrieve a book by its identifier.
/// </summary>
public record GetBookByIdQuery(string Id) : IRequest<Result<BookDto>>;

public class GetBookByIdQueryHandler : IRequestHandler<GetBookByIdQuery, Result<BookDto>>
{
    private readonly IBookRepository _bookRepository;

    public GetBookByIdQueryHandler(IBookRepository bookRepository)
    {
        _bookRepository = bookRepository;
    }

    public async Task<Result<BookDto>> Handle(GetBookByIdQuery request, CancellationToken cancellationToken)
    {
        var book = await _bookRepository.GetByIdAsync(request.Id, cancellationToken);
        if (book is null)
        {
            return Result<BookDto>.Failure($"Book with ID {request.Id} was not found.");
        }

        var dto = book.Adapt<BookDto>();
        return Result<BookDto>.Success(dto);
    }
}
