namespace NshmCalculator.MudClient.Utilities.Interface;

public interface IStateContainer
{
    /// <summary>
    /// 通知发布者事件
    /// </summary>
    event Action<string>? NotifyPublisher;

    /// <summary>
    /// 发布者通知订阅者事件
    /// </summary>
    event Action<string>? NotifyUpdate;

    /// <summary>
    /// 通知发布者方法
    /// </summary>
    /// <param name="code">需要通知的参数代号</param>
    void FindPublisher(string code);
    
    /// <summary>
    /// 通知订阅者方法
    /// </summary>
    /// <param name="code">需要通知的参数代号</param>
    void NotifyValueUpdated(string code);
}