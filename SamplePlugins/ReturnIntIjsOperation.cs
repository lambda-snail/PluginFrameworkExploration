using Application.Sdk;

namespace SamplePlugins;

public partial class ReturnIntIjsOperation : IJSOperation
{
    public string Name { get; set; } = "getInt";

    [Output(ReturnType.Int)]
    public int OutInt { get; set; }
    public void Execute()
    {
        OutInt = Random.Shared.Next();
    }
}