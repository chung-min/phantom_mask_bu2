using Azure.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using PhantomMask.Api.Data;
using PhantomMask.Api.Helpers;
using PhantomMask.Api.Models;
using PhantomMask.Api.Models.DTOs;
using System.Globalization;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace PhantomMask.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PhantomMaskController : ControllerBase
    {
        PhantomMaskHelper phantomMaskHelper = new PhantomMaskHelper();

        private readonly PhantomMaskDbContext _context;

        public PhantomMaskController(PhantomMaskDbContext context)
        {
            _context = context;
        }

        #region 1.List pharmacies, optionally filtered by specific time and/or day of the week.
        [HttpGet("pharmacies")]
        public async Task<IActionResult> GetPharmacies([FromQuery] string? day = null, [FromQuery] string? time = null)
        {
            var pharmacies = await _context.Pharmacies.ToListAsync();

            if (pharmacies == null || !pharmacies.Any())
            {
                return NotFound("No pharmacies found.");
            }

            if (!string.IsNullOrWhiteSpace(day) || !string.IsNullOrWhiteSpace(time))
            {
                //Both day and time must be specified to filter pharmacies.
                pharmacies = pharmacies.Where(p => phantomMaskHelper.IsOpenAt(p.openingHours, day, time)).ToList();
            }

            return Ok(pharmacies);
        }
        #endregion

        #region 2.List all masks sold by a given pharmacy with an option to sort by name or price.
        [HttpGet("pharmacies/{pharmacyId}/masks")]
        public async Task<IActionResult> GetMasksByPharmacy(int pharmacyId, [FromQuery] string? sortBy = null)
        {
            var pharmacy = await _context.Pharmacies.Include(p => p.masks).FirstOrDefaultAsync(p => p.id == pharmacyId);

            if (pharmacy == null)
            {
                return NotFound($"Pharmacy with id {pharmacyId} not found.");
            }

            // sortBy can be "name" or "price", default is no sorting
            var sortedMasks = sortBy switch
            {
                "name" => pharmacy.masks.OrderBy(m => m.name).ToList(),
                "price" => pharmacy.masks.OrderBy(m => m.price).ToList(),
                _ => pharmacy.masks
            };

            var result = sortedMasks.Select(m => new MaskDTO
            {
                Id = m.id,
                Name = m.name,
                Price = m.price,
                StockQuantity = m.stockQuantity
            }).ToList();

            return Ok(result);
        }
        #endregion

        #region 3.List all pharmacies that offer a number of mask products within a given price range, where the count is above, below, or between given thresholds.
        [HttpGet("pharmacies/mask-count-by-price-range")]
        public async Task<IActionResult> GetPharmaciesByMaskPriceAndCount([FromQuery] decimal? minPrice = null, [FromQuery] decimal? maxPrice = null, [FromQuery] int? countMin = null, [FromQuery] int? countMax = null)
        {
            var pharmacies = await _context.Masks.Include(m => m.pharmacies)
                                                 .Where(m => (!minPrice.HasValue || m.price >= minPrice) && (!maxPrice.HasValue || m.price <= maxPrice))
                                                 .Where(m => (!countMin.HasValue || m.stockQuantity >= countMin) && (!countMax.HasValue || m.stockQuantity <= countMax))
                                                 .ToListAsync();

            var filteredPharmacies = pharmacies.GroupBy(p => p.pharmacies.id)
                                               .Select(p => new
                                               {
                                                   Pharmacy = p.First().pharmacies,
                                                   MaskCount = p.Count()
                                               }).Select(p => p.Pharmacy).ToList();

            var result = filteredPharmacies.Select(p => new PharmaciesDTO
            {
                Id = p.id,
                Name = p.name,
                CashBalance = p.cashBalance,
                OpeningHours = p.openingHours
            }).ToList();


            return Ok(result);
        }
        #endregion

        #region 4.Show the top N users who spent the most on masks during a specific date range.
        [HttpGet("users/top-spenders")]
        public async Task<IActionResult> GetTopSpenders([FromQuery] string dateS, [FromQuery] string dateE, [FromQuery] int? topN = null)
        {
            if (!DateTime.TryParseExact(dateS + "000000", "yyyyMMddHHmmss", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime startDate) ||
                !DateTime.TryParseExact(dateE + "235959", "yyyyMMddHHmmss", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime endDate))
            {
                return BadRequest("Invalid date format. Please use 'yyyyMMdd' format for both start and end dates.");
            }

            var TopSpenders = await _context.PurchaseHistory.Where(hist => hist.transactionDatetime >= startDate && hist.transactionDatetime <= endDate)
                                                            .GroupBy(ph => new { ph.users.id, ph.users.name })
                                                            .Select(g => new
                                                            {
                                                                UserId = g.Key.id,
                                                                UserName = g.Key.name,
                                                                TotalSpent = g.Sum(ph => ph.transactionAmount)
                                                            }).OrderByDescending(x => x.TotalSpent).ToListAsync();

            var result = TopSpenders.Select(t => new TopSpendersDTO
            {
                UserId = t.UserId,
                UserName = t.UserName,
                TotalSpent = t.TotalSpent
            }).ToList();

            return Ok(result);
        }
        #endregion

        #region 5.Process a purchase where a user buys masks from multiple pharmacies at once.
        [HttpPost("users/{userId}/purchase")]
        public async Task<IActionResult> ProcessPurchase(int userId, [FromBody] List<PurchaseRequestDTO> request)
        {
            if (request == null || !request.Any())
            {
                return BadRequest("Invalid purchase request.");
            }

            var user = await _context.Users.Include(u => u.purchaseHistories).FirstOrDefaultAsync(u => u.id == userId);
            if (user == null)
            {
                return NotFound($"User with id {userId} not found. Please create the user using POST /api/users before making a purchase.");
            }

            var errors = new List<string>();
            var pharmacyIds = request.Select(r => r.PharmacyId).Distinct().ToList();
            var pharmacies = await _context.Pharmacies.Include(p => p.masks).Where(p => pharmacyIds.Contains(p.id)).ToListAsync();
            decimal totalAmount = 0;
            var validPurchases = new List<(PurchaseRequestDTO purchase, Pharmacies pharmacy, Masks mask, decimal cost)>();

            #region [ error handling ]
            foreach (var purchase in request)
            {
                if (purchase.TransactionQuantity <= 0)
                {
                    errors.Add($"Invalid quantity for mask id {purchase.MasksId} in pharmacy id {purchase.PharmacyId}.");
                    continue;
                }

                var pharmacy = pharmacies.FirstOrDefault(p => p.id == purchase.PharmacyId);
                if (pharmacy == null)
                {
                    errors.Add($"Pharmacy with id {purchase.PharmacyId} not found.");
                    continue;
                }

                var mask = pharmacy.masks.FirstOrDefault(m => m.id == purchase.MasksId);
                if (mask == null)
                {
                    errors.Add($"Mask with id {purchase.MasksId} not found in pharmacy {pharmacy.name} (id {pharmacy.id}).");
                    continue;
                }

                if (mask.stockQuantity < purchase.TransactionQuantity)
                {
                    errors.Add($"Insufficient stock for mask id {purchase.MasksId} in pharmacy {pharmacy.name} (id {pharmacy.id}).");
                    continue;
                }

                var cost = mask.price * purchase.TransactionQuantity;
                totalAmount += cost;

                validPurchases.Add((purchase, pharmacy, mask, cost));
            }

            if (user.cashBalance < totalAmount)
            {
                errors.Add("Insufficient cash balance for this purchase.");
            }

            if (errors.Any())
            {
                return BadRequest(new { Message = "Purchase failed.", Errors = errors });
            }
            #endregion

            using var transaction = await _context.Database.BeginTransactionAsync();
            foreach (var (purchase, pharmacy, mask, cost) in validPurchases)
            {
                pharmacy.cashBalance += cost; // update pharmacy's cash balance
                mask.stockQuantity -= purchase.TransactionQuantity; // update mask stock quantity

                // add purchase histories
                user.purchaseHistories.Add(new PurchaseHistory
                {
                    pharmacyName = pharmacy.name,
                    maskName = mask.name,
                    transactionAmount = cost,
                    transactionQuantity = purchase.TransactionQuantity,
                    transactionDatetime = DateTime.UtcNow,
                    users = user,
                    pharmacyId = pharmacy.id,
                    masksId = mask.id
                });
            }

            user.cashBalance -= totalAmount; // update user's cash balance

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return Ok(new { TotalAmount = totalAmount, Message = "Purchase successful." });
        }
        #endregion

        #region 6.Update the stock quantity of an existing mask product by increasing or decreasing it.
        [HttpPut("masks/stock/batch")]
        public async Task<IActionResult> UpdateMultipleMaskStocks([FromBody] List<MaskStockChangeDTO> stockChanges)
        {
            if (stockChanges == null || !stockChanges.Any())
            {
                return BadRequest("No stock changes provided.");
            }

            var errors = new List<string>();
            using var transaction = await _context.Database.BeginTransactionAsync();
            foreach (var change in stockChanges)
            {
                #region [ error handling ]
                if (change.StockChange == 0)
                {
                    errors.Add($"Stock change cannot be zero (mask id {change.MaskId}, pharmacy id {change.PharmacyId}).");
                    continue;
                }

                var mask = await _context.Masks.FirstOrDefaultAsync(m => m.id == change.MaskId && m.pharmacies.id == change.PharmacyId);
                if (mask == null)
                {
                    errors.Add($"Mask with id {change.MaskId} not found in pharmacy {change.PharmacyId}.");
                    continue;
                }

                int newStock = mask.stockQuantity + change.StockChange; // Calculate new stock quantity
                if (newStock < 0)
                {
                    errors.Add($"Insufficient stock for mask {change.MaskId}. Current: {mask.stockQuantity}, Change: {change.StockChange}.");
                    continue;
                }
                #endregion

                mask.stockQuantity = newStock; // Update stock quantity
            }

            if (errors.Any()) // If there are any errors, rollback the transaction and return the errors
            {
                await transaction.RollbackAsync();
                return BadRequest(new { Message = "Some stock changes failed.", Errors = errors });
            }

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return Ok(new { Message = "All stock changes applied successfully." });
        }
        #endregion

        #region 7.Create or update multiple mask products for a pharmacy at once, including name, price, and stock quantity.
        [HttpPost("pharmacies/{pharmacyId}/masks/batch")]
        public async Task<IActionResult> CreateOrUpdateMasks(int pharmacyId, [FromBody] List<MaskSaveDTO> masks)
        {
            #region [ error handling ]
            if (masks == null || !masks.Any())
            {
                return BadRequest("No masks provided.");
            }

            var pharmacy = await _context.Pharmacies.Include(p => p.masks).FirstOrDefaultAsync(p => p.id == pharmacyId);
            if (pharmacy == null)
            {
                return NotFound($"Pharmacy with id {pharmacyId} not found.");
            }

            var errors = new List<string>();
            foreach (var dto in masks)
            {
                var existing = pharmacy.masks.FirstOrDefault(m => m.id == dto.MaskId);
                if (existing != null)
                {
                    if (string.IsNullOrWhiteSpace(dto.Name) && dto.Price <= 0 && dto.StockQuantity < 0)
                    {
                        errors.Add($"Invalid data for mask: {dto.Name}, Price: {dto.Price}, Stock: {dto.StockQuantity}.");
                        continue;
                    }

                    // Check for duplicate names only for new masks
                    bool duplicateName = pharmacy.masks.Any(m => m.id != dto.MaskId && m.name == dto.Name);
                    if (duplicateName)
                    {
                        errors.Add($"A mask named '{dto.Name}' already exists in pharmacy '{pharmacy.name}' (id: {pharmacy.id}). Please choose a different name.");
                        continue;
                    }
                }
                else
                {
                    if (dto.MaskId != null)
                    {
                        errors.Add($"Mask with id {dto.MaskId} does not exist in pharmacy '{pharmacy.name}' (id: {pharmacy.id}). Please remove the 'Id' field and resend if you intend to create a new mask.");
                        continue;
                    }
                    else if (string.IsNullOrWhiteSpace(dto.Name) || dto.Price == null || dto.Price <= 0 || dto.StockQuantity == null || dto.StockQuantity < 0)
                    {
                        errors.Add($"Invalid data for new mask: {dto.Name}, Price: {dto.Price}, Stock: {dto.StockQuantity}.");
                        continue;
                    }

                    // Check for duplicate names only for new masks
                    bool duplicateName = pharmacy.masks.Any(m => m.name == dto.Name);
                    if (duplicateName)
                    {
                        errors.Add($"A mask named '{dto.Name}' already exists in pharmacy '{pharmacy.name}'. Please use a different name.");
                        continue;
                    }
                }
            }

            if (errors.Any())
            {
                return BadRequest(new { Message = "Some masks failed to process.", Errors = errors });
            }
            #endregion

            using var transaction = await _context.Database.BeginTransactionAsync();
            foreach (var dto in masks)
            {
                var existing = pharmacy.masks.FirstOrDefault(m => m.id == dto.MaskId);

                #region [ Update ]
                if (existing != null)
                {
                    // Update
                    if (!string.IsNullOrWhiteSpace(dto.Name))
                    {
                        existing.name = dto.Name;
                    }
                    if (dto.Price != null)
                    {
                        existing.price = dto.Price.Value;
                    }
                    if (dto.StockQuantity != null)
                    {
                        existing.stockQuantity = dto.StockQuantity.Value;
                    }
                }
                #endregion

                #region [ Create ]
                else
                {
                    pharmacy.masks.Add(new Masks
                    {
                        name = dto.Name,
                        price = dto.Price.Value,
                        stockQuantity = dto.StockQuantity.Value,
                        pharmacies = pharmacy
                    });
                }
                #endregion
            }

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return Ok(new { Message = "Masks processed successfully." });
        }
        #endregion

        #region 8.Search for pharmacies or masks by name and rank the results by relevance to the search term.
        [HttpGet("pharmacies-and-masks/search")]
        public async Task<IActionResult> SearchPharmaciesAndMasks([FromQuery] string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return BadRequest("Search query cannot be empty.");
            }

            var pharmacies = await _context.Pharmacies.Where(p => p.name.ToLower().Contains(query.ToLower())).ToListAsync();
            var masks = await _context.Masks.Include(m => m.pharmacies).Where(m => m.name.ToLower().Contains(query.ToLower())).ToListAsync();

            var results = new List<SearchPharmaciesAndMasksDTO>();
            results.AddRange(pharmacies.Select(p => new SearchPharmaciesAndMasksDTO
            {
                Type = "Pharmacy",
                Id = p.id,
                Name = p.name,
                Description = $"Cash Balance: {p.cashBalance}, Opening Hours: {p.openingHours}"
            }));

            results.AddRange(masks.Select(m => new SearchPharmaciesAndMasksDTO
            {
                Type = "Mask",
                Id = m.id,
                Name = m.name,
                Description = $" Pharmacy: {m.pharmacies.name}, Price: {m.price}, Stock Quantity: {m.stockQuantity}"
            }));

            results = results.OrderByDescending(r => r.Name.ToLower().Contains(query.ToLower())).ThenBy(r => r.Name).ToList();

            return Ok(results);
        }
        #endregion
    }
}
