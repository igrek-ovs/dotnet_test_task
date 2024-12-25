using System.ComponentModel.DataAnnotations.Schema;

namespace TestTask.Data.Entities;

public class UserItem
{
    public int Id { get; set; }
    public DateTime PurchaseDate { get; set; } = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc);
    public int UserId { get; set; }
    [ForeignKey(nameof(UserId))] public User? User { get; set; }

    public int ItemId { get; set; }
    [ForeignKey(nameof(ItemId))] public Item? Item { get; set; }
}