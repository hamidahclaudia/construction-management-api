using System.Text;
using ConstructionManagement.Api.Middleware;
using ConstructionManagement.Application.Helpers;
using ConstructionManagement.Application.Interfaces.Messaging;
using ConstructionManagement.Application.Interfaces.Repositories;
using ConstructionManagement.Application.Interfaces.Services;
using ConstructionManagement.Application.Services;
using ConstructionManagement.Infrastructure.ElasticSearch;
using ConstructionManagement.Infrastructure.Messaging;
using ConstructionManagement.Infrastructure.Persistence;
using Elasticsearch.Net;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Nest;
using Newtonsoft.Json;

var builder = WebApplication.CreateBuilder(args);

// Database Configuration
builder.Services.AddDbContext<ApplicationDbContext>(options => 
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// ElasticSearch Configuration
var pool = new SingleNodeConnectionPool(new Uri("http://localhost:9200"));
var settings = new ConnectionSettings(pool)
    .BasicAuthentication("elastic", "myelastic")
    .ServerCertificateValidationCallback((o, certificate, chain, errors) => true) // Bypass SSL issues
    .DefaultIndex("construction_projects");

//Register ElasticClient as a Singleton
builder.Services.AddSingleton<IElasticClient>(new ElasticClient(settings));

// Add services to the container.
builder.Services.AddControllers()
    .AddNewtonsoftJson(options =>
    {
        options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
    });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "Construction Management API", Version = "v1" });

    // 🔹 Add JWT Authentication Support
    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Name = "Authorization",
        Description = "Enter 'Bearer {your JWT token}'"
    });

    // 🔹 Apply JWT Authentication Globally
    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});


//Register Repositories 
builder.Services.AddScoped(typeof(ConstructionManagement.Application.Interfaces.Repositories.IRepository<>), typeof(RepositoryBase<>));
builder.Services.AddScoped<IProjectRepository, ProjectRepositoryBase>();
builder.Services.AddScoped<IUserRepository, UserRepositoryBase>();
builder.Services.AddScoped<IProjectCategoryRepository, ProjectCategoryRepository>();
builder.Services.AddScoped<IProjectStageRepository, ProjectStageRepository>();
builder.Services.AddSingleton<IElasticSearchRepository, ElasticSearchRepository>();

//Register Services
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IProjectService, ProjectService>();
builder.Services.AddScoped<IProjectCategoryService, ProjectCategoryService>();
builder.Services.AddScoped<IProjectStageService, ProjectStageService>();

//Register Helper
builder.Services.AddScoped<JwtTokenGenerator>();
builder.Services.AddScoped<ProjectNumberGenerator>();

//Register Messaging
builder.Services.AddSingleton<IKafkaProducer, KafkaProducer>();
builder.Services.AddHostedService<KafkaConsumer>();

//Authentication & Authorization
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
        };
    });

builder.Services.AddAuthorization();

//Enable CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowLocalhost", policy =>
    {
        policy.WithOrigins("http://localhost:5173") 
            .AllowAnyHeader()                      
            .AllowAnyMethod();                     
    });
});

var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() || app.Environment.IsProduction()) 
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Construction Management API v1");
        c.DefaultModelsExpandDepth(-1); // Optional: Hide schema models
    });
}


app.UseHttpsRedirection();
app.UseAuthorization();
app.UseCors("AllowLocalhost");
app.MapControllers();
app.Run();

