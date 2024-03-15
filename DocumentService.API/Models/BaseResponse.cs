using Microsoft.AspNetCore.Mvc;

namespace DocumentService.API.Models;

public enum ResponseStatus
{
    Success,
    Fail,
    Error
}

public class BaseResponse
{
    public BaseResponse(ResponseStatus status) => this.Status = status;
    public ResponseStatus? Status { get; set; }
    public string? Base64 { get; set; }
    public string? AuthToken { get; set; }
    public string? Message { get; set; }
    public string? StackTrace { get; set; }
}

public class ModelValidationBadRequest
{
    public static BadRequestObjectResult ModelValidationErrorResponse(ActionContext actionContext)
    {
        return new BadRequestObjectResult(actionContext.ModelState
            .Where(modelError => modelError.Value.Errors.Any())
            .Select(modelError => new BaseResponse(ResponseStatus.Error)
            {
                Message = modelError.Value.Errors.FirstOrDefault().ErrorMessage
            }).FirstOrDefault());
    }
}
