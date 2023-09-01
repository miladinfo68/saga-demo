namespace StockService.Data;

public class Product : BaseModel
{
    public string Name { get; set; }
    public decimal PricePerUnit { get; set; }
    public int Quantity { get; set; }
}