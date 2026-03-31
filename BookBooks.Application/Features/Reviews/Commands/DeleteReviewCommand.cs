using BookBooks.Domain.Common;
using BookBooks.Domain.Interfaces;
using FluentValidation;
using MediatR;

namespace BookBooks.Application.Features.Reviews.Commands;

public record DeleteReviewCommand(
    string ReviewId,
    string UserId
) : IRequest<Result>;

public class DeleteReviewCommandValidator : AbstractValidator<DeleteReviewCommand>
{
    public DeleteReviewCommandValidator()
    {
        RuleFor(x => x.ReviewId).NotEmpty();
        RuleFor(x => x.UserId).NotEmpty();
    }
}

public class DeleteReviewCommandHandler : IRequestHandler<DeleteReviewCommand, Result>
{
    private readonly IReviewRepository _reviewRepository;
    private readonly IBookRepository _bookRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteReviewCommandHandler(
        IReviewRepository reviewRepository,
        IBookRepository bookRepository,
        IUnitOfWork unitOfWork)
    {
        _reviewRepository = reviewRepository;
        _bookRepository = bookRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(DeleteReviewCommand request, CancellationToken cancellationToken)
    {
        var review = await _reviewRepository.GetByIdAsync(request.ReviewId, cancellationToken);
        if (review is null)
        {
            return Result.Failure("Review not found.");
        }

        if (review.UserId != request.UserId)
        {
            return Result.Failure("You can only delete your own reviews.");
        }

        var (currentAverage, currentCount) = await _reviewRepository.GetBookRatingStatsAsync(review.BookId, cancellationToken);
        if (currentCount <= 0)
        {
            return Result.Failure("Rating stats are inconsistent for this book.");
        }

        _reviewRepository.Delete(review);

        var book = await _bookRepository.GetByIdAsync(review.BookId, cancellationToken);
        if (book is null)
        {
            return Result.Failure("Book not found.");
        }

        var newCount = currentCount - 1;
        var newAverage = newCount == 0
            ? 0
            : ((currentAverage * currentCount) - review.Rating) / newCount;

        book.UpdateAverageRating(newAverage, newCount);
        _bookRepository.Update(book);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}