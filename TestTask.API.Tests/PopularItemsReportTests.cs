using Microsoft.AspNetCore.Mvc;
using TestTask.API.Controllers;
using TestTask.Data.Entities;
using TestTask.Services;
using TestTask.Services.DTO;

namespace TestTask.API.Tests;

public class PopularItemsReportTests : BaseTest
{
    private const decimal DefaultBalance = 1000;
    private const decimal ItemCost = 100;
    private MarketService _marketService;

    protected override async Task SetupBase()
    {
        var user1 = new User { Email = "Email1@gmail.com", Balance = DefaultBalance };
        var user2 = new User { Email = "Email2@gmail.com", Balance = DefaultBalance };

        await Context.DbContext.Users.AddRangeAsync(user1, user2);

        var items = new[]
        {
            new Item { Name = "Item 1", Cost = ItemCost },
            new Item { Name = "Item 2", Cost = ItemCost },
            new Item { Name = "Item 3", Cost = ItemCost },
            new Item { Name = "Item 4", Cost = ItemCost }
        };

        await Context.DbContext.Items.AddRangeAsync(items);
        await Context.DbContext.SaveChangesAsync();

        var userItems = new[]
        {
            new UserItem { UserId = user1.Id, ItemId = items[0].Id, PurchaseDate = DateTime.SpecifyKind(new DateTime(2001, 1, 1), DateTimeKind.Utc) },
            new UserItem { UserId = user1.Id, ItemId = items[0].Id, PurchaseDate = DateTime.SpecifyKind(new DateTime(2001, 1, 1), DateTimeKind.Utc) },
            new UserItem { UserId = user1.Id, ItemId = items[1].Id, PurchaseDate = DateTime.SpecifyKind(new DateTime(2001, 2, 1), DateTimeKind.Utc) },
            new UserItem { UserId = user1.Id, ItemId = items[1].Id, PurchaseDate = DateTime.SpecifyKind(new DateTime(2001, 2, 1), DateTimeKind.Utc) },
            new UserItem { UserId = user1.Id, ItemId = items[1].Id, PurchaseDate = DateTime.SpecifyKind(new DateTime(2001, 2, 1), DateTimeKind.Utc) },
            
            new UserItem { UserId = user2.Id, ItemId = items[1].Id, PurchaseDate = DateTime.SpecifyKind(new DateTime(2001, 2, 1), DateTimeKind.Utc) },
            new UserItem { UserId = user2.Id, ItemId = items[2].Id, PurchaseDate = DateTime.SpecifyKind(new DateTime(2000, 12, 31), DateTimeKind.Utc) },
            new UserItem { UserId = user2.Id, ItemId = items[2].Id, PurchaseDate = DateTime.SpecifyKind(new DateTime(2000, 12, 31), DateTimeKind.Utc) },
            new UserItem { UserId = user2.Id, ItemId = items[3].Id, PurchaseDate = DateTime.SpecifyKind(new DateTime(2001, 3, 1), DateTimeKind.Utc) }
        };

        await Context.DbContext.UserItems.AddRangeAsync(userItems);
        await Context.DbContext.SaveChangesAsync();

        _marketService = new MarketService(Context.DbContext);
    }

    [Test]
    public async Task GetPopularItemsReport_ShouldReturnTotalRecords()
    {
        // Arrange
        var controller = new MarketController(_marketService);

        // Act
        var actionResult = await controller.GetPopularItemsReport();

        // Assert
        var okResult = actionResult as OkObjectResult;
        Assert.NotNull(okResult);

        var response = okResult.Value as List<PopularItemReport>;
        Assert.NotNull(response);
        Assert.That(response.Count, Is.EqualTo(4));
    }

    [Test]
    public async Task GetPopularItemsReport_ShouldReturnTop3For2001()
    {
        // Arrange
        var controller = new MarketController(_marketService);

        // Act
        var actionResult = await controller.GetPopularItemsReport();

        // Assert
        var okResult = actionResult as OkObjectResult;
        Assert.NotNull(okResult);

        var response = okResult.Value as List<PopularItemReport>;
        Assert.NotNull(response);

        var items2001 = response.Where(r => r.Year == 2001).ToList();
        Assert.That(items2001.Count, Is.EqualTo(3));

        var itemB = items2001.FirstOrDefault(r => r.ItemName == "Item 2");
        Assert.NotNull(itemB);
        Assert.That(itemB.PurchaseCount, Is.EqualTo(3));

        var itemA = items2001.FirstOrDefault(r => r.ItemName == "Item 1");
        Assert.NotNull(itemA);
        Assert.That(itemA.PurchaseCount, Is.EqualTo(2));

        var itemD = items2001.FirstOrDefault(r => r.ItemName == "Item 4");
        Assert.NotNull(itemD);
        Assert.That(itemD.PurchaseCount, Is.EqualTo(1));
    }

    [Test]
    public async Task GetPopularItemsReport_ShouldReturnDataFor2000()
    {
        // Arrange
        var controller = new MarketController(_marketService);

        // Act
        var actionResult = await controller.GetPopularItemsReport();

        // Assert
        var okResult = actionResult as OkObjectResult;
        Assert.NotNull(okResult);

        var response = okResult.Value as List<PopularItemReport>;
        Assert.NotNull(response);

        var items2000 = response.Where(r => r.Year == 2000).ToList();
        Assert.That(items2000.Count, Is.EqualTo(1));

        var itemC = items2000.FirstOrDefault(r => r.ItemName == "Item 3");
        Assert.NotNull(itemC);
        Assert.That(itemC.PurchaseCount, Is.EqualTo(2));
    }
}
