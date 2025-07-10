namespace PhantomMask.Api.Models.DTOs
{
    public class MaskSaveDTO
    {
        public int? MaskId { get; set; }
        public string? Name { get; set; }
        public decimal? Price { get; set; }
        public int? StockQuantity { get; set; }
    }
}
