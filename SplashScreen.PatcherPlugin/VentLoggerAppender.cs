using VentLib.Logging;
using VentLib.Logging.Appenders;

namespace Lotus.SplashScreen;

// ventframework's logging system has an issue where it just... doesnt show messages in stdout.
// vf has same issue on starlight, pretty sure this fixes it for this at least
public sealed class VentLoggerAppender : ILogAppender 
{
    public static void Register() => GlobalLogAppenders.AddAppender(new VentLoggerAppender());
    public void Receive(LogComposite composite, LogArguments arguments) => SplashManager.SendMessage(composite.Message);
}