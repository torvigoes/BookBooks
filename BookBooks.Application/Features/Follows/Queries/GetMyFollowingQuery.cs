using BookBooks.Application.Features.Follows.DTOs;
using BookBooks.Domain.Common;
using BookBooks.Domain.Interfaces;
using MediatR;

namespace BookBooks.Application.Features.Follows.Queries;

public sealed record GetMyFollowingQuery(
    string FollowerId
) : IRequest<Result<IReadOnlyCollection<FollowedUserDto>>>;

public sealed class GetMyFollowingQueryHandler : IRequestHandler<GetMyFollowingQuery, Result<IReadOnlyCollection<FollowedUserDto>>>
{
    private readonly IUserRepository _userRepository;

    public GetMyFollowingQueryHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<Result<IReadOnlyCollection<FollowedUserDto>>> Handle(GetMyFollowingQuery request, CancellationToken cancellationToken)
    {
        var follows = await _userRepository.GetFollowedByUserAsync(request.FollowerId, cancellationToken);
        var items = follows
            .Select(f => new FollowedUserDto(
                f.FollowedId,
                f.Followed?.UserName ?? string.Empty,
                f.FollowedAt))
            .ToList()
            .AsReadOnly();

        return Result<IReadOnlyCollection<FollowedUserDto>>.Success(items);
    }
}
