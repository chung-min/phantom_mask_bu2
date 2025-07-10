using System.ComponentModel.DataAnnotations;

namespace PhantomMask.Api.Models.DTOs
{
    public class MaskStockChangeDTO
    {
        /// <summary>
        /// ID of the pharmacy
        /// </summary>
        public int PharmacyId { get; set; }

        /// <summary>
        /// ID of the mask
        /// </summary>
        public int MaskId { get; set; }

        /// <summary>
        /// The quantity to change the stock by (positive to increase, negative to decrease).
        /// </summary>
        [Range(int.MinValue, int.MaxValue, ErrorMessage = "StockChange must be a valid integer.")]
        public int StockChange { get; set; }
    }
}
