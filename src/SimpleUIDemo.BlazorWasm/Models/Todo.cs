namespace SimpleUIDemo.BlazorWasm.Models;

public class Todo : ModelValidator
{
    public int UserId { get; set; }
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public bool Completed { get; set; }

    public override Dictionary<string, string>? Errors()
    {
        var errors = new Dictionary<string, string>();
        
        if (Title?.Length < 5 || Title?.Length > 50)
            errors.TryAdd(nameof(Title), $"{nameof(Title)} should be between 5 and 50 chars only.");

        return errors.Count > 0 ? errors : null;
    }
}
