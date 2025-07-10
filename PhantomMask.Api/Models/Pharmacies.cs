namespace PhantomMask.Api.Models
{
    public class Pharmacies
    {
        public int id { get; set; }
        public string name { get; set; }
        public decimal cashBalance { get; set; }
        public string openingHours { get; set; }
        public List<Masks> masks { get; set; } = new();
    }
}
