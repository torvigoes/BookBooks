using BookBooks.Application.Features.Reviews.DTOs;
using BookBooks.Domain.Common;
using BookBooks.Domain.Entities;
using BookBooks.Domain.Interfaces;
using FluentValidation;
using MediatR;

namespace BookBooks.Application.Features.Reviews.Commands;

public record ToggleReviewLikeCommand(
    string ReviewId,
    string UserId
) : IRequest<Result<ToggleReviewLikeResponse>>;

public class ToggleReviewLikeCommandValidator : AbstractValidator<ToggleReviewLikeCommand>
{
    public ToggleReviewLikeCommandValidator()
    {
        RuleFor(x => x.ReviewId).NotEmpty();
        RuleFor(x => x.UserId).NotEmpty();
    }
}

public class ToggleReviewLikeCommandHandler : IRequestHandler<ToggleReviewLikeCommand, Result<ToggleReviewLikeResponse>>
{
    private readonly IReviewRepository _reviewRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ToggleReviewLikeCommandHandler(IReviewRepository reviewRepository, IUnitOfWork unitOfWork)
    {
        _reviewRepository = reviewRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<ToggleReviewLikeResponse>> Handle(ToggleReviewLikeCommand request, CancellationToken cancellationToken)
    {
        var review = await _reviewRepository.GetByIdAsync(request.ReviewId, cancellationToken);
        if (review is null)
        {
            return Result<ToggleReviewLikeResponse>.Failure("Review not found.");
        }

        var currentLike = await _reviewRepository.GetReviewLikeAsync(request.ReviewId, request.UserId, cancellationToken);

        bool likedByCurrentUser;
        if (currentLike is null)
        {
            await _reviewRepository.AddLikeAsync(new ReviewLike(request.UserId, request.ReviewId), cancellationToken);
            likedByCurrentUser = true;
        }
        else
        {
            _reviewRepository.RemoveLike(currentLike);
            likedByCurrentUser = false;
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var likeCount = await _reviewRepository.GetReviewLikesCountAsync(request.ReviewId, cancellationToken);

        return Result<ToggleReviewLikeResponse>.Success(
            new ToggleReviewLikeResponse(request.ReviewId, likedByCurrentUser, likeCount));
    }
}