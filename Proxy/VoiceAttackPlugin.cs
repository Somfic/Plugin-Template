using VoiceAttack.Proxy.Abstractions;
using VoiceAttack.Proxy.Logging;

namespace VoiceAttack.Proxy;

public abstract class VoiceAttackPlugin
{
    public static VoiceAttackPlugin? Instance;
    public static VoiceAttackProxy? Proxy;

    protected VoiceAttackPlugin()
    {
        Instance ??= this;
    }

    public abstract void OnInitialise(IVoiceAttackProxy proxy);

    public abstract void OnInvoke(IVoiceAttackProxy proxy, string context);

    public abstract void OnCommandStopped(IVoiceAttackProxy proxy);

    public abstract void OnExit(IVoiceAttackProxy proxy);

    public void Log(VoiceAttackColor color, string message, Exception? exception = null)
    {
        Proxy?.Log.Write(message, color);
        
        if (exception != null)
            Proxy?.Log.Write(exception.ToString(), color);
    } 
}