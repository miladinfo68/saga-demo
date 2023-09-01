using CommonService.MessageBrokers.RabbitMq;

namespace StockService.Jobs;

public class JobReceivedOrders : BackgroundService
{
    private readonly CancellationToken _cancellationToken;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly IRabbitMqService<RabbitMqOrderService> _rabbitMqOrderService;
    private readonly IRabbitMqService<RabbitMqInventoryService> _rabbitMqInventoryService;


    public JobReceivedOrders(
        IHostApplicationLifetime applicationLifetime,
        IServiceScopeFactory serviceScopeFactory,
        IRabbitMqService<RabbitMqOrderService> rabbitMqOrderService,
        IRabbitMqService<RabbitMqInventoryService> rabbitMqInventoryService)
    {
        _cancellationToken = applicationLifetime.ApplicationStopping;
        _serviceScopeFactory = serviceScopeFactory;
        _rabbitMqOrderService = rabbitMqOrderService;
        _rabbitMqInventoryService = rabbitMqInventoryService;
    }


    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var receivedMessages = await _rabbitMqOrderService.ReceiveMessages();
        var messagesList = receivedMessages.ToList();

        if (!messagesList.Any()) await Task.CompletedTask;

        while (!_cancellationToken.IsCancellationRequested && messagesList.Any())
        {
            foreach (var message in messagesList)
            {
                //await UpdateOrderStatusAsync(orderDb, message, _cancellationToken);
                messagesList.Remove(message);
            }
        }
    }
}