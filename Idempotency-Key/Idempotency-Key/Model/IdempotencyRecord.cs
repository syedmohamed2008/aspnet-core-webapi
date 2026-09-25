namespace Idempotency_Key.Model
{
    public class IdempotencyRecord
    {
        public int Id { get; set; }

        public string IdempotencyKey { get; set; } = string.Empty;

        public string ResponseBody { get; set; } = string.Empty;

        public int StatusCode { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
