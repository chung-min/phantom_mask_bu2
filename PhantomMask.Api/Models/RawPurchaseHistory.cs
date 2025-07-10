namespace PhantomMask.Api.Models
{
    public class RawPurchaseHistory
    {
        public int id { get; set; }
        public string pharmacyName { get; set; }
        public string maskName { get; set; }
        public decimal transactionAmount { get; set; }
        public int transactionQuantity { get; set; }
        public string transactionDatetime { get; set; }
        public Users users { get; set; }
    }
}
