namespace NshmCalculator.MudClient.Utilities;

public static class ConstText
{
    public const string ExceptionText = "<ul><li>计算进程出现异常，建议检查输入内容</li><li>如有必要可与作者联系</li></ul>";

    public const string WarningText = "<ul><li>部分需填写的数据不符合要求</li><li>请根据上方红色提示重新填写</li></ul>";

    public const string SuccessText = "<ul><li>计算完成</li></ul>";

    public const string FooterText = "Version:0.7.0 | Powered by .NET 8.0";

    public const string LastVisitName = "lastVisit";//最近一次访问的计算器路由

    public const string AutoRedirectName = "autoRedirect";//是否重定向

    public const string UpdateLogPath = "../data/updatelog.json";//更新日志相对路径

    public const string TipsJsonPath = "../data/tipsContent.json";//帮助内容相对路径
}