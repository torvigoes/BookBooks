using BookBooks.Domain.Common;
using BookBooks.Domain.Interfaces;
using FluentValidation;
using MediatR;

namespace BookBooks.Application.Features.Reviews.Commands;

public record UpdateReviewCommand(
    string ReviewId,
    string UserId,
    int Rating,
    string Content,
    bool ContainsSpoiler
) : IRequest<Result>;

public class UpdateReviewCommandValidator : AbstractValidator<UpdateReviewCommand>
{
    public UpdateReviewCommandValidator()
    {
        RuleFor(x => x.ReviewId).NotEmpty();
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.Rating).InclusiveBetween(1, 5);
        RuleFor(x => x.Content).NotEmpty().MinimumLength(10).MaximumLength(5000);
    }
}

public class UpdateReviewCommandHandler : IRequestHandler<UpdateReviewCommand, Result>
{
    private readonly IReviewRepository _reviewRepository;
    private readonly IBookRepository _bookRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateReviewCommandHandler(
        IReviewRepository reviewRepository,
        IBookRepository bookRepository,
        IUnitOfWork unitOfWork)
    {
        _reviewRepository = reviewRepository;
        _bookRepository = bookRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(UpdateReviewCommand request, CancellationToken cancellationToken)
    {
        var review = await _reviewRepository.GetByIdAsync(request.ReviewId, cancellationToken);
        if (review is null)
        {
            return Result.Failure("Review not found.");
        }

        if (review.UserId != request.UserId)
        {
            return Result.Failure("You can only update your own reviews.");
        }

        var (currentAverage, currentCount) = await _reviewRepository.GetBookRatingStatsAsync(review.BookId, cancellationToken);
        if (currentCount <= 0)
        {
            return Result.Failure("Rating stats are inconsistent for this book.");
        }

        var oldRating = review.Rating;
        review.Update(request.Rating, request.Content, request.ContainsSpoiler);
        _reviewRepository.Update(review);

        var book = await _bookRepository.GetByIdAsync(review.BookId, cancellationToken);
        if (book is null)
        {
            return Result.Failure("Book not found.");
        }

        var newAverage = ((currentAverage * currentCount) - oldRating + request.Rating) / currentCount;
        book.UpdateAverageRating(newAverage, currentCount);
        _bookRepository.Update(book);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}