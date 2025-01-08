using NshmCalculator.MudClient.Utilities.Interface;

namespace NshmCalculator.MudClient.Utilities;

public class StateContainer : IStateContainer
{
    public event Action<string>? NotifyPublisher;
    public event Action<string>? NotifyUpdate;

    public void FindPublisher(string code)
    {
        NotifyPublisher?.Invoke(code);
    }

    public void NotifyValueUpdated(string code)
    {
        NotifyUpdate?.Invoke(code);
    }
}