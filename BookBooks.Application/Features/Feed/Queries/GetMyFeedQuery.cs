using BookBooks.Application.Features.Feed.DTOs;
using BookBooks.Domain.Common;
using BookBooks.Domain.Interfaces;
using FluentValidation;
using MediatR;

namespace BookBooks.Application.Features.Feed.Queries;

public sealed record GetMyFeedQuery(
    string UserId,
    int Page = 1,
    int PageSize = 20
) : IRequest<Result<IReadOnlyCollection<FeedItemDto>>>;

public sealed class GetMyFeedQueryValidator : AbstractValidator<GetMyFeedQuery>
{
    public GetMyFeedQueryValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.Page).GreaterThan(0);
        RuleFor(x => x.PageSize).InclusiveBetween(1, 100);
    }
}

public sealed class GetMyFeedQueryHandler : IRequestHandler<GetMyFeedQuery, Result<IReadOnlyCollection<FeedItemDto>>>
{
    private readonly IReviewRepository _reviewRepository;

    public GetMyFeedQueryHandler(IReviewRepository reviewRepository)
    {
        _reviewRepository = reviewRepository;
    }

    public async Task<Result<IReadOnlyCollection<FeedItemDto>>> Handle(GetMyFeedQuery request, CancellationToken cancellationToken)
    {
        var reviews = await _reviewRepository.GetFeedByFollowerIdAsync(
            request.UserId,
            request.Page,
            request.PageSize,
            cancellationToken);

        var items = reviews
            .Select(r => new FeedItemDto(
                r.Id,
                r.BookId,
                r.Book?.Title ?? string.Empty,
                r.Book?.Author ?? string.Empty,
                r.Book?.CoverImageUrl,
                r.UserId,
                r.User?.UserName ?? string.Empty,
                r.Rating,
                r.Content,
                r.ContainsSpoiler,
                r.CreatedAt))
            .ToList()
            .AsReadOnly();

        return Result<IReadOnlyCollection<FeedItemDto>>.Success(items);
    }
}
