using CommonService.Models;

namespace CommonService.Extensions;

public static class MoqDataExtension
{
    public static IEnumerable<Product> Products()
    {
        return new List<Product>
        { 
           new Product(1,    "T-Shirt",  30 , 1000),
           new Product(2,    "Suit",  30 , 1000),
           new Product(3,    "Cap",  30 , 1000),
           new Product(4,    "Belt",  30 , 1000),
           new Product(5,    "Gloves",  30 , 1000),
           new Product(6,    "Sweater",  30 , 1000),
           new Product(7,    "Bra",  30 , 1000),
           new Product(8,    "Boots",  30 , 1000),
           new Product(9,    "Jumper",  30 , 1000),
           new Product(10,    "Sockets",  30 , 1000),
           new Product(11,    "Jeans",  30 , 1000),
           new Product(12,    "Dress",  30 , 1000),
           new Product(13,    "Towel",  30 , 1000),
           new Product(14,    "Tie",  30 , 1000),
           new Product(15,    "Shoes",  30 , 1000),
           new Product(16,    "Sandal",  30 , 1000),
           new Product(17,    "Oxford shoe",  30 , 1000),
           new Product(18,    "Brogue shoe",  30 , 1000),
           new Product(19,    "Platform shoe",  30 , 1000),
           new Product(20,    "Clog",  30 , 1000)
        };
    }

}