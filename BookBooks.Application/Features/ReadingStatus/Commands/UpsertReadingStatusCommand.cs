using BookBooks.Application.Features.ReadingStatus.DTOs;
using BookBooks.Domain.Common;
using BookBooks.Domain.Enums;
using BookBooks.Domain.Interfaces;
using FluentValidation;
using MediatR;
using DomainReadingStatus = BookBooks.Domain.Entities.ReadingStatus;

namespace BookBooks.Application.Features.ReadingStatus.Commands;

public sealed record UpsertReadingStatusCommand(
    string BookId,
    string UserId,
    ReadingStatusType Status
) : IRequest<Result<ReadingStatusDto>>;

public sealed class UpsertReadingStatusCommandValidator : AbstractValidator<UpsertReadingStatusCommand>
{
    public UpsertReadingStatusCommandValidator()
    {
        RuleFor(x => x.BookId).NotEmpty();
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.Status).IsInEnum();
    }
}

public sealed class UpsertReadingStatusCommandHandler : IRequestHandler<UpsertReadingStatusCommand, Result<ReadingStatusDto>>
{
    private readonly IBookRepository _bookRepository;
    private readonly IReadingStatusRepository _readingStatusRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpsertReadingStatusCommandHandler(
        IBookRepository bookRepository,
        IReadingStatusRepository readingStatusRepository,
        IUnitOfWork unitOfWork)
    {
        _bookRepository = bookRepository;
        _readingStatusRepository = readingStatusRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<ReadingStatusDto>> Handle(UpsertReadingStatusCommand request, CancellationToken cancellationToken)
    {
        var book = await _bookRepository.GetByIdAsync(request.BookId, cancellationToken);
        if (book is null)
        {
            return Result<ReadingStatusDto>.Failure("Book not found.");
        }

        var readingStatus = await _readingStatusRepository.GetByUserAndBookAsync(
            request.UserId,
            request.BookId,
            cancellationToken);

        if (readingStatus is null)
        {
            readingStatus = new DomainReadingStatus(request.UserId, request.BookId, request.Status);
            await _readingStatusRepository.AddAsync(readingStatus, cancellationToken);
        }
        else
        {
            readingStatus.UpdateStatus(request.Status);
            _readingStatusRepository.Update(readingStatus);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var dto = new ReadingStatusDto(
            readingStatus.BookId,
            readingStatus.UserId,
            readingStatus.Status,
            readingStatus.UpdatedAt);

        return Result<ReadingStatusDto>.Success(dto);
    }
}
