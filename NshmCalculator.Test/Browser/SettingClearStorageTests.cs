using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;

namespace NshmCalculator.Test.Browser;

/// <summary>
/// 设置页「清空本地存储」端到端测试（Playwright + 无头 Chromium）。
///
/// 被测流程：设置页按钮 → SelectCalculatorDialog 多选 → 确定后 LocalStorage.RemoveItem(code)。
/// 该流程依赖 ISyncLocalStorageService（JS 互操作）与真实 DOM 弹窗，bUnit 无法覆盖，
/// 故用 Playwright 在真实浏览器 localStorage 下验证持久化效果。
///
/// 运行前置：
///   1. pwsh bin/Debug/net8.0/playwright.ps1 install chromium
///   2. dotnet run --project NshmCalculator.MudClient（dev server 监听 5218）
///   3. dotnet test NshmCalculator.Test --filter "FullyQualifiedName~SettingClearStorageTests"
/// </summary>
[Parallelizable(ParallelScope.None)]
public class SettingClearStorageTests : PageTest
{
    /// <summary>
    /// 被测 App 根地址。默认取 launchSettings.json 中 http profile 的端口 5218；
    /// 可通过环境变量 BASE_URL 覆盖（CI 或端口被占用时）。
    /// </summary>
    private string BaseUrl => Environment.GetEnvironmentVariable("BASE_URL") ?? "http://localhost:5218";

    /// <summary>
    /// KiUpdate 页面在 OnInitializedAsync→InitParameters→SaveInfo 中写入的两个 localStorage 键。
    /// 与 Setting.razor 的 _dic 中「升级内功词条计算器」→ ["config_KiUpdate", "_kiUpdatePageModel"] 完全对应。
    /// </summary>
    private const string PageModelKey = "_kiUpdatePageModel";
    private const string ConfigCacheKey = "config_KiUpdate";

    [TearDown]
    public async Task ScreenshotOnFailure()
    {
        if (TestContext.CurrentContext.Result.Outcome.Status == NUnit.Framework.Interfaces.TestStatus.Failed)
        {
            try
            {
                await Page.ScreenshotAsync(new PageScreenshotOptions
                {
                    Path = $"../../../test-results-failure-{TestContext.CurrentContext.Test.Name}.png",
                    FullPage = true
                });
            }
            catch
            {
                // 截图失败不影响测试结果
            }
        }
    }
    
    public async Task ClearLocalStorage_RemovesKiUpdateKeys_WhenSelected()
    {
        // 1) 进入 KiUpdate 页，触发其 OnInitializedAsync 写入 _kiUpdatePageModel 与 config_KiUpdate
        await Page.GotoAsync($"{BaseUrl}/KiUpdate");
        // 等待页面渲染完成（骨架屏 renderNotComplete 会切换为真实内容：计算按钮可见）。
        // 用 Exact 避免匹配到导航菜单中的「计算器（更新中）」等按钮。
        await Expect(Page.GetByRole(AriaRole.Button, new() { Name = "计算", Exact = true }))
            .ToBeVisibleAsync();

        // 2) 前置断言：两个键确实已被页面写入
        Assert.That(await Page.EvaluateAsync<bool>($"!!localStorage.getItem('{PageModelKey}')"), Is.True,
            $"进入 KiUpdate 页后应写入 {PageModelKey}");
        Assert.That(await Page.EvaluateAsync<bool>($"!!localStorage.getItem('{ConfigCacheKey}')"), Is.True,
            $"进入 KiUpdate 页后应写入 {ConfigCacheKey}");

        // 3) 进入设置页
        await Page.GotoAsync($"{BaseUrl}/settings");
        // 等待设置页加载：清空本地存储按钮可见（Exact 避免匹配到同页的警告 Alert 文案）
        var clearBtn = Page.GetByRole(AriaRole.Button, new() { Name = "清空本地存储", Exact = true });
        await Expect(clearBtn).ToBeVisibleAsync();

        // 4) 点击「清空本地存储」→ 弹出 SelectCalculatorDialog（Exact 避免匹配到警告文案）
        await clearBtn.ClickAsync();
        // 等待对话框打开（标题「清除存储确认」出现）
        await Expect(Page.GetByText("清除存储确认")).ToBeVisibleAsync();

        // 5) 在 MudSelect 多选中勾选「升级内功词条计算器」。
        //    MudSelect 下拉项（.mud-list-item）默认收起，需先点击 select 输入框展开 popover。
        var selectInput = Page.Locator(".mud-select .mud-input-control").First;
        await Expect(selectInput).ToBeVisibleAsync();
        await selectInput.ClickAsync();
        // popover 展开后，定位包含目标文本的下拉项并点击
        var kiUpdateOption = Page.Locator(".mud-list-item", new() { HasTextString = "升级内功词条计算器" }).First;
        await Expect(kiUpdateOption).ToBeVisibleAsync(new() { Timeout = 10_000 });
        await kiUpdateOption.ClickAsync();
        // 选中一项后关闭下拉，避免遮挡对话框按钮
        await Page.Keyboard.PressAsync("Escape");

        // 6) 点击对话框内「确定」按钮（选中后该按钮由 Disabled 变为可点）
        var confirmBtn = Page.Locator(".mud-dialog-actions").GetByRole(AriaRole.Button, new() { Name = "确定" });
        await confirmBtn.ClickAsync();

        // 7) 等待 Snackbar「已成功清空 N 个计算器的本地缓存」出现，确认删除已执行
        await Expect(Page.Locator(".mud-snackbar").Filter(new() { HasText = "已成功清空" }))
            .ToBeVisibleAsync(new LocatorAssertionsToBeVisibleOptions { Timeout = 10_000 });

        // 8) 核心断言：KiUpdate 的两个 localStorage 键均已被删除
        Assert.That(await Page.EvaluateAsync<bool>($"!!localStorage.getItem('{PageModelKey}')"), Is.False,
            $"清空存储后 {PageModelKey} 应被删除");
        Assert.That(await Page.EvaluateAsync<bool>($"!!localStorage.getItem('{ConfigCacheKey}')"), Is.False,
            $"清空存储后 {ConfigCacheKey} 应被删除");

        // 9) 回归验证：再次进入 KiUpdate 页，应从配置默认值重新初始化并重新写入键
        await Page.GotoAsync($"{BaseUrl}/KiUpdate");
        await Expect(Page.GetByRole(AriaRole.Button, new() { Name = "计算", Exact = true }))
            .ToBeVisibleAsync();
        Assert.That(await Page.EvaluateAsync<bool>($"!!localStorage.getItem('{PageModelKey}')"), Is.True,
            $"清空后重新进入 KiUpdate 页应重新写入 {PageModelKey}");
    }
}
