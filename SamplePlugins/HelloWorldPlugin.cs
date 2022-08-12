

using Application.Sdk;

namespace SamplePlugins;

public class HelloWorldPlugin : IPlugin
{
    public string Name { get; set; } = "hello";
    
    public string Return { get; set; }
    
    public void Execute()
    {
        Console.WriteLine("Hello World!");
    }
}