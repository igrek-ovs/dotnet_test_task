using Microsoft.EntityFrameworkCore;
using TestTask.Data;
using TestTask.Data.Entities;
using TestTask.Services.DTO;

namespace TestTask.Services;

public class MarketService
{
    private readonly TestDbContext _testDbContext;
    private static readonly SemaphoreSlim Semaphore = new SemaphoreSlim(1, 1);

    public MarketService(TestDbContext testDbContext)
    {
        _testDbContext = testDbContext;
    }

    public async Task BuyAsync(int userId, int itemId)
    {
        await Semaphore.WaitAsync();
        using var transaction = await _testDbContext.Database.BeginTransactionAsync(System.Data.IsolationLevel.RepeatableRead);

        try
        {

            var user = await _testDbContext.Users
                .Where(u => u.Id == userId)
                .FirstOrDefaultAsync();

            if (user == null)
                throw new Exception("User not found");

            var item = await _testDbContext.Items
                .Where(i => i.Id == itemId)
                .FirstOrDefaultAsync();

            if (item == null)
                throw new Exception("Item not found");

            if (user.Balance < item.Cost)
                throw new Exception("Not enough balance");

            user.Balance -= item.Cost;

            await _testDbContext.UserItems.AddAsync(new UserItem
            {
                UserId = userId,
                ItemId = itemId,
            });

            await _testDbContext.SaveChangesAsync();

            await transaction.CommitAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception during purchase: {ex.Message}");
            await transaction.RollbackAsync();
        }
        finally
        {
            Semaphore.Release();
        }
    }
    
    public async Task<List<PopularItemReport>> GetPopularItemsReportAsync()
    {
        var data = await _testDbContext.UserItems
            .GroupBy(ui => new { ui.PurchaseDate.Year, ui.ItemId, ui.PurchaseDate.Date, ui.UserId })
            .Select(g => new
            {
                Year = g.Key.Year,
                ItemId = g.Key.ItemId,
                PurchaseDate = g.Key.Date,
                Count = g.Count()
            })
            .ToListAsync();

        var report = data
            .GroupBy(d => new { d.Year, d.ItemId })
            .Select(g => new
            {
                Year = g.Key.Year,
                ItemId = g.Key.ItemId,
                Popularity = g.Max(d => d.Count)
            })
            .OrderByDescending(x => x.Popularity)
            .GroupBy(x => x.Year)
            .SelectMany(g => g
                .OrderByDescending(x => x.Popularity)
                .Take(3)
                .Select(x => new PopularItemReport
                {
                    Year = x.Year,
                    ItemName = _testDbContext.Items
                        .Where(i => i.Id == x.ItemId)
                        .Select(i => i.Name)
                        .FirstOrDefault() ?? "[Unknown Item]",
                    PurchaseCount = x.Popularity
                }))
            .ToList();

        return report;
    }
}