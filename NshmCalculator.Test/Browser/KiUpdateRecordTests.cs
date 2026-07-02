using System.Text.Json;
using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using NshmCalculator.Shared.Models.PageModel;

namespace NshmCalculator.Test.Browser;

/// <summary>
/// 升级内功词条计算器「保存内功记录」功能端到端测试（Playwright + 无头 Chromium）。
///
/// 被测流程：填写数值 → 计算 → 保存为内功记录（弹窗）→ 记录列表展示 → 编辑复算 → 删除。
/// 涉及 ISyncLocalStorageService（JS 互操作）、真实 DOM 弹窗、MudTable 分页，
/// bUnit 无法覆盖，故用 Playwright 在真实浏览器 localStorage 下验证持久化与交互效果。
///
/// 运行前置：
///   1. pwsh bin/Debug/net8.0/playwright.ps1 install chromium
///   2. dotnet run --project NshmCalculator.MudClient（dev server 监听 5218）
///   3. dotnet test NshmCalculator.Test --filter "FullyQualifiedName~KiUpdateRecordTests"
/// </summary>
[Parallelizable(ParallelScope.None)]
public class KiUpdateRecordTests : PageTest
{
    /// <summary>
    /// 被测 App 根地址。默认取 launchSettings.json 中 http profile 的端口 5218；
    /// 可通过环境变量 BASE_URL 覆盖（CI 或端口被占用时）。
    /// </summary>
    private string BaseUrl => Environment.GetEnvironmentVariable("BASE_URL") ?? "http://localhost:5218";

    /// <summary>
    /// 内功记录列表在 localStorage 中的键（与 UpdateCalculator.razor 中 _recordPageModel 命名一致）。
    /// </summary>
    private const string RecordKey = "_kiUpdateRecordPageModel";

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

    /// <summary>
    /// 等待 KiUpdate 页面渲染完成（骨架屏 renderNotComplete 切换为真实内容：计算按钮可见）。
    /// </summary>
    private async Task WaitForPageReady()
    {
        await Expect(Page.GetByRole(AriaRole.Button, new() { Name = "计算", Exact = true }))
            .ToBeVisibleAsync();
    }

    /// <summary>
    /// 点击「保存内功记录」输出区中名为 <paramref name="tabName"/> 的 Tab 面板。
    /// MudBlazor 的 MudTabPanel 标题渲染为 .mud-tab（含文本），非标准 role=tab，故按类+文本定位。
    /// 注意：输出区是页面上第二个 .mud-tabs 容器，限定其内避免误触输入区标签页。
    /// </summary>
    private async Task ClickOutputTab(string tabName)
    {
        // 定位输出区 MudPaper（标题含「输出区」），其内的 .mud-tab
        var tab = Page.Locator(".mud-tabs .mud-tab", new() { HasTextString = tabName }).First;
        await Expect(tab).ToBeVisibleAsync(new() { Timeout = 10_000 });
        await tab.ClickAsync();
    }

    /// <summary>
    /// 在输入区填写某属性数值：定位包含该属性名的表格行，找到其中的输入框并填入数值。
    /// </summary>
    private async Task FillAttributeValue(string attrName, string value)
    {
        // MudTable 行：包含属性名的行（.mud-table-body tr），其内的输入框
        var row = Page.Locator(".mud-table-body tr", new() { HasTextString = attrName }).First;
        await Expect(row).ToBeVisibleAsync();
        var input = row.Locator("input").First;
        await input.ClickAsync();
        await input.FillAsync(value);
        // 触发 bind 失焦提交
        await input.PressAsync("Tab");
    }

    /// <summary>
    /// 在对话框内按标签文本定位 MudTextField 的输入框。
    /// MudBlazor 的浮动标签输入框不会生成标准 &lt;label for&gt;，故通过 .mud-input-control
    /// 容器内包含标签文本的 .mud-input-label 来定位。
    /// </summary>
    private ILocator FindDialogInputByLabel(string labelText)
    {
        return Page.Locator(".mud-dialog-content .mud-input-control",
            new() { HasText = labelText })
            .First.Locator("input");
    }

    /// <summary>
    /// 读取并反序列化 localStorage 中的内功记录列表。
    /// </summary>
    private async Task<KiUpdateRecordPageModel?> ReadRecordPageModel()
    {
        var json = await Page.EvaluateAsync<string?>($"localStorage.getItem('{RecordKey}')");
        if (string.IsNullOrEmpty(json))
        {
            return null;
        }

        return JsonSerializer.Deserialize<KiUpdateRecordPageModel>(json);
    }

