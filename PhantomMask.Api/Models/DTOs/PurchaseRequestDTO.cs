namespace PhantomMask.Api.Models.DTOs
{
    public class PurchaseRequestDTO
    {
        public int PharmacyId { get; set; }
        public int MasksId { get; set; }
        public int TransactionQuantity { get; set; }
    }
}
