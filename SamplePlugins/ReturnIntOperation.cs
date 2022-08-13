using Application.Sdk;

namespace SamplePlugins;

public partial class ReturnIntOperation : IJSOperation
{
    public string Name { get; set; } = "getInt";

    [Output]
    public int OutInt { get; set; }
    public void Execute()
    {
        OutInt = Random.Shared.Next();
    }
}