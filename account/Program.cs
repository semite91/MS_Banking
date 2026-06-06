using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MediatR;
using MassTransit;
using Account.Application.Commands.CreateAccount;
using Account.Domain.Entities;
using Account.Infrastructure.Repositories;
using Account.Infrastructure.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using Account.Infrastructure.Persistence;
using MassTransit.EntityFrameworkCoreIntegration;

var builder = WebApplication.CreateBuilder(args);

// Configuration / Connection
var conn = builder.Configuration.GetConnectionString("Postgres")
		   ?? Environment.GetEnvironmentVariable("ACCOUNT_CONNECTION")
		   ?? "Host=localhost;Database=Banking_User;Username=postgres;Password=!P1o1s1t1";

builder.Services.AddHealthChecks();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<AccountDbContext>(opts => opts.UseNpgsql(conn));
builder.Services.AddDbContext<AccountSagaDbContext>(opts => opts.UseNpgsql(conn));
builder.Services.AddScoped<Shared.Infrastructure.Abstractions.IRepository<Account.Domain.Entities.Account>, AccountRepository>();
builder.Services.AddScoped<Shared.Infrastructure.Abstractions.IUnitOfWork, UnitOfWork>();
builder.Services.AddMediatR(typeof(CreateAccountHandler).Assembly);

builder.Services.AddMassTransit(x =>
{
	x.AddConsumer<Account.Infrastructure.Consumers.CustomerCreatedConsumer>();
	x.AddSagaStateMachine<Account.Infrastructure.Sagas.AccountStateMachine, Account.Infrastructure.Sagas.AccountState>()
		.EntityFrameworkRepository(r =>
		{
			r.AddDbContext<AccountSagaDbContext, AccountSagaDbContext>((provider, builder) =>
			{
				builder.UseNpgsql(conn, sql => sql.MigrationsAssembly(typeof(AccountSagaDbContext).Assembly.GetName().Name));
			});
		});
	x.UsingRabbitMq((context, cfg) =>
	{
		var host = builder.Configuration.GetValue<string>("RabbitMq:Host") ?? Environment.GetEnvironmentVariable("RABBITMQ_HOST") ?? "localhost";
		cfg.Host(host);
		cfg.ConfigureEndpoints(context);
	});
});

var app = builder.Build();

app.MapGet("/health", () => Results.Ok(new { status = "Healthy" }));
app.MapGet("/", () => Results.Ok("Account Service is running."));

app.MapPost("/accounts", async (CreateAccountCommand cmd, IMediator mediator) =>
{
	var id = await mediator.Send(cmd);
	return Results.Created($"/accounts/{id}", new { id });
});

app.UseSwagger();
app.UseSwaggerUI();

app.Run();
