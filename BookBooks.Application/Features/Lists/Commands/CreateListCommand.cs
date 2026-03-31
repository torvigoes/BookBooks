using BookBooks.Application.Features.Lists.DTOs;
using BookBooks.Domain.Common;
using BookBooks.Domain.Entities;
using BookBooks.Domain.Enums;
using BookBooks.Domain.Interfaces;
using FluentValidation;
using MediatR;

namespace BookBooks.Application.Features.Lists.Commands;

public sealed record CreateListCommand(
    string UserId,
    string Name,
    string? Description,
    ListVisibility Visibility
) : IRequest<Result<BookListDto>>;

public sealed class CreateListCommandValidator : AbstractValidator<CreateListCommand>
{
    public CreateListCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.Name).NotEmpty().MaximumLength(120);
        RuleFor(x => x.Description).MaximumLength(500);
        RuleFor(x => x.Visibility).IsInEnum();
    }
}

public sealed class CreateListCommandHandler : IRequestHandler<CreateListCommand, Result<BookListDto>>
{
    private readonly IBookListRepository _bookListRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateListCommandHandler(IBookListRepository bookListRepository, IUnitOfWork unitOfWork)
    {
        _bookListRepository = bookListRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<BookListDto>> Handle(CreateListCommand request, CancellationToken cancellationToken)
    {
        var list = new BookList(request.UserId, request.Name, request.Description, request.Visibility);
        await _bookListRepository.AddAsync(list, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<BookListDto>.Success(ListMapping.ToDto(list));
    }
}
