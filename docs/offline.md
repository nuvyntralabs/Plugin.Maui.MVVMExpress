# Cache policies and capability abstractions

Phase 3. These are adapters, not a database.

```csharp
var result = await fetcher.FetchAsync(
    "products",
    catalog.ListAsync,
    connectivity.IsOnline ? FetchPolicy.NetworkFirst : FetchPolicy.CacheFirst,
    cancellationToken);

ServedFromCache = result.FromCache;
```

| Policy | Behavior |
| --- | --- |
| `CacheFirst` | Return cache; fetch on miss |
| `NetworkFirst` | Fetch; fall back to cache on failure / offline |
| `StaleWhileRevalidate` | Return cache, refresh in the background |
| `NetworkOnly` / `CacheOnly` | Skip the other source |

Production: [Plugin.Maui.ApiCache](https://www.nuget.org/packages/Plugin.Maui.ApiCache) / [Plugin.Maui.OfflineSync](https://www.nuget.org/packages/Plugin.Maui.OfflineSync). Flags: `IFeatureSwitch` → [Plugin.Maui.FeatureFlags](https://www.nuget.org/packages/Plugin.Maui.FeatureFlags). Permissions: `IPermissionGate` → [Plugin.Maui.PermissionFlow](https://www.nuget.org/packages/Plugin.Maui.PermissionFlow). Those are Niladri Padhy / MauiEssentials packages. Usual alternatives: raw `HttpClient`, MAUI `Permissions`, LaunchDarkly.
