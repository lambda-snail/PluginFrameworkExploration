using Application.Sdk;

namespace SamplePlugins;

public partial class ReturnStringOperation : IJSOperation
{
    public string Name { get; set; } = "returnString";

    [Output(ReturnType.String)]
    public string ReturnValue { get; set; }

    public void Execute()
    {
        ReturnValue = "Today's date: " + DateTime.Now.ToShortDateString();
    }
}
