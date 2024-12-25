using Microsoft.EntityFrameworkCore;
using TestTask.API.Controllers;
using TestTask.Data.Entities;

namespace TestTask.API.Tests;

public class PurchaseTests : BaseTest
{
    private const int ItemCost = 100;
    private const int MaxPurchasableItemCount = 2;
    private const int InitialUserBalance = ItemCost * MaxPurchasableItemCount;

    protected override async Task SetupBase()
    {
        await Context.DbContext.Users.AddAsync(new User
        {
            Email = "Email@gmail.com",
            Balance = InitialUserBalance
        });
        await Context.DbContext.Items.AddAsync(new Item
        {
            Name = "Item 1",
            Cost = ItemCost
        });

        await Context.DbContext.SaveChangesAsync();
    }

    [Test]
    public async Task BuyAsync_ShouldPurchaseItemAndUpdateBalance()
    {
        // Arrange
        var user = await Context.DbContext.Users.FirstAsync();
        var item = await Context.DbContext.Items.FirstAsync();

        // Act
        await Rait<MarketController>().Call(controller => controller.BuyAsync(user.Id, item.Id));

        // Assert
        var totalUserItems = await Context.DbContext.UserItems.CountAsync();
        Assert.That(totalUserItems, Is.EqualTo(1));
    }


    [Test]
    public async Task BuyAsync_ShouldHandleConcurrentPurchases()
    {
        // Arrange
        var user = await Context.DbContext.Users.FirstAsync();
        var item = await Context.DbContext.Items.FirstAsync();
        var initialUserItemCount = await Context.DbContext.UserItems.CountAsync();

        // Act
        var tasks = Enumerable.Range(0, 5).Select(_ =>
            Rait<MarketController>().Call(controller => controller.BuyAsync(user.Id, item.Id))
        );

        await Task.WhenAll(tasks);

        // Assert
        var finalUserItemCount = await Context.DbContext.UserItems.CountAsync();
        Assert.That(finalUserItemCount, Is.EqualTo(initialUserItemCount + MaxPurchasableItemCount));
    }
}