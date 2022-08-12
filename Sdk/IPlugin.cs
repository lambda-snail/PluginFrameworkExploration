namespace Application.Sdk;

public interface IPlugin
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

public enum ReturnType
{
    Bool,
    Double,
    Int,
    String
}