namespace TestTask.Services.DTO;

public class PopularItemReport
{
    public int Year { get; set; }
    public string ItemName { get; set; } = string.Empty;
    public int PurchaseCount { get; set; }
}