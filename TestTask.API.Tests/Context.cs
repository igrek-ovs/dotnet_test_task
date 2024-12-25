using TestTask.Data;

namespace TestTask.API.Tests;

public static class Context
{
    public static HttpClient Client { get; set; } = null!;
    public static IServiceProvider Services { get; set; } = null!;
    public static TestDbContext DbContext { get; set; } = null!;
}