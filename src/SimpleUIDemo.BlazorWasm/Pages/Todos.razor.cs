using SimpleUIDemo.BlazorWasm.Models;
using Sysinfocus.AspNetCore.Components;

namespace SimpleUIDemo.BlazorWasm.Pages;

public partial class Todos(HttpClient client)
{
    private ICollection<Todo>? _data;
    private IEnumerable<Todo>? _pagedData;
    private string _search = string.Empty;
    private SortModel _sortModel = new() { Header = "ID", IsAscending = true };
    private PaginationState _paging = new() { CurrentPage = 1, TotalRecords = 0 };
    private bool _confirmDelete;
    private Todo? _actionItem;

    protected override async Task OnParametersSetAsync() =>
        (_data, _pagedData) = await client.LoadData<Todo>("https://jsonplaceholder.typicode.com/todos", _paging);

    private void HandleDelete(Todo dataItem)
    {
        _actionItem = dataItem;
        _confirmDelete = true;
    }

    private void HandleDeleteCancelled()
    {
        _confirmDelete = false;
        _actionItem = null;
    }

    private void HandleDeleteConfirmation()
    {
        _confirmDelete = false;
        if (_actionItem is null) return;
        _data?.Remove(_actionItem);
        _data.ResetPaging(_paging);
        HandlePaging();
    }

    private async Task HandleEdit(Todo dataItem) => await Task.Delay(10);

    private void HandleSorting(SortModel sortModel)
    {
        if (_data is null) return;
        _data = (sortModel.Header.ToLower(), sortModel.IsAscending) switch
        {
            ("id", true) => _data.OrderBy(a => a.Id).ToList(),
            ("id", false) => _data.OrderByDescending(a => a.Id).ToList(),
            ("user id", true) => _data.OrderBy(a => a.UserId).ToList(),
            ("user id", false) => _data.OrderByDescending(a => a.UserId).ToList(),
            ("status", true) => _data.OrderBy(a => a.Completed).ToList(),
            ("status", false) => _data.OrderByDescending(a => a.Completed).ToList(),
            (_, _) => _data
        };
        _paging.CurrentPage = 1;
        HandlePaging();
    }

    private void HandlePaging() => _pagedData = _data.PageData(_paging);

    private void HandleSearch()
    {
        _pagedData = _data.Search(_search, a =>
            a.Title.Contains(_search, StringComparison.OrdinalIgnoreCase) ||
            a.UserId.ToString().Contains(_search, StringComparison.OrdinalIgnoreCase) ||
            a.Completed.ToString().Contains(_search, StringComparison.OrdinalIgnoreCase) ||
            a.Id.ToString().Contains(_search, StringComparison.OrdinalIgnoreCase)
        );
        _pagedData.ResetPaging(_paging);
    }
}