using OrderService.Data;

namespace OrderService.Models;

public  class Product : BaseModel
{
    public Product()
    {
        OrderItems = new List<OrderItem>();
    }
    public string Name { get; set; } = null!;
    public int Quantity { get; set; }
    public decimal PricePerUnit { get; set; }
    public virtual ICollection<OrderItem> OrderItems { get; set; }
}