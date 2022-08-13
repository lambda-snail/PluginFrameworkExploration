using Application.Sdk;

namespace SamplePlugins;

public class HelloWorldOperation : IJSOperation
{
    public string Name { get; set; } = "hello";
    public void Execute()
    {
        Console.WriteLine("Hello World!");
    }
}