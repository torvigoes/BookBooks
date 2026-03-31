using BookBooks.Application.Features.Lists.DTOs;
using BookBooks.Domain.Common;
using BookBooks.Domain.Interfaces;
using FluentValidation;
using MediatR;

namespace BookBooks.Application.Features.Lists.Commands;

public sealed record RemoveBookFromListCommand(
    string ListId,
    string UserId,
    string BookId
) : IRequest<Result<BookListDto>>;

public sealed class RemoveBookFromListCommandValidator : AbstractValidator<RemoveBookFromListCommand>
{
    public RemoveBookFromListCommandValidator()
    {
        RuleFor(x => x.ListId).NotEmpty();
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.BookId).NotEmpty();
    }
}

public sealed class RemoveBookFromListCommandHandler : IRequestHandler<RemoveBookFromListCommand, Result<BookListDto>>
{
    private readonly IBookListRepository _bookListRepository;
    private readonly IUnitOfWork _unitOfWork;

    public RemoveBookFromListCommandHandler(IBookListRepository bookListRepository, IUnitOfWork unitOfWork)
    {
        _bookListRepository = bookListRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<BookListDto>> Handle(RemoveBookFromListCommand request, CancellationToken cancellationToken)
    {
        var list = await _bookListRepository.GetByIdAsync(request.ListId, cancellationToken);
        if (list is null)
        {
            return Result<BookListDto>.Failure("List not found.");
        }

        if (list.UserId != request.UserId)
        {
            return Result<BookListDto>.Failure("Forbidden: you can only change your own lists.");
        }

        var item = await _bookListRepository.GetItemAsync(request.ListId, request.BookId, cancellationToken);
        if (item is null)
        {
            return Result<BookListDto>.Failure("Book is not in this list.");
        }

        _bookListRepository.RemoveItem(item);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var updatedList = await _bookListRepository.GetByIdAsync(request.ListId, cancellationToken);
        if (updatedList is null)
        {
            return Result<BookListDto>.Failure("List not found.");
        }

        return Result<BookListDto>.Success(ListMapping.ToDto(updatedList));
    }
}
