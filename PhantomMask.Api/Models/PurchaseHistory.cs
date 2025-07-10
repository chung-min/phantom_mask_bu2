namespace PhantomMask.Api.Models
{
    public class PurchaseHistory
    {
        public int id { get; set; }
        public string pharmacyName { get; set; }
        public string maskName { get; set; }
        public decimal transactionAmount { get; set; }
        public int transactionQuantity { get; set; }
        public DateTime transactionDatetime { get; set; }
        public Users users { get; set; }
        public int pharmacyId { get; set; }
        public int masksId { get; set; }
    }
}
