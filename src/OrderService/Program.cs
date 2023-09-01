using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using CommonService.Configs;
using CommonService.Extensions;
using CommonService.MessageBrokers.RabbitMq;
using OrderService.Jobs;
using OrderService.Models;
using OrderService.ApiEndPoints;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<ConnectionStrings>(builder.Configuration.GetSection("ConnectionStrings"));
builder.Services.Configure<RabbitMqConnection>(builder.Configuration.GetSection("RabbitMqConnection"));
builder.Services.Configure<RabbitMqConfigs>(builder.Configuration.GetSection("RabbitMqConfigs"));

builder.Services.AddRabbitMqServices(builder.Configuration);

builder.Services.AddDbContextPool<OrderDbContext>((serviceProvider, options) =>
{
    var connStringsOptions = serviceProvider.GetRequiredService<IOptions<ConnectionStrings>>().Value;
    options.UseSqlServer(connStringsOptions.OrderDbConnection);
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHostedService<JobCreateOrderOutBox>();
// builder.Services.AddHostedService<JobUpdateOrderOutBoxStatus>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();

app.AddOrderEndPoints();

app.Run();







