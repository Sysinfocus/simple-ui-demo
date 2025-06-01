using Sysinfocus.AspNetCore.Components;
using System.Net.Http.Json;
using System.Text.Json;

namespace SimpleUIDemo.BlazorWasm;

public static class Extensions
{
    private static readonly JsonSerializerOptions _serializerOptions = new() { PropertyNameCaseInsensitive = true };
    public static async Task<(ICollection<T>?, IEnumerable<T>?)> LoadData<T>(this HttpClient client, string url, PaginationState paging, string? existingData = null)
    {
        ICollection<T>? data = existingData is not null
            ? JsonSerializer.Deserialize<ICollection<T>>(existingData, _serializerOptions)
            : await client.GetFromJsonAsync<ICollection<T>>(url, _serializerOptions);

        if (data is not null)
        {
            var pagedData = data.UpdatePaging(paging);
            return (data, pagedData);
        }
        return (data, null);
    }

    public static IEnumerable<T> UpdatePaging<T>(this ICollection<T> _data, PaginationState paging)
    {
        paging.TotalRecords = _data.Count;
        return _data.Take(paging.PageSize);
    }

    public static T If<T>(this T oldValue, bool condition, T newValue)
        => condition ? newValue : oldValue;

    public static IEnumerable<T>? PageData<T>(this IEnumerable<T>? source, PaginationState paging)
    {
        var skip = (paging.CurrentPage - 1) * paging.PageSize;
        var balance = paging.TotalRecords - skip;
        balance = balance.If(balance > paging.PageSize, paging.PageSize);
        return source?.Skip(skip).Take(balance);
    }

    public static string PagingDetails(this PaginationState paging, string? search = null)
    {
        var count = paging.CurrentPage * paging.PageSize;
        count = count.If(count > paging.TotalRecords, paging.TotalRecords);

        var searchType = string.IsNullOrWhiteSpace(search) ? "" :
            search[0] == '*' ? "ending with" :
            search[0] == '=' ? "equal to" :
            search[^1]== '*' ? "starting with" :
            "containing";

        var searchTerm = searchType switch
        {
            "ending with" or "equal to" => search?[1..],
            "starting with" => search?[0..^1],
            _ => search,
        };

        var searchText = search.If(!string.IsNullOrEmpty(search), $" for search {searchType} '{searchTerm}'");
        if (paging.TotalRecords == 0) return $"No records{searchText}.";
        return $"Showing {count} / {paging.TotalRecords} records{searchText}.";
    }

    public static void ResetPaging<T>(this IEnumerable<T>? source, PaginationState paging)
    {
        if (source is null) return;
        paging.TotalRecords = source.Count();
        paging.CurrentPage = 1;
    }

    public static IEnumerable<T>? Search<T>(this IEnumerable<T>? source, string? search, Func<T, bool> conditions)
        => string.IsNullOrEmpty(search) ? [.. source!] : [.. source!.Where(conditions)];

    public static bool Match(this string? source, string? find)
    {
        if (string.IsNullOrWhiteSpace(find) || string.IsNullOrWhiteSpace(source)) return true;

        return find[0] == '*' ? source.EndsWith(find[1..], StringComparison.OrdinalIgnoreCase)
            : find[0] == '=' ? source.Equals(find[1..], StringComparison.OrdinalIgnoreCase)
            : find[^1] == '*' ? source.StartsWith(find[0..^1], StringComparison.OrdinalIgnoreCase)
            : source.Contains(find, StringComparison.OrdinalIgnoreCase);
    }
}
