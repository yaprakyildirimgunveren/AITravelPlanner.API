using AITravelPlanner.Data;
using AITravelPlanner.Services.Services.Abstract;
using AITravelPlanner.Services.Services.Concrete;
using Microsoft.EntityFrameworkCore;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using AITravelPlanner.Data.Repositories.Interfaces;
using AITravelPlanner.Data.Repositories;
using AITravelPlanner.API.Workers;
using AITravelPlanner.Services.Messaging;
using AITravelPlanner.Services.Options;
using AITravelPlanner.Data.Seed;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IFlightService, FlightService>();
builder.Services.AddScoped<ITravelService, TravelService>();
builder.Services.AddScoped<IRecommendationService, AiRecommendationService>();
builder.Services.AddSingleton<IMessagePublisher, RabbitMqPublisher>();
builder.Services.AddHostedService<RabbitMqConsumerWorker>();

builder.Services.Configure<AiServiceOptions>(builder.Configuration.GetSection("AIService"));
builder.Services.Configure<RabbitMqOptions>(builder.Configuration.GetSection("RabbitMQ"));
builder.Services.AddHttpClient<IRecommendationService, AiRecommendationService>();

var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings["Secret"];

var key = Encoding.UTF8.GetBytes(secretKey);

builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(key)
        };
    });

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "AITravelPlanner API", Version = "v1" });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});


builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();


var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

var allowedOrigins = builder.Configuration
    .GetSection("Cors:AllowedOrigins")
    .Get<string[]>() ?? Array.Empty<string>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("DefaultCors", policy =>
    {
        if (allowedOrigins.Length == 0)
        {
            policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
            return;
        }

        policy.WithOrigins(allowedOrigins).AllowAnyHeader().AllowAnyMethod();
    });
});

builder.Services.AddControllers();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await dbContext.Database.MigrateAsync();
    await DbSeeder.SeedAsync(dbContext);
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "AITravelPlanner.API v1");
        c.RoutePrefix = string.Empty; 
    });
}

app.UseHttpsRedirection();
app.UseCors("DefaultCors");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapGet("/health", () => Results.Ok(new { status = "ok" }));
app.MapGet("/status", async (IHttpClientFactory httpClientFactory, IOptions<AiServiceOptions> aiOptions) =>
{
    var client = httpClientFactory.CreateClient();
    var aiBaseUrl = aiOptions.Value.BaseUrl?.TrimEnd('/') ?? "http://localhost:8000";
    var aiHealth = "unreachable";

    try
    {
        var response = await client.GetAsync($"{aiBaseUrl}/health");
        aiHealth = response.IsSuccessStatusCode ? "ok" : $"error ({(int)response.StatusCode})";
    }
    catch
    {
        aiHealth = "error";
    }

    var html = $@"
<!doctype html>
<html lang=""en"">
<head>
  <meta charset=""utf-8"" />
  <title>AITravelPlanner Status</title>
  <style>
    body {{ font-family: Arial, Helvetica, sans-serif; background: #0b1020; color: #f4f6fb; padding: 24px; }}
    .card {{ background: #121a33; padding: 16px; border-radius: 12px; max-width: 520px; }}
    .row {{ display: flex; justify-content: space-between; padding: 8px 0; }}
    a {{ color: #8aa2ff; }}
  </style>
</head>
<body>
  <div class=""card"">
    <h2>Service Status</h2>
    <div class=""row""><span>API</span><strong>ok</strong></div>
    <div class=""row""><span>AI Service</span><strong>{aiHealth}</strong></div>
    <div class=""row""><span>Health Endpoint</span><a href=""/health"">/health</a></div>
    <div class=""row""><span>Swagger UI</span><a href=""/swagger"">/swagger</a></div>
  </div>
</body>
</html>
";

    return Results.Content(html, "text/html");
});

app.Run();