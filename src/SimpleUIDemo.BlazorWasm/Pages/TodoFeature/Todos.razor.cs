using SimpleUIDemo.BlazorWasm.Models;
using Sysinfocus.AspNetCore.Components;
using System.Text.Json;

namespace SimpleUIDemo.BlazorWasm.Pages.TodoFeature;

public partial class Todos(HttpClient client)
{
    private ICollection<Todo>? _data;
    private IEnumerable<Todo>? _pagedData;
    private string _search = string.Empty;
    private readonly SortModel _sortModel = new() { Header = "ID", IsAscending = true };
    private readonly PaginationState _paging = new() { CurrentPage = 1, TotalRecords = 0 };
    private bool _isAdding, _isEditing, _confirmDelete;
    private Todo? _actionItem;

    private bool _showAlert;
    private string? _alertMessage;
    private AlertType _alertType = AlertType.Success;

    protected override async Task OnInitializedAsync()
    {
        var persistedData = await browserExtensions.GetFromLocalStorage("todos");
        (_data, _pagedData) = await client.LoadData<Todo>("https://jsonplaceholder.typicode.com/todos", _paging, persistedData);
    }

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

    private async Task HandleDeleteConfirmation()
    {
        _confirmDelete = false;
        if (_actionItem is null) return;
        _data?.Remove(_actionItem);
        _data.ResetPaging(_paging);
        HandlePaging();
        await SaveChanges();
        _alertMessage = "Item deleted successfully.";
        _alertType = AlertType.Success;
        _showAlert = true;
    }

    private void HandleEdit(Todo dataItem)
    {
        _actionItem = new Todo { Id = dataItem.Id, UserId = dataItem.UserId, Title = dataItem.Title, Completed = dataItem.Completed };
        _isEditing = true;
    }

    private void HandleSorting(SortModel sortModel)
    {
        if (_data is null) return;
        _data = (sortModel.Header.ToLower(), sortModel.IsAscending) switch
        {
            ("id", true) => [.. _data.OrderBy(a => a.Id)],
            ("id", false) => [.. _data.OrderByDescending(a => a.Id)],
            ("user id", true) => [.. _data.OrderBy(a => a.UserId)],
            ("user id", false) => [.. _data.OrderByDescending(a => a.UserId)],
            ("status", true) => [.. _data.OrderBy(a => a.Completed)],
            ("status", false) => [.. _data.OrderByDescending(a => a.Completed)],
            (_, _) => _data
        };
        _paging.CurrentPage = 1;
        HandlePaging();
    }

    private void HandlePaging()
        => _pagedData = SearchResults().PageData(_paging);

    private IEnumerable<Todo>? SearchResults()
        => _data.Search(_search, a =>
            a.Title.Match(_search) ||
            a.UserId.ToString().Match(_search) ||
            a.Completed.ToString().Match(_search) ||
            a.Id.ToString().Match(_search));
   
    private void HandleSearch()
    {
        _pagedData = SearchResults();
        _pagedData.ResetPaging(_paging);
        HandlePaging();
    }

    private void HandleAddNew(Todo todo)
    {
        todo.UserId = 1;
        todo.Id = (_data?.Max(x => x.Id) ?? 0) + 1;
        _data?.Add(todo);
    }

    private void HandleUpdate(Todo todo)
    {
        if (_actionItem?.Id != todo.Id) return;
        var updateItem = _data?.FirstOrDefault(x => x.Id ==  todo.Id);
        if (updateItem is null) return;
        updateItem.Title = todo.Title;
        updateItem.Completed = todo.Completed;
    }

    private async Task HandleCloseAddition(bool refreshPage)
    {
        if (_isAdding) _alertMessage = "Item added successfully.";
        else if (_isEditing) _alertMessage = "Item updated successfully.";
        _alertType = AlertType.Success;
        _isAdding = _isEditing = false;
        if (!refreshPage) return;
        await SaveChanges();
        _pagedData = _data?.UpdatePaging(_paging);
        _data.ResetPaging(_paging);
        _showAlert = true;
    }

    private async Task SaveChanges()
    {
        var data = JsonSerializer.Serialize(_data);
        await browserExtensions.SetToLocalStorage("todos", data);
    }
}