using Infrastructure.Instrumentation.MicrosoftApplicationInsights;
using Infrastructure.Messaging;
using Infrastructure.Messaging.AzureStorageQueue;
using Infrastructure.Persistence;
using Infrastructure.Vault;
using Infrastructure.Vault.AzureVault;
using Payment.Common.Dto;
using PaymentApi.Domain.Models;

var builder = WebApplication.CreateBuilder(args);


// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddMicrosoftApplicationInsights("Payment Api", "Payment Api Instance");


builder.Services.AddOptions();
builder.Services.Configure<QueueingOptions>(
    builder.Configuration.GetSection("Queueing"));

builder.Services.AddOptions();
builder.Services.Configure<DbConnectionOptions>(
    builder.Configuration.GetSection("DbConnection"));

builder.Services.AddAzureStorageQueue(builder.Configuration);
builder.Services.AddAzureSecrets(builder.Configuration);
builder.Services.AddRepositories();

builder.Services.AddMediatR(cfg => { cfg.RegisterServicesFromAssembly(typeof(Program).Assembly); });


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();