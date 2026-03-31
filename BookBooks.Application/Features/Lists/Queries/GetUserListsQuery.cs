using BookBooks.Application.Features.Lists.DTOs;
using BookBooks.Domain.Common;
using BookBooks.Domain.Interfaces;
using MediatR;

namespace BookBooks.Application.Features.Lists.Queries;

public sealed record GetUserListsQuery(
    string UserId
) : IRequest<Result<IReadOnlyCollection<BookListDto>>>;

public sealed class GetUserListsQueryHandler : IRequestHandler<GetUserListsQuery, Result<IReadOnlyCollection<BookListDto>>>
{
    private readonly IBookListRepository _bookListRepository;

    public GetUserListsQueryHandler(IBookListRepository bookListRepository)
    {
        _bookListRepository = bookListRepository;
    }

    public async Task<Result<IReadOnlyCollection<BookListDto>>> Handle(GetUserListsQuery request, CancellationToken cancellationToken)
    {
        var lists = await _bookListRepository.GetUserListsAsync(request.UserId, cancellationToken);

        var dtos = lists
            .Select(ListMapping.ToDto)
            .ToList()
            .AsReadOnly();

        return Result<IReadOnlyCollection<BookListDto>>.Success(dtos);
    }
}
