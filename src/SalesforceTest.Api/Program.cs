using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using SalesforceTest.Api.Features.Auth;
using SalesforceTest.Api.Features.Salesforce;
using SalesforceTest.Api.Interfaces;
using SalesforceTest.Api.Middleware;
using SalesforceTest.Api.Persistence;
using SalesforceTest.Api.Repositories;
using SalesforceTest.Api.Services;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Database
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? "Data Source=salesforcetest.db";
builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlite(connectionString));
builder.Services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<AppDbContext>());

// Repositories
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ISalesforceConnectionRepository, SalesforceConnectionRepository>();
builder.Services.AddScoped<ISalesforceObjectCacheRepository, SalesforceObjectCacheRepository>();

// Services
builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
builder.Services.AddScoped<ITokenService, JwtTokenService>();
builder.Services.AddHttpClient<ISalesforceOAuthService, SalesforceOAuthService>();
builder.Services.AddHttpClient<ISalesforceDataService, SalesforceDataService>();
builder.Services.AddScoped<DatabaseSeeder>();

// Feature services
builder.Services.AddScoped<LoginService>();
builder.Services.AddScoped<GetAuthorizationUrlService>();
builder.Services.AddScoped<HandleOAuthCallbackService>();
builder.Services.AddScoped<GetSalesforceConnectionService>();
builder.Services.AddScoped<DisconnectSalesforceService>();
builder.Services.AddScoped<GetOrdersService>();
builder.Services.AddScoped<GetInvoicesService>();
builder.Services.AddScoped<GetAccountsService>();
builder.Services.AddScoped<GetContactsService>();
builder.Services.AddScoped<GetAvailableObjectsService>();
builder.Services.AddScoped<GetObjectRecordsService>();
builder.Services.AddScoped<RescanObjectsService>();
builder.Services.AddScoped<RefreshObjectCountService>();

// JWT Authentication
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings["SecretKey"] ?? throw new InvalidOperationException("JwtSettings:SecretKey is not configured.");

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
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();

builder.Services.AddCors(options =>
{
    options.AddPolicy("BlazorClient", policy =>
        policy.WithOrigins(builder.Configuration.GetSection("AllowedOrigins").Get<string[]>() ?? ["https://localhost:7000"])
              .AllowAnyHeader()
              .AllowAnyMethod());
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var seeder = scope.ServiceProvider.GetRequiredService<DatabaseSeeder>();
    await seeder.SeedAsync();
}

if (app.Environment.IsDevelopment())
    app.MapOpenApi();

app.UseExceptionHandlingMiddleware();
app.UseHttpsRedirection();
app.UseCors("BlazorClient");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
