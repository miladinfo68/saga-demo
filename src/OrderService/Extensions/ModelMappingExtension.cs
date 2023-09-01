using Newtonsoft.Json;
using CommonService.Dtos;
using CommonService.Enums;
using CommonService.Events;
using OrderService.Models;

namespace OrderService.Extensions;

public static class ModelMappingExtension
{
    public static Order AsOrder(this AddOrderDto dto ,IEnumerable<OrderItemDto> OrderItemsDto)
    {
        return new Order
        {
            CustomerName = dto.CustomerName,
            OrderDate = DateTime.UtcNow,
            Status = OrderStatus.Pending,
            Total = OrderItemsDto.Sum(s => s.PricePerUnit * s.Quantity),
            ProductIds = string.Join(",", OrderItemsDto.Select(s => s.ProductId))
        };
    }

    public static IEnumerable<OrderItem> AsOrderItems(this IEnumerable<OrderItemDto> orderItemsDto , int orderId)
    {
        var orderItems= orderItemsDto.Select(s => new OrderItem
        {
            //OrderId = orderId,
            ProductId = s.ProductId,
            Quantity = s.Quantity ,
            PricePerUnit = s.PricePerUnit
        }).ToList();
        return orderItems;
    }


    private static OutboxOrderDto AsOutboxOrderDto(this Order order,IEnumerable<OrderItemDto> orderItemsDto)
    {
        return new OutboxOrderDto(
            order.Id,
            order.CustomerName,
            orderItemsDto , 
            order.Status,
            order.Total,
            order.OrderDate
        );
    }

    public static OrderOutbox AsAddedOrderOutbox(this Order order,IEnumerable<OrderItemDto> orderItemsDto)
    {
        var outboxOrderDto = order.AsOutboxOrderDto(orderItemsDto);
        return new OrderOutbox
        {
            EventType = BusinessEvents.ORDER_ADDED,
            EventData = JsonConvert.SerializeObject(outboxOrderDto),
            Status = OrderStatus.Pending,
            CreatedAt = DateTime.Now
        };
    }
}


