namespace AgriMartAPI.Models
{
    public class PaymentTransaction
    {
        public Guid Id { get; set; }
        public Guid? OrderId { get; set; }
        public Guid UserId { get; set; }
        public Guid? PaymentMethodId { get; set; }
        public string? GatewayTransactionId { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; } = "INR";
        public DateTime TransactionDate { get; set; }
        public string? ResponseMessage { get; set; }
        public int? StatusId { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
    }
}