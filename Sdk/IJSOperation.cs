namespace Application.Sdk;

public interface IJSOperation
{
    public string Name { get; set; }
    void Execute();
}

[AttributeUsage(AttributeTargets.Property)]
public class OutputAttribute : Attribute
{
    public ReturnType Type { get; set; }
    public OutputAttribute(ReturnType type)
    {
        Type = type;
    }
}

public class InputAttribute : Attribute
{
    public ReturnType Type { get; set; }
    public InputAttribute(ReturnType type)
    {
        Type = type;
    }
}

public enum ReturnType
{
    Bool,
    Double,
    Int,
    String
}