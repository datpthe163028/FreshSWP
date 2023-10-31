namespace Project.Models
{
    public class Payments
    {
        public int PaymentCode { get; set; }
        public string PaymentName { get; set; } = null!;
        public string PaymentStatus { get; set; } = null!;

        public virtual List<Bill> Bills { get; set; } = null!;
    }
}
