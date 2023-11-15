using DocumentServiceWebAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Serilog.Events;
using Serilog;
using System.Text.Json.Serialization;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Controller Services
builder.Services.AddControllers(options => options.Filters.Add(new ProducesAttribute("application/json")))
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
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
