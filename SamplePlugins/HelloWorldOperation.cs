using Application.Sdk;

namespace SamplePlugins;

public partial class HelloWorldOperation : IJSOperation
{
    public string Name { get; set; } = "hello";
    public void Execute()
    {
        Console.WriteLine("Hello World!");
    }
}