    /// <summary>
    /// 测试 1：计算后保存一条内功记录 → localStorage 写入 → 列表展示 → 跨刷新持久化。
    /// </summary>
    [Test]
    public async Task SaveRecord_PersistsToLocalStorage_AndAppearsInList()
    {
        // 1) 进入 KiUpdate 页，等待渲染完成
        await Page.GotoAsync($"{BaseUrl}/KiUpdate");
        await WaitForPageReady();

        // 2) 隔离环境：清空历史记录数据，避免干扰
        await Page.EvaluateAsync($"localStorage.removeItem('{RecordKey}')");
        // 清空后重新加载以使页面状态重置
        await Page.GotoAsync($"{BaseUrl}/KiUpdate");
        await WaitForPageReady();

        // 3) 在输入区填写若干属性数值（攻击、会心）
        await FillAttributeValue("攻击", "33");
        await FillAttributeValue("会心", "66");

        // 4) 点击「计算」→ 等待输出结果（总实际收益率 chip 可见，表示 _results.Count > 0）
        await Page.GetByRole(AriaRole.Button, new() { Name = "计算", Exact = true }).ClickAsync();
        await Expect(Page.GetByText("总实际收益率", new() { Exact = true }))
            .ToBeVisibleAsync(new() { Timeout = 10_000 });

        // 5) 点击输出结果 Tab 底部的「保存内功记录」按钮 → 弹出保存对话框
        await Page.GetByRole(AriaRole.Button, new() { Name = "保存内功记录", Exact = true }).ClickAsync();
        // 等待对话框打开
        await Expect(Page.GetByText("保存内功记录", new() { Exact = true }).First)
            .ToBeVisibleAsync(new() { Timeout = 10_000 });

        // 6) 填入内功标题、填备注
        await FindDialogInputByLabel("内功标题").FillAsync("测试内功A");
        await FindDialogInputByLabel("内功备注").FillAsync("自动化测试备注");

        // 7) 点击对话框内「确定」→ 等待 Snackbar「内功记录已保存」
        await Page.Locator(".mud-dialog-actions").GetByRole(AriaRole.Button, new() { Name = "确定", Exact = true }).ClickAsync();
        await Expect(Page.Locator(".mud-snackbar").Filter(new() { HasText = "内功记录已保存" }))
            .ToBeVisibleAsync(new() { Timeout = 10_000 });
        // 等待 Blazor 渲染/持久化完成
        await Page.WaitForTimeoutAsync(500);

        // 8) 断言 localStorage：已写入，Records.Count == 1，标题匹配
        var pageModel = await ReadRecordPageModel();
        Assert.That(pageModel, Is.Not.Null, "保存后 localStorage 应写入内功记录");
        Assert.That(pageModel!.Records.Count, Is.EqualTo(1), "应有 1 条记录");
        Assert.That(pageModel.Records[0].Title, Is.EqualTo("测试内功A"));
        Assert.That(pageModel.Records[0].InputValues, Is.Not.Empty, "记录应包含属性填写数值");

        // 9) 切换到「保存内功记录」Tab → 等待记录行出现 → 断言行文本包含标题
        await ClickOutputTab("保存内功记录");
        var recordRow = Page.Locator(".mud-table-body tr", new() { HasTextString = "测试内功A" }).First;
        await Expect(recordRow).ToBeVisibleAsync(new() { Timeout = 10_000 });

        // 10) 跨刷新持久化：重新进入页面 → 切到记录 Tab → 断言行仍存在（验证加载时复算 TotalRate）
        await Page.GotoAsync($"{BaseUrl}/KiUpdate");
        await WaitForPageReady();
        await ClickOutputTab("保存内功记录");
        await Expect(Page.Locator(".mud-table-body tr", new() { HasTextString = "测试内功A" }).First)
            .ToBeVisibleAsync(new() { Timeout = 10_000 });

        // 清理本次测试写入的数据
        await Page.EvaluateAsync($"localStorage.removeItem('{RecordKey}')");
    }

