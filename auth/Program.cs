using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OpenIddict.Abstractions;
using OpenIddict.EntityFrameworkCore.Models;
using Auth.Infrastructure.Persistence;
using Auth.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

var conn = builder.Configuration.GetConnectionString("Postgres")
		   ?? Environment.GetEnvironmentVariable("AUTH_CONNECTION")
		   ?? "Host=localhost;Database=Banking_Auth;Username=postgres;Password=!P1o1s1t1";

builder.Services.AddHealthChecks();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddControllers();

builder.Services.AddDbContext<AuthDbContext>(opts =>
	opts.UseNpgsql(conn, sql => sql.MigrationsAssembly(typeof(AuthDbContext).Assembly.GetName().Name)));

builder.Services.AddIdentity<IdentityUser, IdentityRole>()
	.AddEntityFrameworkStores<AuthDbContext>()
	.AddDefaultTokenProviders();

builder.Services.AddScoped<RefreshTokenService>();

builder.Services.AddOpenIddict()
	.AddCore(options =>
	{
		options.UseEntityFrameworkCore()
			   .UseDbContext<AuthDbContext>();
	})
	.AddServer(options =>
	{
		options.SetTokenEndpointUris("/connect/token");

		options.AllowPasswordFlow()
			   .AllowRefreshTokenFlow();

		// For dev: use ephemeral signing/encryption keys
		options.AddDevelopmentEncryptionCertificate()
			   .AddDevelopmentSigningCertificate();

		options.UseAspNetCore();
	})
	.AddValidation(options =>
	{
		options.UseLocalServer();
		options.UseAspNetCore();
	});

builder.Services.AddAuthentication(options =>
{
	// Allow JWT Bearer tokens for the API endpoints (we also keep OpenIddict configured)
	options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
	options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
});

builder.Services.AddAuthentication()
	.AddJwtBearer(options =>
	{
		var key = builder.Configuration.GetValue<string>("Jwt:Key") ?? "dev-secret-key-please-change";
		options.TokenValidationParameters = new TokenValidationParameters
		{
			ValidateIssuer = false,
			ValidateAudience = false,
			ValidateIssuerSigningKey = true,
			IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key))
		};
	});

var app = builder.Build();

// Seed a test user and a test client
using (var scope = app.Services.CreateScope())
{
	var services = scope.ServiceProvider;
	var db = services.GetRequiredService<AuthDbContext>();
	db.Database.Migrate();

	var userManager = services.GetRequiredService<UserManager<IdentityUser>>();

	var testUser = userManager.FindByNameAsync("testuser").GetAwaiter().GetResult();
	if (testUser == null)
	{
		testUser = new IdentityUser("testuser") { Email = "testuser@example.com", EmailConfirmed = true };
		userManager.CreateAsync(testUser, "P@ssword123!").GetAwaiter().GetResult();
	}
}

app.MapGet("/health", () => Results.Ok(new { status = "Healthy" }));
app.MapGet("/", () => Results.Ok("Auth Service is running."));

app.MapControllers();

app.UseAuthentication();
app.UseAuthorization();

app.UseSwagger();
app.UseSwaggerUI();

app.Run();
