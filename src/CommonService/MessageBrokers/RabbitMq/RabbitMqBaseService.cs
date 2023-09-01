using RabbitMQ.Client;
using System.Text;
using CommonService.Configs;
using RabbitMQ.Client.Events;
using Newtonsoft.Json;

namespace CommonService.MessageBrokers.RabbitMq;
//https://www.gokhan-gokalp.com/en/implementation-of-choreography-based-saga-in-dotnet-microservices/

public interface IRabbitMqService<TService> where TService : class
{
    bool SendMessage(object data, string? routingKey = null, IDictionary<string, object>? headers = null);

    Task<IEnumerable<string>> ReceiveMessages(string? routingKey = null, IDictionary<string, object>? headers = null,
        Func<object, bool>? messageHandler = null);

    bool IsAliveRabbitMqServer();
}

public abstract class RabbitMqBaseService<TService> : IDisposable ,
    IRabbitMqService<TService> where TService : class
{
    private readonly RabbitMqMetaData _rabbitMqMetaData;
    private readonly IConnection _connection;
    private readonly IModel _channel;

    protected RabbitMqBaseService(RabbitMqConnection rabbitMqConfigs, RabbitMqMetaData rabbitMqMetaData)
    {
        _rabbitMqMetaData = rabbitMqMetaData;

        var factory = new ConnectionFactory
        {
            Uri = new Uri(rabbitMqConfigs.Uri),

            //HostName = _rabbitMqConfigs.HostName,
            //UserName = _rabbitMqConfigs.UserName,
            //Password = _rabbitMqConfigs.Password,
        };

        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();

        SetupExchangeAndQueueBindings();
    }

    private void SetupExchangeAndQueueBindings()
    {
        _channel.ExchangeDeclare(_rabbitMqMetaData.ExchangeName, _rabbitMqMetaData.ExchangeType);

        if (_rabbitMqMetaData.ExchangeType == ExchangeType.Fanout) return;

        _channel.QueueDeclare(_rabbitMqMetaData.QueueName, durable: false, exclusive: false, autoDelete: false,
            arguments: null);
        _channel.QueueBind(_rabbitMqMetaData.QueueName, _rabbitMqMetaData.ExchangeName, _rabbitMqMetaData.RoutingKey,
            null);
    }

    public bool IsAliveRabbitMqServer() => _connection.IsOpen;

    public bool SendMessage(object data, string? routingKey = null, IDictionary<string, object>? headers = null)
    {
        var result = false;
        try
        {
            var message = JsonConvert.SerializeObject(data);
            var body = Encoding.UTF8.GetBytes(message);

            var properties = _channel.CreateBasicProperties();
            properties.Headers = headers;

            var routeKey = _rabbitMqMetaData.RoutingKey;

            if (!string.IsNullOrEmpty(routingKey))
            {
                routeKey = routingKey.TrimStart('.');
            }

            _channel.BasicPublish(_rabbitMqMetaData.ExchangeName, routeKey, properties, body);
            result = true;
        }
        catch (Exception)
        {
            // ignored
        }

        return result;
    }

    public async Task<IEnumerable<string>> ReceiveMessages(string? routingKey = null,
        IDictionary<string, object>? headers = null, Func<object, bool>? messageHandler = null)
    {
        try
        {
            if (string.IsNullOrEmpty(_rabbitMqMetaData.QueueName))
                return Enumerable.Empty<string>();

            var messages = new List<string>();
            var consumer = new AsyncEventingBasicConsumer(_channel);
            consumer.Received += (sender, args) =>
            {
                var message = Encoding.UTF8.GetString(args.Body.ToArray());
                messages.Add(message);
                messageHandler?.Invoke(message);
                return Task.CompletedTask;
            };
            _channel.BasicConsume(_rabbitMqMetaData.QueueName, true, consumer);
            await Task.Delay(1000); // Wait for 1 second to allow all messages to be consumed
            return messages;
        }
        catch (Exception)
        {
            return Enumerable.Empty<string>();
        }
    }

    public void Dispose()
    {
        _channel?.Close();
        _connection?.Close();
    }
}