using DocumentServiceWebAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Serilog.Events;
using Serilog;
using System.Text.Json.Serialization;
using System.Threading.RateLimiting;
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
builder.Services.AddSwaggerGen(c =>
{
    c.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());
    c.IgnoreObsoleteActions();
    c.IgnoreObsoleteProperties();
    c.CustomSchemaIds(type => type.FullName);
});

// Setup Rate limiting service
int permitLimit = Convert.ToInt32(builder.Configuration.GetSection("RATE_LIMITING:PERMIT_LIMIT").Value);
int queueLimit = Convert.ToInt32(builder.Configuration.GetSection("RATE_LIMITING:QUEUE_LIMIT").Value);
int windowTimeLimitSeconds = Convert.ToInt32(builder.Configuration.GetSection("RATE_LIMITING:WINDOW_TIME_LIMIT_SECONDS").Value);

builder.Services.AddRateLimiter(options =>
{
    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
    RateLimitPartition.GetFixedWindowLimiter(
        partitionKey: httpContext.Request.Headers.Host.ToString(),
        factory: partition => new FixedWindowRateLimiterOptions
        {
            AutoReplenishment = true,
            PermitLimit = permitLimit,
            QueueLimit = queueLimit,
            Window = TimeSpan.FromSeconds(windowTimeLimitSeconds)
        }));

    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
});

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

app.UseRateLimiter();

app.MapControllers();

app.Run();
