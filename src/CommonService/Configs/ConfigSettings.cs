namespace CommonService.Configs;

public class ConnectionStrings
{
    public string OrderDbConnection { get; set; }
    public string InventoryDbConnection { get; set; }
    public string PaymentDbConnection { get; set; }
}

public class RabbitMqConnection
{
    public string Uri { get; set; }
    public string HostName { get; set; }
    public string UserName { get; set; }
    public string Password { get; set; }
}

public class RabbitMqConfigs
{
    public List<RabbitMqMetaData> MetaData { get; set; }
}

public class RabbitMqMetaData
{
    public string MicroServiceName { get; set; }
    public string ExchangeType { get; set; }
    public string ExchangeName { get; set; }
    public string QueueName { get; set; }
    public string RoutingKey { get; set; }
}





