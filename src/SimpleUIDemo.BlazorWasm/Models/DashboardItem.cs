namespace SimpleUIDemo.BlazorWasm.Models;
public record DashboardItem(string Title, double Price, double Change)
{
    public bool Show { get; set; } = true;
};