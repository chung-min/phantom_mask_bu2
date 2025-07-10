using Microsoft.EntityFrameworkCore;
using PhantomMask.Api.Models;

namespace PhantomMask.Api.Data
{
    public class PhantomMaskDbContext : DbContext
    {
        public PhantomMaskDbContext(DbContextOptions<PhantomMaskDbContext> options) : base(options) { }
        public DbSet<Pharmacies> Pharmacies { get; set; }
        public DbSet<Masks> Masks { get; set; }
        public DbSet<Users> Users { get; set; }
        public DbSet<PurchaseHistory> PurchaseHistory { get; set; }
    }
}
