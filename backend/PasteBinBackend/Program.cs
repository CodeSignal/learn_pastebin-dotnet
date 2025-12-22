using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BCrypt.Net;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PasteBinBackend.Data;
using PasteBinBackend.Models;

// Needs at least 256 bits (32 bytes) for HS256
const string JwtSecretKey = "0123456789abcdef0123456789abcdef";

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.UseUrls("http://localhost:3001");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=pastebin.db"));

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
    });
});

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JwtSecretKey)),
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization();
builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();

    if (!db.Users.Any(u => u.Username == "admin"))
    {
        db.Users.Add(new User
        {
            Username = "admin",
            Password = BCrypt.Net.BCrypt.HashPassword("admin"),
            Role = "admin"
        });
        db.SaveChanges();
    }
}

app.UseCors();
app.UseAuthentication();
app.UseAuthorization();

app.MapPost("/api/auth/register", async (RegisterRequest request, AppDbContext db) =>
{
    var exists = await db.Users.AnyAsync(u => u.Username == (request.Username ?? string.Empty));
    if (exists)
    {
        return Results.BadRequest(new { error = "Username already exists" });
    }

    var user = new User
    {
        Username = request.Username ?? string.Empty,
        Password = BCrypt.Net.BCrypt.HashPassword(request.Password ?? string.Empty),
        Role = (request.Role?.ToLower() == "admin") ? "admin" : "user"
    };

    db.Users.Add(user);
    await db.SaveChangesAsync();

    return Results.Json(new { message = "User registered successfully", userId = user.Id });
});

app.MapPost("/api/auth/login", async (LoginRequest request, AppDbContext db) =>
{
    var user = await db.Users.FirstOrDefaultAsync(u => u.Username == (request.Username ?? string.Empty));
    if (user is null || !BCrypt.Net.BCrypt.Verify(request.Password ?? string.Empty, user.Password))
    {
        return Results.Unauthorized();
    }

    var token = GenerateToken(user);
    return Results.Json(new { token });
});

app.MapPost("/api/snippets", async (SnippetRequest request, ClaimsPrincipal principal, AppDbContext db) =>
{
    var userIdClaim = principal.FindFirstValue(ClaimTypes.NameIdentifier);
    if (userIdClaim is null || !int.TryParse(userIdClaim, out var userId))
    {
        return Results.Unauthorized();
    }

    var snippet = new Snippet
    {
        Id = Guid.NewGuid(),
        Title = request.Title ?? string.Empty,
        Content = request.Content ?? string.Empty,
        Language = request.Language ?? string.Empty,
        UserId = userId
    };

    db.Snippets.Add(snippet);
    await db.SaveChangesAsync();

    return Results.Json(snippet);
}).RequireAuthorization();

app.MapGet("/api/snippets/{id}", async (Guid id, AppDbContext db) =>
{
    var snippet = await db.Snippets.FindAsync(id);
    return snippet is null ? Results.NotFound(new { error = "Snippet not found" }) : Results.Json(snippet);
});

app.MapGet("/api/snippets", async (ClaimsPrincipal principal, AppDbContext db) =>
{
    var userIdClaim = principal.FindFirstValue(ClaimTypes.NameIdentifier);
    if (userIdClaim is null || !int.TryParse(userIdClaim, out var userId))
    {
        return Results.Unauthorized();
    }

    var snippets = await db.Snippets.Where(s => s.UserId == userId).ToListAsync();
    return Results.Json(snippets);
}).RequireAuthorization();

app.MapDelete("/api/snippets/{id}", async (Guid id, AppDbContext db) =>
{
    var snippet = await db.Snippets.FindAsync(id);
    if (snippet is null)
    {
        return Results.NotFound(new { error = "Snippet not found" });
    }

    db.Snippets.Remove(snippet);
    await db.SaveChangesAsync();
    return Results.Json(new { message = "Snippet deleted successfully" });
});

app.MapGet("/api/admin/test", async (ClaimsPrincipal principal, AppDbContext db) =>
{
    var userIdClaim = principal.FindFirstValue(ClaimTypes.NameIdentifier);
    if (userIdClaim is null || !int.TryParse(userIdClaim, out var userId))
    {
        return Results.Unauthorized();
    }

    var user = await db.Users.FindAsync(userId);
    if (user is null || user.Role != "admin")
    {
        return Results.Json(new { error = "Access denied" }, statusCode: StatusCodes.Status403Forbidden);
    }

    return Results.Json(new { message = "Admin test endpoint accessed successfully" });
}).RequireAuthorization();

app.MapGet("/api/admin/testOpen", () => Results.Json(new { message = "Test endpoint is working!" }));

app.MapGet("/api/admin/accountInfo", async (int? id, AppDbContext db) =>
{
    if (id is null)
    {
        return Results.BadRequest(new { error = "Missing user ID parameter" });
    }

    // Intentionally naive query to mirror the original demo's insecurity.
    var sql = $"SELECT * FROM Users WHERE Id = {id}";
    var results = await db.Users.FromSqlRaw(sql).ToListAsync();
    return Results.Json(results);
});

app.Run();

static string GenerateToken(User user)
{
    var claims = new List<Claim>
    {
        new(ClaimTypes.NameIdentifier, user.Id.ToString()),
        new(ClaimTypes.Name, user.Username),
        new(ClaimTypes.Role, user.Role)
    };

    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JwtSecretKey));
    var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
    var token = new JwtSecurityToken(
        claims: claims,
        expires: DateTime.UtcNow.AddHours(4),
        signingCredentials: creds);

    return new JwtSecurityTokenHandler().WriteToken(token);
}

record LoginRequest(string? Username, string? Password);
record RegisterRequest(string? Username, string? Password, string? Role);
record SnippetRequest(string? Title, string? Content, string? Language);