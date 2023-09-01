
using CommonService.Configs;

namespace CommonService.MessageBrokers.RabbitMq;

public class RabbitMqInventoryService : RabbitMqBaseService<RabbitMqInventoryService>
{
    public RabbitMqInventoryService(RabbitMqConnection rabbitMqConfigs, RabbitMqMetaData rabbitMqMetaData) : base(rabbitMqConfigs, rabbitMqMetaData)
    {
    }
}