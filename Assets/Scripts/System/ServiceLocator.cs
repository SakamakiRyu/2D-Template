/// <summary>
/// サービスロケーター
/// </summary>
public static class ServiceLocator<T> where T : class
{
    public static T Instance { get; private set; }

    public static bool IsValid => Instance != null;

    public static void Bind(T instance)
    {
        Instance = instance;
    }

    public static void UnBind(T instance)
    {
        if (Instance == instance)
        {
            Instance = null;
        }
    }

    public static void Clear()
    {
        Instance = null;
    }
}