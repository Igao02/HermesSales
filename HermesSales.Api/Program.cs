using HermesSales.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// ----------------------
// Banco de dados
// ----------------------
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

// ----------------------
// Identity API Endpoints
// ----------------------
builder.Services
    .AddIdentityApiEndpoints<ApplicationUser>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddAuthorization();

// ----------------------
// Swagger
// ----------------------
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// ----------------------
// Swagger
// ----------------------
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// ----------------------
// Middleware
// ----------------------
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

// ----------------------
// Identity Endpoints
// ----------------------
app.MapIdentityApi<ApplicationUser>();

app.Run();