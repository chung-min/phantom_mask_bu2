namespace PhantomMask.Api.Models
{
    public class RawUsers
    {
        public int id { get; set; }
        public string name { get; set; }
        public decimal cashBalance { get; set; }
        public List<RawPurchaseHistory> purchaseHistories { get; set; } = new();
    }
}
