using CommonService.Enums;
using OrderService.Data;

namespace OrderService.Models;

public class Outbox : BaseModel
{
    public string EventType { get; set; }
    public string EventData { get; set; }
    public OrderStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }=DateTime.Now;
}

public class OrderOutbox : Outbox
{
}