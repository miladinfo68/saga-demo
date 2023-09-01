using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using CommonService.Configs;
using CommonService.Extensions;
using StockService.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

builder.Services.Configure<ConnectionStrings>(builder.Configuration.GetSection("ConnectionStrings"));
builder.Services.Configure<RabbitMqConnection>(builder.Configuration.GetSection("RabbitMqConnection"));
builder.Services.Configure<RabbitMqConfigs>(builder.Configuration.GetSection("RabbitMqConfigs"));

builder.Services.AddRabbitMqServices(builder.Configuration);

builder.Services.AddDbContextPool<StockDbContext>((serviceProvider, options) =>
{
    var connStringsOptions = serviceProvider.GetRequiredService<IOptions<ConnectionStrings>>().Value;
    options.UseSqlServer(connStringsOptions.InventoryDbConnection);
});


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();


app.Run();

