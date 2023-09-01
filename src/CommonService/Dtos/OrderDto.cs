using CommonService.Enums;

namespace CommonService.Dtos;

public record AddOrderDto(string CustomerName, IEnumerable<OrderItemDto> OrderItemsDto);
public record OrderItemDto(int ProductId, int Quantity, decimal PricePerUnit);

public  record OutboxOrderDto(
    int OrderId, 
    string CustomerName, 
    IEnumerable<OrderItemDto> OrderItemsDto , 
    OrderStatus Status, 
    decimal Total,
    DateTime OrderDate);


public record InventoryOrderDto(int OrderId, int ProductId, int Quantity, int? Supply,DateTime? OrderDate);



