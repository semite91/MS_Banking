using MediatR;
using Microsoft.EntityFrameworkCore;
using Fraud.Application.Commands.RaiseFraudAlert;
using Fraud.Domain.Entities;
using Fraud.Infrastructure.Persistence;
using Fraud.Infrastructure.Repositories;
using Fraud.Infrastructure.UnitOfWork;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MassTransit;
using MassTransit.EntityFrameworkCoreIntegration;
using Fraud.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

var conn = builder.Configuration.GetConnectionString("Postgres")
		   ?? Environment.GetEnvironmentVariable("FRAUD_CONNECTION")
		   ?? "Host=localhost;Database=Banking_Fraud;Username=postgres;Password=!P1o1s1t1";

builder.Services.AddHealthChecks();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<FraudDbContext>(opts => opts.UseNpgsql(conn));
builder.Services.AddDbContext<FraudSagaDbContext>(opts => opts.UseNpgsql(conn));
builder.Services.AddScoped<Shared.Infrastructure.Abstractions.IRepository<FraudAlert>, FraudAlertRepository>();
builder.Services.AddScoped<Shared.Infrastructure.Abstractions.IUnitOfWork, UnitOfWork>();
builder.Services.AddMediatR(typeof(RaiseFraudAlertCommand).Assembly);

builder.Services.AddMassTransit(x =>
{
	x.AddConsumers(typeof(Fraud.Infrastructure.Consumers.TransactionCreatedConsumer).Assembly);

	x.AddSagaStateMachine<Fraud.Infrastructure.Sagas.FraudStateMachine, Fraud.Infrastructure.Sagas.FraudState>()
		.EntityFrameworkRepository(r =>
		{
			r.AddDbContext<FraudSagaDbContext, FraudSagaDbContext>((provider, builder) =>
			{
				builder.UseNpgsql(conn, sql => sql.MigrationsAssembly(typeof(FraudSagaDbContext).Assembly.GetName().Name));
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

app.MapPost("/fraud/alerts", async (RaiseFraudAlertDto dto, IMediator mediator) =>
{
	var cmd = new RaiseFraudAlertCommand(dto.TransactionId, dto.PaymentId, dto.Score, dto.RuleId, dto.Details);
	var id = await mediator.Send(cmd);
	return Results.Created($"/fraud/alerts/{id}", new { id });
});

app.MapGet("/", () => Results.Ok("Fraud Service is running."));

app.UseSwagger();
app.UseSwaggerUI();

app.Run();

public record RaiseFraudAlertDto(Guid? TransactionId, Guid? PaymentId, decimal Score, string RuleId, string? Details);
