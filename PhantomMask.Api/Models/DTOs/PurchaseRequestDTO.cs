namespace PhantomMask.Api.Models.DTOs
{
    public class PurchaseRequestDTO
    {
        /// <summary>
        /// ID of the pharmacy
        /// </summary>
        public int PharmacyId { get; set; }

        /// <summary>
        /// ID of the mask
        /// </summary>
        public int MasksId { get; set; }

        /// <summary>
        /// Transaction Quantity
        /// </summary>
        public int TransactionQuantity { get; set; }
    }
}
