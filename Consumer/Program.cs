using Consumer.Clients;
using Consumer.Config;
using Consumer.Consumers;
using Microsoft.AspNetCore.Builder;

var builder = WebApplication.CreateBuilder(args);
builder.Services.Configure<RabbitMqSettings>(builder.Configuration.GetSection(nameof(RabbitMqSettings)));
builder.Services.AddHostedService<OmsOrderCreatedConsumer>();
builder.Services.AddHttpClient<OmsClient>(c => c.BaseAddress = new Uri(builder.Configuration["HttpClient:Oms:BaseAddress"]));

var app = builder.Build();
await app.RunAsync();