namespace MauiApp1;

public sealed class GreetingService : IGreetingService
{
    public string Greet(string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        return $"Hello, {name}";
    }
}
