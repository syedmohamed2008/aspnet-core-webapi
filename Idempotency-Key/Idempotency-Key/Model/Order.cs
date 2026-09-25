namespace Idempotency_Key.Model
{
    public class Order
    {
        public Guid Id { get; set; }
        public string ProductName { get; set; } = string.Empty;
    }
}
