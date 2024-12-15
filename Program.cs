using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using StudyGroupFinder;
using StudyGroupFinder.Models;
using StudyGroupFinder.Repository;
using StudyGroupFinder.Services;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddTransient<ISellerInfoService, SellerInfoService>();
builder.Services.AddScoped<ISellerInfoRepository, SellerInfoRepository>();
builder.Services.AddScoped<IStudyGroupService, StudyGroupService>();
builder.Services.AddScoped<IStudyGoalRepository, StudyGoalRepository>();
builder.Services.AddScoped<IStudyGoalService, StudyGoalService>();

// Configure DbContext for Entity Framework with SQL Server
builder.Services.AddDbContext<StudyGroupFinderDbContext>(db =>
    db.UseSqlServer(builder.Configuration.GetConnectionString("StudyGroupFinderDbConnectionString")), ServiceLifetime.Scoped);

// Configure Identity services for user management
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
})
    .AddEntityFrameworkStores<StudyGroupFinderDbContext>()
    .AddDefaultTokenProviders();

// Configure JWT Authentication
var jwtSettings = builder.Configuration.GetSection("Jwt");
var key = Encoding.UTF8.GetBytes(jwtSettings["Key"]);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        // Set ValidAudiences to an array of valid audiences
        ValidAudiences = jwtSettings.GetSection("Audiences").Get<string[]>(),
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]))
    };
});

// Configure CORS policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigins", builder =>
        builder.WithOrigins("https://localhost:7034", "http://127.0.0.1:5500", "http://127.0.0.1:5501")  // Frontend URLs
               .AllowAnyMethod()
               .AllowAnyHeader()
               .AllowCredentials());
});

// Configure cookie behavior for Identity
builder.Services.ConfigureApplicationCookie(options =>
{
    options.Events.OnRedirectToLogin = context =>
    {
        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        return Task.CompletedTask;
    };

    options.Events.OnRedirectToAccessDenied = context =>
    {
        context.Response.StatusCode = StatusCodes.Status403Forbidden;
        return Task.CompletedTask;
    };
});

// Add controllers and setup endpoints
builder.Services.AddControllers()
    .AddJsonOptions(options =>
        options.JsonSerializerOptions.PropertyNamingPolicy = null); // Ensures case-sensitive matching

// Configure Swagger/OpenAPI only for development
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Enter 'Bearer' followed by a space and your token",
        Name = "Authorization",
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
}); ;

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Enable CORS globally
app.UseCors("AllowSpecificOrigins");

// Enable authentication and authorization
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers(); // Map controller routes

app.Run();
