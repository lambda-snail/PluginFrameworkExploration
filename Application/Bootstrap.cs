using System.Text;
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

    private static async Task<string> LoadScriptIntoMemory(string? scriptName)
    {
        string scriptPath = Path.Combine(Directory.GetCurrentDirectory(), "scripts", scriptName);
        return await System.IO.File.ReadAllTextAsync(scriptPath);
        // StringBuilder builder = new StringBuilder();
        // await using var fs = File.Open(scriptPath, FileMode.Open);
        //
        // byte[] b = new byte[1024];
        // UTF8Encoding temp = new UTF8Encoding(true);
        //
        // while (fs.Read(b, 0, b.Length) > 0)
        // {
        //     string s = temp.GetString(b).Trim();
        //     builder.Append(s);
        // }

        //return builder.ToString();
    }

    public static void ConfigureServices(IServiceCollection services)
    {
        services.AddSingleton<Program>();
    }
}