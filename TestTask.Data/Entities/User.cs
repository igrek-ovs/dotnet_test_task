using System.ComponentModel.DataAnnotations;

namespace TestTask.Data.Entities;

public class User
{
    public int Id { get; set; }
    [MaxLength(100)] public required string Email { get; set; }
    public decimal Balance { get; set; }
}