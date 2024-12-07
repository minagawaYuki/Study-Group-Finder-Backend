using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using StudyGroupFinder;
using StudyGroupFinder.Models;
using StudyGroupFinder.Repository;
using StudyGroupFinder.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddTransient<ISellerInfoService, SellerInfoService>();
builder.Services.AddScoped<ISellerInfoRepository, SellerInfoRepository>();

// Configure the DbContext for Entity Framework with SQL Server
builder.Services.AddDbContext<StudyGroupFinderDbContext>(db =>
    db.UseSqlServer(builder.Configuration.GetConnectionString("StudyGroupFinderDbConnectionString")), ServiceLifetime.Scoped);

// Configure Identity services for user management
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    // Optional: Customize Identity options here, like password policy, lockout settings, etc.
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
})
    .AddEntityFrameworkStores<StudyGroupFinderDbContext>()
    .AddDefaultTokenProviders();

// Configure CORS policy to allow the specific frontend and backend origins
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigins", builder =>
        builder.WithOrigins("https://localhost:7034", "http://127.0.0.1:5500", "http://127.0.0.1:5501")  // Allow both backend and frontend URLs
               .AllowAnyMethod()
               .AllowAnyHeader()
               .AllowCredentials());  // Allow credentials (cookies, authorization headers, etc.)
});

// Add controllers and setup endpoints
builder.Services.AddControllers();

// Configure Swagger/OpenAPI only for development environment
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Enable CORS globally - use the policy that allows specific origins
app.UseCors("AllowSpecificOrigins");

app.UseAuthentication();  // Enable authentication middleware
app.UseAuthorization();   // Enable authorization middleware

app.MapControllers();  // Map controller routes

app.Run();
