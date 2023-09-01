using CommonService.Configs;

namespace CommonService.MessageBrokers.RabbitMq;

public class RabbitMqOrderService: RabbitMqBaseService<RabbitMqOrderService>
{
    public RabbitMqOrderService(RabbitMqConnection rabbitMqConfigs, RabbitMqMetaData rabbitMqMetaData) : base(rabbitMqConfigs, rabbitMqMetaData)
    {
    }
}