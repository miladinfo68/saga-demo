using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using CommonService.Dtos;
using CommonService.Enums;
using CommonService.MessageBrokers.RabbitMq;
using OrderService.Models;

namespace OrderService.Jobs;

public class JobUpdateOrderOutBoxStatus : BackgroundService
{
    private readonly CancellationToken _cancellationToken;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly IRabbitMqService<RabbitMqOrderService> _rabbitMqOrderService;

    public JobUpdateOrderOutBoxStatus(
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

        var receivedMessages = await _rabbitMqOrderService.ReceiveMessages();
        var messagesList = receivedMessages.ToList();

        if (!messagesList.Any()) await Task.CompletedTask;

        while (!_cancellationToken.IsCancellationRequested && messagesList.Any())
        {
            foreach (var message in messagesList)
            {
                await UpdateOrderStatusAsync(orderDb, message, _cancellationToken);
                messagesList.Remove(message);
            }
        }
    }

    private static async Task UpdateOrderStatusAsync(OrderDbContext db, string message,
        CancellationToken cancellationToken)
    {
        await using var transaction = await db.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            var deSerializedOrderOutbox = JsonConvert.DeserializeObject<OrderOutbox>(message);

            if (deSerializedOrderOutbox == null) await Task.CompletedTask;

            var deSerializedOrderOutboxOrderDto =
                JsonConvert.DeserializeObject<OutboxOrderDto>(deSerializedOrderOutbox!.EventData);
            
            if (deSerializedOrderOutboxOrderDto == null) await Task.CompletedTask;

            var order = await db.Orders
                .FirstOrDefaultAsync(x => x.Id == deSerializedOrderOutboxOrderDto!.OrderId, cancellationToken);

            if (order is not { Status: OrderStatus.Pending }) await Task.CompletedTask;
            {
                order!.Status = OrderStatus.Approved;
                await db.SaveChangesAsync(cancellationToken);

                var orderOutbox = await db.OrderOutbox
                    .FirstOrDefaultAsync(x => x.Id == deSerializedOrderOutbox.Id, cancellationToken);

                if (orderOutbox is not { Status: OrderStatus.Pending }) await Task.CompletedTask;

                orderOutbox!.Status = OrderStatus.Approved;
                await db.SaveChangesAsync(cancellationToken);
            }
        }
        catch (Exception)
        {
            await transaction.RollbackAsync(cancellationToken);
        }
    }
}