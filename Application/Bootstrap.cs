using Application;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

public partial class Bootstrap
{
    public static async Task Main(string[] args)
    {
        using IHost host = Host.CreateDefaultBuilder(args)
            .ConfigureServices((_, services) =>
                Bootstrap.ConfigureServices(services))
            .Build();
        
        Program p = host.Services.GetRequiredService<Program>();
        
        await p.RunAsync();
        
        //await host.RunAsync();
    }
    
    public static void ConfigureServices(IServiceCollection services)
    {
        services.AddSingleton<Program>();
    }
}