namespace Plugin.Maui.MVVMExpress.Behaviors;

/// <summary>
/// First-party list guidance: bind <c>ItemsSource</c> to SnapshotCollection / PagedCollection,
/// <c>IsRefreshing</c> to AsyncState, and enable remaining-items only when the fetch is async.
/// </summary>
public static class CollectionBind
{
    /// <summary>
    /// When <see langword="true"/>, remaining-items threshold is allowed (async fetch).
    /// When <see langword="false"/>, pair the list with <c>SnapshotCollection</c> instead of a sync paged fetch.
    /// </summary>
    public static readonly BindableProperty AsyncFetchProperty = BindableProperty.CreateAttached(
        "AsyncFetch",
        typeof(bool),
        typeof(CollectionBind),
        defaultValue: false);

    /// <summary>Gets the async-fetch flag.</summary>
    public static bool GetAsyncFetch(BindableObject view) => (bool)view.GetValue(AsyncFetchProperty);

    /// <summary>Sets the async-fetch flag.</summary>
    public static void SetAsyncFetch(BindableObject view, bool value) => view.SetValue(AsyncFetchProperty, value);
}
