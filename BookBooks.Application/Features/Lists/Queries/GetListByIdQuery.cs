using BookBooks.Application.Features.Lists.DTOs;
using BookBooks.Domain.Common;
using BookBooks.Domain.Interfaces;
using MediatR;

namespace BookBooks.Application.Features.Lists.Queries;

public sealed record GetListByIdQuery(
    string ListId,
    string CurrentUserId
) : IRequest<Result<BookListDto>>;

public sealed class GetListByIdQueryHandler : IRequestHandler<GetListByIdQuery, Result<BookListDto>>
{
    private readonly IBookListRepository _bookListRepository;

    public GetListByIdQueryHandler(IBookListRepository bookListRepository)
    {
        _bookListRepository = bookListRepository;
    }

    public async Task<Result<BookListDto>> Handle(GetListByIdQuery request, CancellationToken cancellationToken)
    {
        var list = await _bookListRepository.GetByIdAsync(request.ListId, cancellationToken);
        if (list is null)
        {
            return Result<BookListDto>.Failure("List not found.");
        }

        if (list.Visibility == Domain.Enums.ListVisibility.Private && list.UserId != request.CurrentUserId)
        {
            return Result<BookListDto>.Failure("Forbidden: this list is private.");
        }

        return Result<BookListDto>.Success(ListMapping.ToDto(list));
    }
}
