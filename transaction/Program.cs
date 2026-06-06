using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MediatR;
using MassTransit;
using Transaction.Application.Commands.CreateTransaction;
using Shared.Infrastructure.Abstractions;
using Transaction.Infrastructure.Repositories;
using Transaction.Infrastructure.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using MassTransit.EntityFrameworkCoreIntegration;
using Transaction.Infrastructure.Persistence;
using Transaction.Infrastructure.Persistence;


var builder = WebApplication.CreateBuilder(args);

var conn = builder.Configuration.GetConnectionString("Postgres")
		   ?? Environment.GetEnvironmentVariable("TRANSACTION_CONNECTION")
		   ?? "Host=localhost;Database=Banking_Transaction;Username=postgres;Password=!P1o1s1t1";

builder.Services.AddHealthChecks();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<TransactionDbContext>(opts => opts.UseNpgsql(conn));
builder.Services.AddDbContext<TransactionSagaDbContext>(opts => opts.UseNpgsql(conn));
builder.Services.AddScoped<IRepository<Transaction.Domain.Entities.Transaction>, TransactionRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddMediatR(typeof(CreateTransactionHandler).Assembly);

builder.Services.AddMassTransit(x =>
{
	x.AddSagaStateMachine<Transaction.Infrastructure.Sagas.TransactionStateMachine, Transaction.Infrastructure.Sagas.TransactionState>()
		.EntityFrameworkRepository(r =>
		{
			r.AddDbContext<TransactionSagaDbContext, TransactionSagaDbContext>((provider, builder) =>
			{
				builder.UseNpgsql(conn, sql => sql.MigrationsAssembly(typeof(TransactionSagaDbContext).Assembly.GetName().Name));
			});
		});
    
	x.AddConsumer<Transaction.Infrastructure.Consumers.PaymentProcessedConsumer>();
	x.UsingRabbitMq((context, cfg) =>
	{
		var host = builder.Configuration.GetValue<string>("RabbitMq:Host") ?? Environment.GetEnvironmentVariable("RABBITMQ_HOST") ?? "localhost";
		cfg.Host(host);
		cfg.ConfigureEndpoints(context);
	});
});

var app = builder.Build();

app.MapGet("/health", () => Results.Ok(new { status = "Healthy" }));
app.MapGet("/", () => Results.Ok("Transaction Service is running."));

app.MapPost("/transactions", async (CreateTransactionCommand cmd, IMediator mediator) =>
{
	var id = await mediator.Send(cmd);
	return Results.Created($"/transactions/{id}", new { id });
});

app.UseSwagger();
app.UseSwaggerUI();

app.Run();
