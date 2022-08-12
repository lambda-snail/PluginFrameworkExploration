using Application.Sdk;

namespace SamplePlugins;

public class ReturnIntPlugin : IPlugin
{
    public string Name { get; set; } = "getInt";

    [Output(ReturnType.Int)]
    public int OutInt { get; set; }
    public void Execute()
    {
        OutInt = Random.Shared.Next();
    }
}