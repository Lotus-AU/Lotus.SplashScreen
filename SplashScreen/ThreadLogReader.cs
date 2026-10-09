using System.Collections.Concurrent;
using System.Text;

namespace Lotus.SplashScreen;

public static class ThreadLogReader
{
    private static readonly ConcurrentQueue<string> Messages = new();
    private static bool closed;

    public static bool Closed => closed;

    public static void Start() => new Thread(ReadInput){ IsBackground = true }.Start();

    private static void ReadInput()
    {
        try
        {
            using var reader = new StreamReader(Console.OpenStandardInput(), Encoding.UTF8);
            string? message;
            while ((message = reader.ReadLine()) != null) Messages.Enqueue(message);
        }
        catch (Exception exception)
        {
            Console.Error.WriteLine($"Splash input failed: {exception.Message}");
        }
        finally
        {
            closed = true;
        }
    }

    public static void Update()
    {
        while (Messages.TryDequeue(out string? message)) StateManager.ProcessLogMessage(message);
    }
}