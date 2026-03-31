using BookBooks.Application.Features.ReadingStatus.DTOs;
using BookBooks.Domain.Common;
using BookBooks.Domain.Interfaces;
using MediatR;

namespace BookBooks.Application.Features.ReadingStatus.Queries;

public sealed record GetReadingStatusByBookQuery(
    string BookId,
    string UserId
) : IRequest<Result<ReadingStatusDto>>;

public sealed class GetReadingStatusByBookQueryHandler : IRequestHandler<GetReadingStatusByBookQuery, Result<ReadingStatusDto>>
{
    private readonly IReadingStatusRepository _readingStatusRepository;

    public GetReadingStatusByBookQueryHandler(IReadingStatusRepository readingStatusRepository)
    {
        _readingStatusRepository = readingStatusRepository;
    }

    public async Task<Result<ReadingStatusDto>> Handle(GetReadingStatusByBookQuery request, CancellationToken cancellationToken)
    {
        var readingStatus = await _readingStatusRepository.GetByUserAndBookAsync(
            request.UserId,
            request.BookId,
            cancellationToken);

        if (readingStatus is null)
        {
            return Result<ReadingStatusDto>.Failure("Reading status not found.");
        }

        var dto = new ReadingStatusDto(
            readingStatus.BookId,
            readingStatus.UserId,
            readingStatus.Status,
            readingStatus.UpdatedAt);

        return Result<ReadingStatusDto>.Success(dto);
    }
}
