public class Order {
    public int OrderId {get;set;}
    public int ClientId { get;set;}
    public Client Client { get; set;}
    public int ItemId { get; set;}
    public Item Item { get; set;}
}