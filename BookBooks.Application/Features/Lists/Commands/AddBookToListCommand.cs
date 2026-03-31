using BookBooks.Application.Features.Lists.DTOs;
using BookBooks.Domain.Common;
using BookBooks.Domain.Entities;
using BookBooks.Domain.Interfaces;
using FluentValidation;
using MediatR;

namespace BookBooks.Application.Features.Lists.Commands;

public sealed record AddBookToListCommand(
    string ListId,
    string UserId,
    string BookId,
    string? Notes
) : IRequest<Result<BookListDto>>;

public sealed class AddBookToListCommandValidator : AbstractValidator<AddBookToListCommand>
{
    public AddBookToListCommandValidator()
    {
        RuleFor(x => x.ListId).NotEmpty();
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.BookId).NotEmpty();
        RuleFor(x => x.Notes).MaximumLength(500);
    }
}

public sealed class AddBookToListCommandHandler : IRequestHandler<AddBookToListCommand, Result<BookListDto>>
{
    private readonly IBookListRepository _bookListRepository;
    private readonly IBookRepository _bookRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AddBookToListCommandHandler(
        IBookListRepository bookListRepository,
        IBookRepository bookRepository,
        IUnitOfWork unitOfWork)
    {
        _bookListRepository = bookListRepository;
        _bookRepository = bookRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<BookListDto>> Handle(AddBookToListCommand request, CancellationToken cancellationToken)
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

        var book = await _bookRepository.GetByIdAsync(request.BookId, cancellationToken);
        if (book is null)
        {
            return Result<BookListDto>.Failure("Book not found.");
        }

        var existingItem = await _bookListRepository.GetItemAsync(request.ListId, request.BookId, cancellationToken);
        if (existingItem is not null)
        {
            return Result<BookListDto>.Failure("This book is already in the list.");
        }

        var nextOrder = await _bookListRepository.CountItemsAsync(request.ListId, cancellationToken) + 1;
        await _bookListRepository.AddItemAsync(
            new BookListItem(request.ListId, request.BookId, nextOrder, request.Notes),
            cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var updatedList = await _bookListRepository.GetByIdAsync(request.ListId, cancellationToken);
        if (updatedList is null)
        {
            return Result<BookListDto>.Failure("List not found.");
        }

        return Result<BookListDto>.Success(ListMapping.ToDto(updatedList));
    }
}
