using BookBooks.Application.Features.Auth.DTOs;
using BookBooks.Domain.Common;
using BookBooks.Domain.Entities;
using BookBooks.Domain.Interfaces;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace BookBooks.Application.Features.Auth.Commands;

public record RegisterCommand(
    string Username,
    string Email,
    string Password
) : IRequest<Result<AuthResponse>>;

public class RegisterCommandValidator : AbstractValidator<RegisterCommand>
{
    public RegisterCommandValidator()
    {
        RuleFor(x => x.Username).NotEmpty().MinimumLength(3).MaximumLength(50);
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Password).NotEmpty().MinimumLength(6);
    }
}

public class RegisterCommandHandler : IRequestHandler<RegisterCommand, Result<AuthResponse>>
{
    private readonly UserManager<AppUser> _userManager;
    private readonly IJwtProvider _jwtProvider;

    public RegisterCommandHandler(UserManager<AppUser> userManager, IJwtProvider jwtProvider)
    {
        _userManager = userManager;
        _jwtProvider = jwtProvider;
    }

    public async Task<Result<AuthResponse>> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var existingUser = await _userManager.FindByEmailAsync(request.Email);
        if (existingUser is not null)
        {
            return Result<AuthResponse>.Failure("Email is already in use.");
        }

        var existingUsername = await _userManager.FindByNameAsync(request.Username);
        if (existingUsername is not null)
        {
            return Result<AuthResponse>.Failure("Username is already taken.");
        }

        var user = new AppUser(request.Username, request.Email);
        var result = await _userManager.CreateAsync(user, request.Password);

        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            return Result<AuthResponse>.Failure(errors);
        }

        var token = _jwtProvider.Generate(user);

        return Result<AuthResponse>.Success(
            new AuthResponse(user.Id, user.UserName!, user.Email!, token)
        );
    }
}
