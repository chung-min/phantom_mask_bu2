namespace PhantomMask.Api.Models
{
    public class Users
    {
        public int id { get; set; }
        public string name { get; set; }
        public decimal cashBalance { get; set; }
        public List<PurchaseHistory> purchaseHistories { get; set; } = new();
    }
}
