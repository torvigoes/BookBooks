using BookBooks.Domain.Common;
using BookBooks.Domain.Entities;
using BookBooks.Domain.Interfaces;
using FluentValidation;
using MediatR;

namespace BookBooks.Application.Features.Reviews.Commands;

public record CreateReviewCommand(
    string BookId,
    string UserId,
    int Rating,
    string Content,
    bool ContainsSpoiler
) : IRequest<Result<string>>;

public class CreateReviewCommandValidator : AbstractValidator<CreateReviewCommand>
{
    public CreateReviewCommandValidator()
    {
        RuleFor(x => x.BookId).NotEmpty();
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.Rating).InclusiveBetween(1, 5);
        RuleFor(x => x.Content).NotEmpty().MinimumLength(10).MaximumLength(5000);
    }
}

public class CreateReviewCommandHandler : IRequestHandler<CreateReviewCommand, Result<string>>
{
    private readonly IBookRepository _bookRepository;
    private readonly IReviewRepository _reviewRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateReviewCommandHandler(
        IBookRepository bookRepository,
        IReviewRepository reviewRepository,
        IUnitOfWork unitOfWork)
    {
        _bookRepository = bookRepository;
        _reviewRepository = reviewRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<string>> Handle(CreateReviewCommand request, CancellationToken cancellationToken)
    {
        var book = await _bookRepository.GetByIdAsync(request.BookId, cancellationToken);
        if (book is null)
        {
            return Result<string>.Failure("Book not found.");
        }

        var existingReview = await _reviewRepository.GetUserReviewForBookAsync(request.UserId, request.BookId, cancellationToken);
        if (existingReview is not null)
        {
            return Result<string>.Failure("You already reviewed this book.");
        }

        var (currentAverage, currentCount) = await _reviewRepository.GetBookRatingStatsAsync(request.BookId, cancellationToken);

        var review = new Review(request.BookId, request.UserId, request.Rating, request.Content, request.ContainsSpoiler);
        await _reviewRepository.AddAsync(review, cancellationToken);

        var newCount = currentCount + 1;
        var newAverage = ((currentAverage * currentCount) + request.Rating) / newCount;
        book.UpdateAverageRating(newAverage, newCount);
        _bookRepository.Update(book);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result<string>.Success(review.Id);
    }
}