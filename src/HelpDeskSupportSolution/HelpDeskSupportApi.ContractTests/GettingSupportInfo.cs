using Alba;
using HelpDeskSupportApi.Services;
using Microsoft.Extensions.DependencyInjection;

namespace HelpDeskSupportApi.ContractTests;
public class GettingSupportInfo
{
    [Fact]
    public async Task WhenWeAreOpen()
    {
        var host = await AlbaHost.For<Program>(options =>
        {
            options.ConfigureServices(services => services.AddScoped<IProvideTheBusinessClock, FakeOpenBusinessClock>());
        });

        var expectedResults = new SupportResponseModel("Bob Smith", "555-1212", "bob@company.com");

        var response = await host.Scenario(api =>
        {
            api.Get.Url("/"); // Do a get request to the api to this ("/") resource.
            api.StatusCodeShouldBeOk();
        });

        var actualResponse = await response.ReadAsJsonAsync<SupportResponseModel>();

        Assert.NotNull(actualResponse);
        Assert.Equal(expectedResults, actualResponse);
    }

    [Fact]
    public async Task WhenWeAreClosed()
    {
        var host = await AlbaHost.For<Program>(options =>
        {
            options.ConfigureServices(services => services.AddScoped<IProvideTheBusinessClock, FakeClosedBusinessClock>());
        });
        var expectedResults = new SupportResponseModel("Support Pros", "(800) BAD-CODE", "help@support-pros.com");

        var response = await host.Scenario(api =>
        {
            api.Get.Url("/"); // Do a get request to the api to this ("/") resource.
            api.StatusCodeShouldBeOk();
        });

        var actualResponse = await response.ReadAsJsonAsync<SupportResponseModel>();

        Assert.NotNull(actualResponse);
        Assert.Equal(expectedResults, actualResponse);
    }
}

public class FakeOpenBusinessClock : IProvideTheBusinessClock
{
    public Task<bool> AreWeCurrentlyOpenAsync()
    {
        return Task.FromResult(true);
    }
}
public class FakeClosedBusinessClock : IProvideTheBusinessClock
{
    public Task<bool> AreWeCurrentlyOpenAsync()
    {
        return Task.FromResult(false);
    }
}