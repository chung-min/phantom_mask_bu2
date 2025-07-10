namespace PhantomMask.Api.Models.DTOs
{
    public class MaskSaveDTO
    {
        /// <summary>
        /// ID of the mask
        /// </summary>
        public int? MaskId { get; set; }

        /// <summary>
        /// Name of the mask product
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// Price of the mask product (in your currency)
        /// </summary>
        public decimal? Price { get; set; }

        /// <summary>
        /// Stock quantity available for the mask product
        /// </summary>
        public int? StockQuantity { get; set; }
    }
}
