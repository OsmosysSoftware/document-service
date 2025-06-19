using Microsoft.AspNetCore.Mvc;
using OsmoDoc.API.Models;
using OsmoDoc.API.Helpers;
using Microsoft.AspNetCore.Authorization;

namespace OsmoDoc.API.Controllers;

[Route("api")]
[ApiController]
public class LoginController : ControllerBase
{
    private readonly ILogger<LoginController> _logger;

    public LoginController(ILogger<LoginController> logger)
    {
        this._logger = logger;
    }

    [HttpPost]
    [Route("login")]
    [AllowAnonymous]
    public ActionResult<BaseResponse> Login([FromBody] LoginRequestDTO loginRequest)
    {
        BaseResponse response = new BaseResponse(ResponseStatus.Fail);
        try
        {
            string token = AuthenticationHelper.JwtTokenGenerator(loginRequest.Email);

            response.Status = ResponseStatus.Success;
            response.AuthToken = token;
            response.Message = "Token generated successfully";
            return this.Ok(response);
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