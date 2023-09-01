using CommonService.Configs;

namespace CommonService.MessageBrokers.RabbitMq;

public class RabbitMqPaymentService : RabbitMqBaseService<RabbitMqPaymentService>
{
    public RabbitMqPaymentService(RabbitMqConnection rabbitMqConfigs, RabbitMqMetaData rabbitMqMetaData) : base(rabbitMqConfigs, rabbitMqMetaData)
    {
    }
}