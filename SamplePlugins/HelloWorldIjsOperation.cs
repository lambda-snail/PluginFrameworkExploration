using Application.Sdk;

namespace SamplePlugins;

public class HelloWorldIjsOperation : IJSOperation
{
    public string Name { get; set; } = "hello";
    public void Execute()
    {
        Console.WriteLine("Hello World!");
    }
}