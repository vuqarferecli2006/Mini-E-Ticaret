namespace E_Biznes.Domain.Entities;

public class OrderProduct
{
    public Guid OrderId { get; set; }
    public Order? Order { get; set; }

    public Guid ProductId { get; set; }
    public Product? Product { get; set; }

    public int Quantity { get; set; }  

    public decimal UnitPrice { get; set; }
}
