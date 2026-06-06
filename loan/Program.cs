using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MediatR;
using MassTransit;
using Loan.Application.Commands.CreateLoan;
using Loan.Infrastructure.Repositories;
using Loan.Infrastructure.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using MassTransit.EntityFrameworkCoreIntegration;
using Loan.Infrastructure.Persistence;


var builder = WebApplication.CreateBuilder(args);

var conn = builder.Configuration.GetConnectionString("Postgres")
		   ?? Environment.GetEnvironmentVariable("LOAN_CONNECTION")
		   ?? "Host=localhost;Database=Banking_Loan;Username=postgres;Password=!P1o1s1t1";

builder.Services.AddHealthChecks();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IMongoRepository<Loan.Domain.Entities.LoanApplication>, MongoRepository<Loan.Domain.Entities.LoanApplication>>();
builder.Services.AddScoped<IMongoUnitOfWork, MongoUnitOfWork>();
builder.Services.AddMediatR(typeof(CreateLoanHandler).Assembly);

builder.Services.AddDbContext<LoanSagaDbContext>(opts => opts.UseNpgsql(conn));

builder.Services.AddMassTransit(x =>
{
	x.AddConsumer<Loan.Infrastructure.Consumers.CustomerCreatedConsumer>();
	builder.Services.AddDbContext<LoanSagaDbContext>(opts => opts.UseNpgsql(conn));
	x.AddSagaStateMachine<Loan.Infrastructure.Sagas.LoanStateMachine, Loan.Infrastructure.Sagas.LoanState>()
		.EntityFrameworkRepository(r =>
		{
			r.AddDbContext<LoanSagaDbContext, LoanSagaDbContext>((provider, builder) =>
			{
				builder.UseNpgsql(conn, sql => sql.MigrationsAssembly(typeof(Loan.Infrastructure.Persistence.LoanSagaDbContext).Assembly.GetName().Name));
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
app.MapGet("/", () => Results.Ok("Loan Service is running."));

app.MapPost("/loans", async (CreateLoanCommand cmd, IMediator mediator) =>
{
	var id = await mediator.Send(cmd);
	return Results.Created($"/loans/{id}", new { id });
});

app.UseSwagger();
app.UseSwaggerUI();

app.Run();
