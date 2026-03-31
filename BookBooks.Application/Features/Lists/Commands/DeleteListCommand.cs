using BookBooks.Domain.Common;
using BookBooks.Domain.Interfaces;
using FluentValidation;
using MediatR;

namespace BookBooks.Application.Features.Lists.Commands;

public sealed record DeleteListCommand(
    string ListId,
    string UserId
) : IRequest<Result>;

public sealed class DeleteListCommandValidator : AbstractValidator<DeleteListCommand>
{
    public DeleteListCommandValidator()
    {
        RuleFor(x => x.ListId).NotEmpty();
        RuleFor(x => x.UserId).NotEmpty();
    }
}

public sealed class DeleteListCommandHandler : IRequestHandler<DeleteListCommand, Result>
{
    private readonly IBookListRepository _bookListRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteListCommandHandler(IBookListRepository bookListRepository, IUnitOfWork unitOfWork)
    {
        _bookListRepository = bookListRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(DeleteListCommand request, CancellationToken cancellationToken)
    {
        var list = await _bookListRepository.GetByIdAsync(request.ListId, cancellationToken);
        if (list is null)
        {
            return Result.Failure("List not found.");
        }

        if (list.UserId != request.UserId)
        {
            return Result.Failure("Forbidden: you can only delete your own lists.");
        }

        _bookListRepository.Delete(list);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
