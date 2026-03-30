using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BookBooks.Domain.Entities;
using BookBooks.Domain.Interfaces;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace BookBooks.Infrastructure.Authentication;

internal sealed class JwtProvider : IJwtProvider
{
    private readonly JwtOptions _options;

    public JwtProvider(IOptions<JwtOptions> options)
    {
        _options = options.Value;
    }

    public string Generate(AppUser user)
    {
        var claims = new Claim[]
        {
            new(JwtRegisteredClaimNames.Sub, user.Id),
            new(JwtRegisteredClaimNames.Email, user.Email!),
            new(JwtRegisteredClaimNames.Name, user.UserName!)
            // Add roles or permissions here if needed later
        };

        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.SecretKey));
        var credentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            _options.Issuer,
            _options.Audience,
            claims,
            null,
            DateTime.UtcNow.AddHours(2),
            credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}