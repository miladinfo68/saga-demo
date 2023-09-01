/*using RabbitMQ.Client;
using System.Text;
using RabbitMQ.Client.Events;
using CommonService.Configs;

namespace CommonService.MessageBrokers.RabbitMq;

public interface IRabbitMqService2
{
    Task SendMessage(string message, string? exchangeType, string? routingKey);
    Task ReceiveMessages(Task<Action<string>> messageHandler, string? queueName);
}




public class RabbitMqService2 : IRabbitMqService2, IDisposable
{
    private readonly IConnection _connection;
    private readonly IModel _channel;
    private readonly RabbitMqConnection _rabbitMqConfigs;

    public RabbitMqService2(RabbitMqConnection rabbitMqConfigs)
    {
        _rabbitMqConfigs = rabbitMqConfigs;

        var factory = new ConnectionFactory
        {
            HostName = _rabbitMqConfigs.HostName,
            UserName = _rabbitMqConfigs.UserName,
            Password = _rabbitMqConfigs.Password
        };

        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();
    }

    public async Task SendMessage(string message, string? exchangeName, string? routingKey)
    {
        var body = Encoding.UTF8.GetBytes(message);
        var xChangeName = string.IsNullOrEmpty(exchangeName) ? _rabbitMqConfigs.ExchangeType : exchangeName;
        var routKey = string.IsNullOrEmpty(exchangeName) ? _rabbitMqConfigs.RoutingKey : routingKey;

        _channel.BasicPublish(
            exchange: xChangeName,
            routingKey: routKey,
            basicProperties: null,
            body: body
        );
        await Task.CompletedTask;
    }

    public async Task ReceiveMessages(Task<Action<string>> messageHandler, string? queueName)
    {
        var qName = string.IsNullOrEmpty(queueName) ? _rabbitMqConfigs.QueueName : queueName;

        _channel.QueueDeclare(
            queue: qName,
            durable: false,
            exclusive: false,
            autoDelete: false,
            arguments: null
        );

        var consumer = new EventingBasicConsumer(_channel);

        consumer.Received += async (sender, args) =>
        {
            var message = Encoding.UTF8.GetString(args.Body.ToArray());
            (await messageHandler)(message);
        };

        _channel.BasicConsume(
            queue: queueName,
            autoAck: true,
            consumer: consumer
        );
        await Task.CompletedTask;
    }

    public void Dispose()
    {
        _channel.Close();
        _connection.Close();
    }
}*/