using DocumentServiceWebAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Serilog.Events;
using Serilog;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Server.Kestrel.Core;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Controller Services
builder.Services.AddControllers(options => options.Filters.Add(new ProducesAttribute("application/json")))
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

// Configure request size limit
long requestBodySizeLimitBytes = Convert.ToInt64(builder.Configuration.GetSection("CONFIG:REQUEST_BODY_SIZE_LIMIT_BYTES").Value);

// Configure request size for Kestrel server - ASP.NET Core project templates use Kestrel by default when not hosted with IIS
builder.Services.Configure<KestrelServerOptions>(options =>
{
    options.Limits.MaxRequestBodySize = requestBodySizeLimitBytes;
});

// Configure request size for IIS server
builder.Services.Configure<IISServerOptions>(options =>
{
    options.MaxRequestBodySize = requestBodySizeLimitBytes;
});

// AutoMapper Services
builder.Services.AddAutoMapper(typeof(Program));

// Swagger UI Services
builder.Services.AddSwaggerGen();

// Configure Error Response from Model Validations
builder.Services.AddMvc().ConfigureApiBehaviorOptions(options =>
{
    options.InvalidModelStateResponseFactory = actionContext =>
    {
        return ModelValidationBadRequest.ModelValidationErrorResponse(actionContext);
    };
});

// Logging service Serilogs
builder.Logging.AddSerilog();
Log.Logger = new LoggerConfiguration()
    .WriteTo.File(
    path: "wwwroot/logs/log-.txt",
    outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}{NewLine}{NewLine}",
    rollingInterval: RollingInterval.Day,
    restrictedToMinimumLevel: LogEventLevel.Information
    ).CreateLogger();

WebApplication app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseStaticFiles();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
