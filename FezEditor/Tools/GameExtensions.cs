using Microsoft.Xna.Framework;

namespace FezEditor.Tools;

public static class GameExtensions
{
    private static readonly Lock Lock = new();

    private static readonly List<object> Services = new();

    public static void AddService<T>(this Game game, T service)
    {
        if (service is not IUpdateable &&
            service is not IDrawable &&
            service is not IGameComponent &&
            service is not IComparable)
        {
            game.Services.AddService(typeof(T), service);
            Services.Add(service!);
        }
    }

    public static T GetService<T>(this Game game) where T : class
    {
        var @object = game.Services.GetService(typeof(T));
        if (@object is not T service)
        {
            throw new InvalidCastException("Could not cast service " + typeof(T).FullName);
        }
        
        return service;
    }

    public static void RemoveService<T>(this Game game)
    {
        var @object = game.Services.GetService(typeof(T));
        if (@object is T service)
        {
            game.Services.RemoveService(typeof(T));
            if (service is IDisposable disposable)
            {
                disposable.Dispose();
            }
            Services.Remove(service);
        }
    }

    public static void RemoveServices(this Game game)
    {
        foreach (var service in Services)
        {
            game.Services.RemoveService(service.GetType());
            if (service is IDisposable disposable)
            {
                disposable.Dispose();
            }
        }
        Services.Clear();
    }

    public static void AddComponent<T>(this Game game, T component) where T : IGameComponent
    {
        lock (Lock)
        {
            game.Components.Add(component);
        }
    }

    public static T GetComponent<T>(this Game game) where T : IGameComponent
    {
        lock (Lock)
        {
            return game.Components.OfType<T>().First();
        }
    }
    
    public static void RemoveComponent<T>(this Game game, T component) where T : IGameComponent
    {
        if (component is IDisposable disposable)
        {
            disposable.Dispose();
        }

        lock (Lock)
        {
            game.Components.Remove(component);
        }
    }
}