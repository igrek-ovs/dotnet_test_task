using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using RAIT.Core;
using TestTask.Data;
using ServiceCollectionExtensions = RAIT.Core.ServiceCollectionExtensions;

namespace TestTask.API.Tests;

public class BaseTest
{
    protected RaitHttpClientWrapper<T> Rait<T>() where T : ControllerBase
    {
        return Context.Client.Rait<T>();
    }

    [SetUp]
    public async Task Setup()
    {
        var application = new WebApplicationFactory<Program>().WithWebHostBuilder(PrepareEnv);
        application.Services.ConfigureRait();
        Context.Client = application.CreateDefaultClient();
        Context.Services = application.Services;
        Context.DbContext = application.Services.GetRequiredService<TestDbContext>();

        await SetupBase();
    }

    protected virtual async Task SetupBase()
    {
    }

    private static void PrepareEnv(IWebHostBuilder _)
    {
        _.UseEnvironment("test");
        _.ConfigureTestServices(s => ServiceCollectionExtensions.AddRait(s));
    }
}