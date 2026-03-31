using BookBooks.Application.Features.Books.DTOs;
using BookBooks.Domain.Common;
using BookBooks.Domain.Interfaces;
using Mapster;
using MediatR;
using FluentValidation;

namespace BookBooks.Application.Features.Books.Queries;

public sealed record SearchBooksQuery(
    string SearchTerm = "",
    int Page = 1,
    int PageSize = 20
) : IRequest<Result<IReadOnlyCollection<BookDto>>>;

public sealed class SearchBooksQueryValidator : AbstractValidator<SearchBooksQuery>
{
    public SearchBooksQueryValidator()
    {
        RuleFor(x => x.Page).GreaterThan(0);
        RuleFor(x => x.PageSize).InclusiveBetween(1, 100);
    }
}

public sealed class SearchBooksQueryHandler : IRequestHandler<SearchBooksQuery, Result<IReadOnlyCollection<BookDto>>>
{
    private readonly IBookRepository _bookRepository;

    public SearchBooksQueryHandler(IBookRepository bookRepository)
    {
        _bookRepository = bookRepository;
    }

    public async Task<Result<IReadOnlyCollection<BookDto>>> Handle(SearchBooksQuery request, CancellationToken cancellationToken)
    {
        var books = await _bookRepository.SearchAsync(
            request.SearchTerm,
            request.Page,
            request.PageSize,
            cancellationToken);

        var dtos = books
            .Select(b => b.Adapt<BookDto>())
            .ToList()
            .AsReadOnly();

        return Result<IReadOnlyCollection<BookDto>>.Success(dtos);
    }
}
