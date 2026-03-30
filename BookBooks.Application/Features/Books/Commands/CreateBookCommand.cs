using BookBooks.Application.Features.Books.DTOs;
using BookBooks.Domain.Common;
using BookBooks.Domain.Entities;
using BookBooks.Domain.Interfaces;
using FluentValidation;
using Mapster;
using MediatR;

namespace BookBooks.Application.Features.Books.Commands;

/// <summary>
/// Command to create a new book.
/// Returns the generated Book ID.
/// </summary>
public record CreateBookCommand(
    string Title, 
    string Author, 
    string Isbn, 
    int Year, 
    string? CoverImageUrl
) : IRequest<Result<string>>;

public class CreateBookCommandValidator : AbstractValidator<CreateBookCommand>
{
    public CreateBookCommandValidator()
    {
        RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Author).NotEmpty().MaximumLength(150);
        RuleFor(x => x.Isbn).NotEmpty().Length(10, 13);
        RuleFor(x => x.Year).GreaterThan(0).LessThanOrEqualTo(DateTime.UtcNow.Year + 1);
    }
}

public class CreateBookCommandHandler : IRequestHandler<CreateBookCommand, Result<string>>
{
    private readonly IBookRepository _bookRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateBookCommandHandler(IBookRepository bookRepository, IUnitOfWork unitOfWork)
    {
        _bookRepository = bookRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<string>> Handle(CreateBookCommand request, CancellationToken cancellationToken)
    {
        // Check for unique ISBN
        var existingBook = await _bookRepository.GetByIsbnAsync(request.Isbn, cancellationToken);
        if (existingBook is not null)
        {
            return Result<string>.Failure("A book with the specified ISBN already exists.");
        }

        var newBook = new Book(
            request.Title,
            request.Author,
            request.Isbn,
            request.Year,
            request.CoverImageUrl
        );

        await _bookRepository.AddAsync(newBook, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<string>.Success(newBook.Id);
    }
}
