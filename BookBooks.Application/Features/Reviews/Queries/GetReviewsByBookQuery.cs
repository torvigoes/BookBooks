using BookBooks.Application.Features.Reviews.DTOs;
using BookBooks.Domain.Common;
using BookBooks.Domain.Interfaces;
using MediatR;

namespace BookBooks.Application.Features.Reviews.Queries;

public record GetReviewsByBookQuery(
    string BookId,
    int Page = 1,
    int PageSize = 20
) : IRequest<Result<IReadOnlyCollection<ReviewDto>>>;

public class GetReviewsByBookQueryHandler : IRequestHandler<GetReviewsByBookQuery, Result<IReadOnlyCollection<ReviewDto>>>
{
    private readonly IReviewRepository _reviewRepository;

    public GetReviewsByBookQueryHandler(IReviewRepository reviewRepository)
    {
        _reviewRepository = reviewRepository;
    }

    public async Task<Result<IReadOnlyCollection<ReviewDto>>> Handle(GetReviewsByBookQuery request, CancellationToken cancellationToken)
    {
        var reviews = await _reviewRepository.GetByBookIdAsync(request.BookId, request.Page, request.PageSize, cancellationToken);

        var dtos = reviews
            .Select(r => new ReviewDto(
                r.Id,
                r.BookId,
                r.UserId,
                r.User?.UserName ?? string.Empty,
                r.Rating,
                r.Content,
                r.ContainsSpoiler,
                r.CreatedAt))
            .ToList()
            .AsReadOnly();

        return Result<IReadOnlyCollection<ReviewDto>>.Success(dtos);
    }
}