public class Otp
{
    public int Id { get; set; }
    public required string Identifier { get; set; }
    public required string Code { get; set; }
    public DateTime ExpiryTime { get; set; }
}