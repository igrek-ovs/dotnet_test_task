using System.ComponentModel.DataAnnotations;

namespace TestTask.Data.Entities;

public class Item
{
    public int Id { get; set; }
    [MaxLength(100)] public required string Name { get; set; }
    public required decimal Cost { get; set; }
}