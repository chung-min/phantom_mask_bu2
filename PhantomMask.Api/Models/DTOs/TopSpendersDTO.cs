namespace PhantomMask.Api.Models.DTOs
{
    public class TopSpendersDTO
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public decimal TotalSpent { get; set; }
    }
}
