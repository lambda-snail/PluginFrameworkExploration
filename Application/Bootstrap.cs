using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Application;

public static partial class Bootstrap
{
    public static async Task Main(string[] args)
    {
        using IHost host = Host.CreateDefaultBuilder(args)
            .ConfigureServices((_, services) =>
                Bootstrap.ConfigureServices(services))
            .Build();
        
        HelloFrom("Main");
        
        Program p = host.Services.GetRequiredService<Program>();

        try
        {
            Console.WriteLine("Which script would you like to execute?");
            string scriptName = Console.ReadLine();

            string scriptContent = await LoadScriptIntoMemory(scriptName);
            await p.RunAsync(scriptContent);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }

    static partial void HelloFrom(string name);

    private static async Task<string> LoadScriptIntoMemory(string? scriptName)
    {
        string scriptPath = Path.Combine(Directory.GetCurrentDirectory(), "scripts", scriptName);
        return await System.IO.File.ReadAllTextAsync(scriptPath);
    }

    public static void ConfigureServices(IServiceCollection services)
    {
        services.AddSingleton<Program>();
    }
}