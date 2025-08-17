public class Order
{
    public int Id { get; set; }
    public Guid UserId { get; set; }
    public string Item { get; set; }
    public int Quantity { get; set; }
    public DateTime OrderDate { get; set; }
    public decimal TotalPrice { get; set; }
}
