using Infrastructure.BankConnection;
using Infrastructure.Instrumentation.MicrosoftApplicationInsights;
using Infrastructure.Messaging;
using Infrastructure.Messaging.AzureStorageQueue;
using Infrastructure.Persistence;
using Infrastructure.Vault;
using Payment.Common.Dto;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddMicrosoftApplicationInsights("Bank Processor", "Bank Processor Instance");


builder.Services.AddOptions();
builder.Services.Configure<QueueingOptions>(
    builder.Configuration.GetSection("Queueing"));

builder.Services.AddOptions();
builder.Services.Configure<DbConnectionOptions>(
    builder.Configuration.GetSection("DbConnection"));


builder.Services.AddBankSimulator();
builder.Services.AddAzureStorageQueue(builder.Configuration);
builder.Services.AddAzureSecrets(builder.Configuration);
builder.Services.AddRepositories();
builder.Services.AddAzureStorageQueueConsumer<PaymentRequestDto>(builder.Configuration);
builder.Services.AddHealthChecks();
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

app.UseHealthChecks("/health");

app.Run();