using BookBooks.Domain.Common;
using BookBooks.Domain.Interfaces;
using FluentValidation;
using MediatR;

namespace BookBooks.Application.Features.Follows.Commands;

public sealed record UnfollowUserCommand(
    string FollowerId,
    string FollowedId
) : IRequest<Result>;

public sealed class UnfollowUserCommandValidator : AbstractValidator<UnfollowUserCommand>
{
    public UnfollowUserCommandValidator()
    {
        RuleFor(x => x.FollowerId).NotEmpty();
        RuleFor(x => x.FollowedId).NotEmpty();
    }
}

public sealed class UnfollowUserCommandHandler : IRequestHandler<UnfollowUserCommand, Result>
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UnfollowUserCommandHandler(IUserRepository userRepository, IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(UnfollowUserCommand request, CancellationToken cancellationToken)
    {
        var follow = await _userRepository.GetFollowAsync(
            request.FollowerId,
            request.FollowedId,
            cancellationToken);

        if (follow is null)
        {
            return Result.Failure("Follow relationship not found.");
        }

        _userRepository.RemoveFollow(follow);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
