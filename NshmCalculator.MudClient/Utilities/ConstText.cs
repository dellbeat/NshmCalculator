namespace NshmCalculator.MudClient.Utilities;

public static class ConstText
{
    public const string ExceptionText = "<ul><li>计算进程出现异常，建议检查输入内容</li><li>如有必要可与作者联系</li></ul>";

    public const string WarningText = "<ul><li>部分需填写的数据不符合要求</li><li>请根据上方红色提示重新填写</li></ul>";

    public const string SuccessText = "<ul><li>计算完成</li></ul>";

    public const string LimitText = "<ul><li>计算完成</li><li>本次破防/命中出现超限情况</li><li>已忽略超限部分的提升</li></ul>";

    public const string SelectionExceptionText = "<ul><li>首领数据加载有误，请刷新后再试</li><li>如有必要可与作者联系</li></ul>";

    public const string ConfigNotFoundText = "<ul><li>获取计算器配置时出现异常，请检查网络</li></ul>";
    
    public const string FooterText = "Version:2.1.3 | Powered by .NET 8.0";

    public const string LastVisitName = "lastVisit";//最近一次访问的计算器路由

    public const string AutoRedirectName = "autoRedirect";//是否重定向

    public const string DenseModeName = "denseMode";
    
    public const string VersionPath = "../data/version.json";

    public const string NotExistText = "<ul><li>无对应配置项，请联系作者</li></ul>";
}