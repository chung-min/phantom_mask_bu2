namespace PhantomMask.Api.Models
{
    public class Masks
    {
        public int id { get; set; }
        public string name { get; set; }
        public decimal price { get; set; }
        public int stockQuantity { get; set; }

        /// <summary>
        /// 對應Pharmacies id
        /// </summary>
        public Pharmacies pharmacies { get; set; }
    }
}
