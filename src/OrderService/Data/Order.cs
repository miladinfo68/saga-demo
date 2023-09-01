using CommonService.Enums;
using OrderService.Data;

namespace OrderService.Models;

public  class Order : BaseModel
{
    public Order()
    {
        OrderItems = new List<OrderItem>();
    }
    public string CustomerName { get; set; } = null!;
    public DateTime OrderDate { get; set; }
    public OrderStatus Status { get; set; }
    public decimal Total { get; set; }
    public string? ProductIds { get; set; }
    public virtual ICollection<OrderItem> OrderItems { get; set; }
}