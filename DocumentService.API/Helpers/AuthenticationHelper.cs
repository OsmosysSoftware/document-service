using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Security.Claims;

namespace DocumentService.API.Helpers;

public class AuthenticationHelper
{
    // Function to generate non expiry jwt token based on email
    public static string JwtTokenGenerator(string LoginEmail)
    {
        SymmetricSecurityKey secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
            Environment.GetEnvironmentVariable("JWT_KEY") ?? throw new InvalidOperationException("No JWT key specified")
        ));
        SigningCredentials signinCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);

        JwtSecurityToken tokenOptions = new JwtSecurityToken(
            claims: new List<Claim>()
            {
                new(ClaimTypes.Email, LoginEmail),
            },
            signingCredentials: signinCredentials
        );

        return new JwtSecurityTokenHandler().WriteToken(tokenOptions);
    }
}
