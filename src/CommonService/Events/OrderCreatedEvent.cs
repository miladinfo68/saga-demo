namespace CommonService.Events;

// public record OrderCreatedEvent(int OrderId,string CustomerName);
// public record InventoryReservedEvent(int OrderId);
// public record PaymentProcessedEvent(int OrderId);

public static class BusinessEvents
{
    public const string ORDER_ADDED = "Order-Added";
    public const string ORDER_UPDATED = "Order-Updated";
}