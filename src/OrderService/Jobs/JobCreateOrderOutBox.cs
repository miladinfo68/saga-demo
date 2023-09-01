using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using CommonService.Dtos;
using CommonService.Enums;
using CommonService.MessageBrokers.RabbitMq;
using OrderService.Models;

namespace OrderService.Jobs;

public class JobCreateOrderOutBox : BackgroundService
{
    private readonly CancellationToken _cancellationToken;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly IRabbitMqService<RabbitMqOrderService> _rabbitMqOrderService;
    //can not inject directly singletone OrderDbContext into this background service because of 
    //service lifecycle issue


    public JobCreateOrderOutBox(
        IHostApplicationLifetime applicationLifetime,
        IServiceScopeFactory serviceScopeFactory,
        IRabbitMqService<RabbitMqOrderService> rabbitMqOrderService)
    {
        _cancellationToken = applicationLifetime.ApplicationStopping;
        _serviceScopeFactory = serviceScopeFactory;
        _rabbitMqOrderService = rabbitMqOrderService;
    }


    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await using var scope = _serviceScopeFactory.CreateAsyncScope();
        var orderDb = scope.ServiceProvider.GetRequiredService<OrderDbContext>();
        var userOrders = orderDb!.OrderOutbox!.Where(w => w.Status == OrderStatus.Pending)?.ToHashSet();

        while (!_cancellationToken.IsCancellationRequested && userOrders?.Count > 0)
        {
            foreach (var order in userOrders)
            {
                //if (!_rabbitMqService.IsRabbitMqServerUp()) continue;
                //await SendOrderOutBox2Queue(order, orderDb, _cancellationToken);
                var iSendMessage =  _rabbitMqOrderService.SendMessage(order);
                userOrders.Remove(order);
            }
        }
    }



    private async Task SendOrderOutBox2Queue(Outbox orderOutbox, OrderDbContext db, CancellationToken cancellationToken)
    {
        await using var transaction = await db.Database.BeginTransactionAsync(cancellationToken);
        try
        {

            var messageSentSuccessfully = _rabbitMqOrderService.SendMessage(orderOutbox);
            if (messageSentSuccessfully)
            {
                orderOutbox.Status = OrderStatus.Approved;
                await db.SaveChangesAsync(cancellationToken);

                var deSerializedOrder = JsonConvert.DeserializeObject<OutboxOrderDto>(orderOutbox.EventData);
                if (deSerializedOrder is not null)
                {
                    var order = await db.Orders.FirstOrDefaultAsync(x => x.Id == deSerializedOrder.OrderId, cancellationToken);
                    if (order != null)
                    {
                        order.Status = OrderStatus.Approved;
                        await db.SaveChangesAsync(cancellationToken);
                    }
                }

                await transaction.CommitAsync(cancellationToken);
            }
        }
        catch (Exception)
        {
            await transaction.RollbackAsync(cancellationToken);
        }

    }





    /*

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        List<TestMessage> messages = new();
        for (var i = 1; i <= 1000; i++)
        {
            messages.Add(new TestMessage(i, $"Message\t{i}"));
        }

        var counter = messages?.Count() ?? 0;

        while (!_cancellationToken.IsCancellationRequested && counter > 0)
        {

            foreach (var message in messages)
            {
                await SendFakeMessage(message);
                --counter;
            }
        }
    }



    public record TestMessage(int Id, string Message);

    private async Task SendFakeMessage(object data)
    {
        await using var scope = _scopeFactory.CreateAsyncScope();
        var rabbitMqService = scope.ServiceProvider.GetRequiredService<IRabbitMqService>();
        await rabbitMqService.SendMessage(data);
    }

    */

}