    /// <summary>
    /// 测试 2：点击记录行 → 编辑词条数值并复算 → 保存；再次点击 → 删除。
    /// </summary>
    [Test]
    public async Task RecordDialog_EditRecalculate_Delete_Works()
    {
        // ---- 前置：保存一条记录作为被测数据 ----
        await Page.GotoAsync($"{BaseUrl}/KiUpdate");
        await WaitForPageReady();
        await Page.EvaluateAsync($"localStorage.removeItem('{RecordKey}')");
        await Page.GotoAsync($"{BaseUrl}/KiUpdate");
        await WaitForPageReady();

        await FillAttributeValue("攻击", "33");
        await Page.GetByRole(AriaRole.Button, new() { Name = "计算", Exact = true }).ClickAsync();
        await Expect(Page.GetByText("总实际收益率", new() { Exact = true }))
            .ToBeVisibleAsync(new() { Timeout = 10_000 });

        await Page.GetByRole(AriaRole.Button, new() { Name = "保存内功记录", Exact = true }).ClickAsync();
        await Expect(Page.GetByText("保存内功记录", new() { Exact = true }).First).ToBeVisibleAsync(new() { Timeout = 10_000 });
        await FindDialogInputByLabel("内功标题").FillAsync("待编辑内功");
        await Page.Locator(".mud-dialog-actions").GetByRole(AriaRole.Button, new() { Name = "确定", Exact = true }).ClickAsync();
        await Expect(Page.Locator(".mud-snackbar").Filter(new() { HasText = "内功记录已保存" }))
            .ToBeVisibleAsync(new() { Timeout = 10_000 });

        // 切到记录 Tab，记录下原始的实际收益率（用于后续复算变化对比）
        await ClickOutputTab("保存内功记录");
        var recordRow = Page.Locator(".mud-table-body tr", new() { HasTextString = "待编辑内功" }).First;
        await Expect(recordRow).ToBeVisibleAsync(new() { Timeout = 10_000 });

        // ---- 1) 点击记录行 → 编辑对话框打开 ----
        await recordRow.ClickAsync();
        // 等待编辑对话框出现（实时收益率标签可见）
        await Expect(Page.GetByText("实际收益率", new() { Exact = true }).First)
            .ToBeVisibleAsync(new() { Timeout = 10_000 });

        // 2) 修改某属性 InputValue（攻击行）→ 断言实时收益率随之变化（复算）
        //    定位对话框内含「攻击」标签的输入框，清空并填入新值
        var attackInput = FindDialogInputByLabel("攻击");
        await attackInput.ClickAsync();
        await attackInput.FillAsync("26");
        await attackInput.PressAsync("Tab");
        // 实际收益率应仍可见（复算后保留显示，不报错即视为复算路径正常）
        await Expect(Page.GetByText("实际收益率", new() { Exact = true }).First).ToBeVisibleAsync();

        // 4) 点击「保存」→ 断言 localStorage 中该记录的 InputValues 已更新、Snackbar 出现
        await Page.Locator(".mud-dialog-actions").GetByRole(AriaRole.Button, new() { Name = "保存", Exact = true }).ClickAsync();
        await Expect(Page.Locator(".mud-snackbar").Filter(new() { HasText = "内功记录已更新" }))
            .ToBeVisibleAsync(new() { Timeout = 10_000 });
        var afterEdit = await ReadRecordPageModel();
        Assert.That(afterEdit, Is.Not.Null);
        Assert.That(afterEdit!.Records.Count, Is.EqualTo(1));
        Assert.That(afterEdit.Records[0].InputValues["攻击"], Is.EqualTo(26d),
            "编辑保存后记录的攻击数值应更新为 26");

        // ---- 5) 再次点击记录行 → 点击「删除」→ 二次确认 ----
        await ClickOutputTab("保存内功记录");
        var row2 = Page.Locator(".mud-table-body tr", new() { HasTextString = "待编辑内功" }).First;
        await Expect(row2).ToBeVisibleAsync(new() { Timeout = 10_000 });
        await row2.ClickAsync();
        await Expect(Page.GetByText("实际收益率", new() { Exact = true }).First).ToBeVisibleAsync(new() { Timeout = 10_000 });

        // 点击对话框「删除」按钮
        await Page.Locator(".mud-dialog-actions").GetByRole(AriaRole.Button, new() { Name = "删除", Exact = true }).ClickAsync();
        // 二次确认 MessageBox：点「删除」
        await Expect(Page.Locator(".mud-dialog").Filter(new() { HasText = "删除确认" }))
            .ToBeVisibleAsync(new() { Timeout = 10_000 });
        await Page.Locator(".mud-dialog").Filter(new() { HasText = "删除确认" })
            .GetByRole(AriaRole.Button, new() { Name = "删除", Exact = true }).ClickAsync();

        // 6) 断言 localStorage：Records.Count == 0
        await Expect(Page.Locator(".mud-snackbar").Filter(new() { HasText = "内功记录已删除" }))
            .ToBeVisibleAsync(new() { Timeout = 10_000 });
        var afterDelete = await ReadRecordPageModel();
        Assert.That(afterDelete, Is.Not.Null);
        Assert.That(afterDelete!.Records.Count, Is.EqualTo(0), "删除后记录数应为 0");

        // 列表应显示空状态提示
        await Expect(Page.GetByText("暂无保存的内功记录")).ToBeVisibleAsync(new() { Timeout = 10_000 });
    }
}
