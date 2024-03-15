using DocumentService.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace DocumentService.API.Controllers;

[Route("api/auth")]
[ApiController]
public class AuthenticationController : ControllerBase
{
    private readonly ILogger<PdfController> _logger;

    public AuthenticationController(ILogger<PdfController> logger)
    {
        this._logger = logger;
    }

    [HttpPost("login")]
    public ActionResult<BaseResponse> Login(LoginRequest request)
    {
        BaseResponse response = new BaseResponse(ResponseStatus.Fail);

        try
        {
            if (request == null)
            {
                throw new BadHttpRequestException("Invalid client request");
            }

            string LoginEmail = Environment.GetEnvironmentVariable("EMAIL") ?? throw new Exception("No EMAIL specified");
            string LoginPassword = Environment.GetEnvironmentVariable("PASSWORD") ?? throw new Exception("No PASSWORD specified");
            if (request.Email != LoginEmail || request.Password != LoginPassword)
            {
                throw new BadHttpRequestException("Invalid credentials");
            }

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

            string tokenString = new JwtSecurityTokenHandler().WriteToken(tokenOptions);
            response.Status = ResponseStatus.Success;
            response.AuthToken = tokenString;
            return this.Ok(response);
        }
        catch (BadHttpRequestException ex)
        {
            response.Status = ResponseStatus.Error;
            response.Message = ex.Message;
            this._logger.LogError(ex.Message);
            this._logger.LogError(ex.StackTrace);
            return this.StatusCode(StatusCodes.Status401Unauthorized, response);
        }
        catch (Exception ex)
        {
            response.Status = ResponseStatus.Error;
            response.Message = ex.Message;
            this._logger.LogError(ex.Message);
            this._logger.LogError(ex.StackTrace);
            return this.StatusCode(StatusCodes.Status500InternalServerError, response);
        }
    }
}
