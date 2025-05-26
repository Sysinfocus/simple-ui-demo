namespace SimpleUIDemo.BlazorWasm.Models;

public abstract class ModelValidator
{
    public abstract Dictionary<string, string>? Errors();
    public bool IsValid => Errors() is null;
}