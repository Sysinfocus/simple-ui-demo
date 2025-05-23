using SimpleUIDemo.BlazorWasm.Models;
using Sysinfocus.AspNetCore.Components;
using System.Text.Json;

namespace SimpleUIDemo.BlazorWasm.Pages;
public partial class Dashboard
{
    private List<MenuItemOption> menu = [];
    private bool showMenu;
    private string? barChart, pieChart;
    private List<DashboardItem> dbItems = [];
    protected override void OnInitialized()
    {
        SetupBarChart();
        SetupPieChart();

        SetupDashboardItems();
        SetupMenuItems();
    }

    private void SetupDashboardItems()
    {
        dbItems = [
            new("HDFC Bank", 1930.7, 16.87),
            new("Bharat Elec", 373.5, 12.36),
            new("Bajaj Finance", 9190, 12.02),
            new("Kotak Mahindra", 2071.4, -3.24),
            new("Adani Ports", 1377.5, -2.67),
            new("Sun Pharma", 1739, 0)
        ];
    }

    private void SetupMenuItems()
    {
        foreach (var item in dbItems)
            menu.Add(new(item.Title) { MenuType = MenuType.Checkbox, Checked = item.Show });

        menu.Add(new(string.Empty));
        menu.Add(new("Reset All") { MenuType = MenuType.Icon, Icon = "refresh" });
    }

    private void SetupBarChart()
    {
        var barChartData = new
        {
            tooltip = new { },
            legend = new { data = new[] { "Stock analysis for last 6 days" } },
            xAxis = new
            {
                data = new[] { DateTime.Now.AddDays(-6).ToString("d/M"), DateTime.Now.AddDays(-5).ToString("d/M"), DateTime.Now.AddDays(-4).ToString("d/M"), DateTime.Now.AddDays(-3).ToString("d/M"), DateTime.Now.AddDays(-2).ToString("d/M"), DateTime.Now.AddDays(-1).ToString("d/M") }
            },
            yAxis = new { },
            series = new
            {
                name = "Stock price",
                type = "bar",
                data = new[] { 5, 20, 36, 10, 10, 20 },
                showBackground = false,
                backgroundStyle = new { color = "rgba(220, 220, 220, 0.8)" }
            }
        };
        barChart = JsonSerializer.Serialize(barChartData);
    }

    private void SetupPieChart()
    {
        var pieChartData = new
        {
            tooltip = new { trigger = "item", formatter = "{a} <br/>{b} : {c} ({d}%)" },
            series = new
            {
                name = "Sales",
                type = "pie",
                data = new[] {
                new { value = 5, name = "Jan" },
                new { value = 20, name = "Feb" },
                new { value = 36, name = "Mar" },
                new { value = 10, name = "Apr" },
                new { value = 10, name = "May" },
                new { value = 20, name = "Jun" }
            },
                emphasis = new
                {
                    itemStyle = new
                    {
                        shadowBlur = 10,
                        shadowColor = "rgba(0, 0, 0, 0.75)"
                    }
                }
            }
        };
        pieChart = JsonSerializer.Serialize(pieChartData);
    }

    private void ResetMenu()
    {
        dbItems.ForEach(x => x.Show = true);
        menu.ForEach(x => x.Checked = true);
        showMenu = false;
    }

    private void UpdateMenu(MenuItemOption mio)
    {
        if (mio.Text == "Reset All")
        {
            ResetMenu();
            return;
        }

        var update = dbItems.FirstOrDefault(x => x.Title == mio.Text);
        if (update is null) return;
        update.Show = mio.Checked;
    }

    private void HandleSortChange((int oIndex, int nIndex, string from, string to) index)
    {
        if (dbItems is null || index.from != "dashboard") return;
        var itm = dbItems[index.oIndex];
        dbItems.Remove(itm);
        dbItems.Insert(index.nIndex, itm);
    }
}