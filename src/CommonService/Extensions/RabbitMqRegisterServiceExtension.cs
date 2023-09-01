using CommonService.Configs;
using CommonService.MessageBrokers.RabbitMq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;


namespace CommonService.Extensions;

public static class RabbitMqRegisterServiceExtension
{
    public static IServiceCollection AddRabbitMqServices(this IServiceCollection services,
        IConfiguration configuration)
    {
        var rabbitMqConnection = configuration.GetSection("RabbitMqConnection").Get<RabbitMqConnection>();
        var rabbitMqConfigs = configuration.GetSection("RabbitMqConfigs").Get<RabbitMqConfigs>();

        if (rabbitMqConnection is null || rabbitMqConfigs?.MetaData is null) return services;

        //make different instances of RabbitMqService for different micro-services


        var orderMicro = rabbitMqConfigs.MetaData?
            .FirstOrDefault(x => (bool)x.MicroServiceName?.Contains("order"));

        var inventoryMicro = rabbitMqConfigs.MetaData?
            .FirstOrDefault(x => (bool)x.MicroServiceName?.Contains("inventory"));

        var paymentMicro = rabbitMqConfigs.MetaData?
            .FirstOrDefault(x => (bool)x.MicroServiceName?.Contains("payment"));

        if (orderMicro != null)
        {
            services.AddSingleton<IRabbitMqService<RabbitMqOrderService>>(_ =>
                new RabbitMqOrderService(rabbitMqConnection, orderMicro));
        }

        if (inventoryMicro != null)
        {
            services.AddSingleton<IRabbitMqService<RabbitMqInventoryService>>(_ =>
                new RabbitMqInventoryService(rabbitMqConnection, inventoryMicro));
        }

        if (paymentMicro != null)
        {
            services.AddSingleton<IRabbitMqService<RabbitMqPaymentService>>(_ =>
                new RabbitMqPaymentService(rabbitMqConnection, paymentMicro));
        }

        return services;
    }
}