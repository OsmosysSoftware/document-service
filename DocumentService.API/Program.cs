using DocumentService.API.Models;
using Microsoft.AspNetCore.Mvc;
using Serilog.Events;
using Serilog;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Swashbuckle.AspNetCore.Filters;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Controller Services
builder.Services.AddControllers(options => options.Filters.Add(new ProducesAttribute("application/json")))
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

// Load .env file
string root = Directory.GetCurrentDirectory();
string dotenv = Path.GetFullPath(Path.Combine(root, "..", ".env"));
DocumentService.API.DotEnv.Load(dotenv);

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
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "DocumentService API", Version = "v1" });

    options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Description = "Standard Authorization header using the Bearer Scheme (\"bearer {token}\")",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey
    });

    options.OperationFilter<SecurityRequirementsOperationFilter>();
});

// Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    string JWT_KEY = Environment.GetEnvironmentVariable("JWT_KEY") ?? throw new InvalidOperationException("No JWT key specified");

    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JWT_KEY)),
        ValidateIssuer = false,
        ValidateAudience = false,
        // Following code is to allow us to custom handle expiry
        // Here check expiry as nullable
        ClockSkew = TimeSpan.Zero,
        ValidateLifetime = true,
        LifetimeValidator = (DateTime? notBefore, DateTime? expires, SecurityToken securityToken, TokenValidationParameters validationParameters) =>
        {
            // Clone the validation parameters, and remove the defult lifetime validator
            TokenValidationParameters clonedParameters = validationParameters.Clone();
            clonedParameters.LifetimeValidator = null;

            // If token expiry time is not null, then validate lifetime with skewed clock
            if (expires != null)
            {
                Validators.ValidateLifetime(notBefore, expires, securityToken, clonedParameters);
            }

            return true;
        }
    };
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

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
