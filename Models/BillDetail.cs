    namespace Project.Models
{
    public class BillDetail
    {   
        public int BillDetailId { get; set; }
        public int BillId { get; set; }
        public int ProductId { get; set; }
        public string color { get; set; } = null!;
        public string size { get; set; } = null!;
        public int quantity { get; set; }
        public bool IsFeedbackSubmitted { get; set; } = false;
        public virtual Bill Bill { get; set; } = null!;
        public virtual Product Product { get; set; } = null!;

    }
}
