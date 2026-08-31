# Plugin.Maui.MVVMExpress.Pagination

Paging and search debounce for **MVVMExpress** lists.

[![NuGet](https://img.shields.io/nuget/v/Plugin.Maui.MVVMExpress.Pagination.svg?label=NuGet)](https://www.nuget.org/packages/Plugin.Maui.MVVMExpress.Pagination)

```csharp
var page = new DelegatePagedCollection<Product>(
    (skip, take, ct) => catalog.ListAsync(skip, take, ct),
    pageSize: 20);

await page.RefreshAsync();
await page.LoadMoreAsync();
```

`PagedCollection<T>` uses `ObservableRangeCollection<T>` so each page is one collection reset. `SearchQuery` debounces text before `WhenReadyAsync`.

## Install

```bash
dotnet add package Plugin.Maui.MVVMExpress.Pagination --prerelease
```

Target framework: `net10.0`. Depends on [Core](https://www.nuget.org/packages/Plugin.Maui.MVVMExpress.Core). Version `0.1.0-preview`.

## Related

Product docs: [repository README](https://github.com/nuvyntralabs/Plugin.Maui.MVVMExpress). License: MIT. Niladri Padhy / MauiEssentials.
