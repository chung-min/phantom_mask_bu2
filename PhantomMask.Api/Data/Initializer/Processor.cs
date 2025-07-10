using Microsoft.EntityFrameworkCore;
using PhantomMask.Api.Models;
using System.Globalization;
using System.Text.Json;

namespace PhantomMask.Api.Data.Initializer
{
    public class Processor
    {
        private readonly PhantomMaskDbContext _context;

        public Processor(PhantomMaskDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// 從json載入pharmacies並寫入資料庫
        /// </summary>
        public async Task LoadPharmaciesAsync(string filePath)
        {
            var json = await File.ReadAllTextAsync(filePath);
            var pharmacies = JsonSerializer.Deserialize<List<Pharmacies>>(json);

            if (pharmacies != null && pharmacies.Any())
            {
                _context.Pharmacies.AddRange(pharmacies);
                await _context.SaveChangesAsync();
            }
        }

        public async Task LoadUsersAsync(string filePath)
        {
            var json = await File.ReadAllTextAsync(filePath);
            var rawUsers = JsonSerializer.Deserialize<List<RawUsers>>(json);

            if (rawUsers == null || !rawUsers.Any())
            {
                return;
            }

            var users = new List<Users>();
            var pharmacies = _context.Pharmacies.Include(p => p.masks).ToList();
            foreach (var raw in rawUsers)
            {
                var user = new Users
                {
                    name = raw.name,
                    cashBalance = raw.cashBalance,
                    purchaseHistories = new List<PurchaseHistory>()
                };

                foreach (var hist in raw.purchaseHistories)
                {
                    var pharmacy = pharmacies.FirstOrDefault(p => p.name == hist.pharmacyName);
                    var mask = pharmacy?.masks.FirstOrDefault(m => m.name == hist.maskName);

                    if (pharmacy == null || mask == null)
                    {
                        continue; // 對應不到就跳過
                    }

                    user.purchaseHistories.Add(new PurchaseHistory
                    {
                        pharmacyName = hist.pharmacyName,
                        maskName = hist.maskName,
                        transactionAmount = hist.transactionAmount,
                        transactionQuantity = hist.transactionQuantity,
                        transactionDatetime = DateTime.ParseExact(hist.transactionDatetime, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture),
                        pharmacyId = pharmacy.id,
                        masksId = mask.id
                    });
                }

                users.Add(user);
            }

            _context.Users.AddRange(users);
            await _context.SaveChangesAsync();
        }
    }
}
