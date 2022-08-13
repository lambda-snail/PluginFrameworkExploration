using Application.Sdk;

namespace SamplePlugins;

public partial class SquareInputPlugin : IJSOperation
{
    public string Name { get; set; } = "square";

    [Output(ReturnType.Int)]
    public int OutInt { get; set; }

    [Input(ReturnType.Int)]
    public int InputInt { get; set; }
    
    public void Execute()
    {
        OutInt = InputInt * InputInt;
    }
}