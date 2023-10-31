namespace Project.Models
{
    public class Transport
    {
        public int TransportId { get; set; }
        public string TransportName { get; set; } = null!;
        public string TransportStatus { get; set; } = null!;
        public virtual List<Bill> Bills { get; set; } = null!;
    }
}
