using HermesSales.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("ApiWeb", policy =>
    {
        policy.WithOrigins("https://localhost:7127")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

// Banco
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

// Identity
builder.Services
    .AddIdentityApiEndpoints<ApplicationUser>(options =>
    {
        options.SignIn.RequireConfirmedEmail = false;
    })
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddAuthorization();

// Cookie importante para cross-domain
builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.HttpOnly = true;
    options.Cookie.SameSite = SameSiteMode.None;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
});

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseCors("ApiWeb");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

// Endpoints do Identity
app.MapIdentityApi<ApplicationUser>();

app.Run();