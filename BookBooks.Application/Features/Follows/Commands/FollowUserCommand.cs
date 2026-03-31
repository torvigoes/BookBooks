using BookBooks.Domain.Common;
using BookBooks.Domain.Entities;
using BookBooks.Domain.Interfaces;
using FluentValidation;
using MediatR;

namespace BookBooks.Application.Features.Follows.Commands;

public sealed record FollowUserCommand(
    string FollowerId,
    string FollowedId
) : IRequest<Result>;

public sealed class FollowUserCommandValidator : AbstractValidator<FollowUserCommand>
{
    public FollowUserCommandValidator()
    {
        RuleFor(x => x.FollowerId).NotEmpty();
        RuleFor(x => x.FollowedId).NotEmpty();
    }
}

public sealed class FollowUserCommandHandler : IRequestHandler<FollowUserCommand, Result>
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    public FollowUserCommandHandler(IUserRepository userRepository, IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(FollowUserCommand request, CancellationToken cancellationToken)
    {
        if (request.FollowerId == request.FollowedId)
        {
            return Result.Failure("A user cannot follow themselves.");
        }

        var followedUser = await _userRepository.GetByIdAsync(request.FollowedId, cancellationToken);
        if (followedUser is null)
        {
            return Result.Failure("User to follow was not found.");
        }

        var existingFollow = await _userRepository.GetFollowAsync(
            request.FollowerId,
            request.FollowedId,
            cancellationToken);

        if (existingFollow is not null)
        {
            return Result.Failure("You are already following this user.");
        }

        await _userRepository.AddFollowAsync(new UserFollow(request.FollowerId, request.FollowedId), cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
