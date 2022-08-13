namespace Application.Sdk;

public interface IJSOperation
{
    public string Name { get; set; }
    void Execute();
}

[AttributeUsage(AttributeTargets.Property)]
public class OutputAttribute : Attribute
{
}

public class InputAttribute : Attribute
{
}
