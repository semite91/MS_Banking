using MediatR;
using MassTransit;
using MassTransit.EntityFrameworkCoreIntegration;
using Payment.Infrastructure.Sagas;
using Microsoft.EntityFrameworkCore;
using Payment.Application.Commands.CreatePayment;
using Payment.Domain.Entities;
using Payment.Infrastructure.Persistence;
using Payment.Infrastructure.Repositories;
using Payment.Infrastructure.UnitOfWork;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

var conn = builder.Configuration.GetConnectionString("Postgres")
		   ?? Environment.GetEnvironmentVariable("PAYMENT_CONNECTION")
		   ?? "Host=localhost;Database=Banking_Payment;Username=postgres;Password=!P1o1s1t1";

builder.Services.AddHealthChecks();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<PaymentDbContext>(opts => opts.UseNpgsql(conn));
builder.Services.AddDbContext<PaymentSagaDbContext>(opts => opts.UseNpgsql(conn));
builder.Services.AddScoped<Shared.Infrastructure.Abstractions.IRepository<Payment.Domain.Entities.Payment>, PaymentRepository>();
builder.Services.AddScoped<Shared.Infrastructure.Abstractions.IUnitOfWork, UnitOfWork>();
builder.Services.AddMediatR(typeof(CreatePaymentCommand).Assembly);

// MassTransit + Saga registration (in-memory saga repository for development)
builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<Payment.Infrastructure.Consumers.TransactionCreatedConsumer>();

	x.AddSagaStateMachine<PaymentStateMachine, PaymentState>()
		.EntityFrameworkRepository(r =>
		{
			r.AddDbContext<PaymentSagaDbContext, PaymentSagaDbContext>((provider, builder) =>
			{
				builder.UseNpgsql(conn, sql => sql.MigrationsAssembly(typeof(PaymentSagaDbContext).Assembly.GetName().Name));
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

app.MapPost("/payments", async (CreatePaymentDto dto, IMediator mediator) =>
{
	var cmd = new CreatePaymentCommand(dto.Amount, dto.Currency, dto.Method, dto.Provider, dto.TransactionId);
	var id = await mediator.Send(cmd);
	return Results.Created($"/payments/{id}", new { id });
});

app.MapGet("/", () => Results.Ok("Payment Service is running."));

app.UseSwagger();
app.UseSwaggerUI();

app.Run();

public record CreatePaymentDto(decimal Amount, string Currency, string Method, string Provider, Guid? TransactionId = null);
