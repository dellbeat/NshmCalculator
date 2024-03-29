# 逆水寒手游在线计算器

***

![License](https://img.shields.io/github/license/dellbeat/NshmCalculator)
![Build](https://img.shields.io/badge/build-passing-brightgreen)
![Version](https://img.shields.io/badge/version-1.0.3-brightgreen)
![PWA Support](https://img.shields.io/badge/PWA-support-blue)

一款集合了网络上部分计算手游属性数值的在线计算器，方便手机端的小伙伴使用。

支持静态部署，如部署无误即可在手机端浏览器打开，且部署环境如满足QQ/微信的外部网站要求，可在QQ/微信内直接打开。

支持PWA功能，可在计算器加载后作为应用安装在本地（Chrome内核浏览器功能实现，可随时卸载/移除）。

## 计算器功能

* 在网页内计算各类数值，如素问治疗量/增伤率/属性收益/内功收益等。 详细功能可参考下方适配情况一节。


* 计算数据支持本地存储，包含填写在计算界面的数据、计算结果数据（内功收益计算器内“动态计算基础词条加成”除外）和内功数据。

## 计算器适配情况

* 增伤百分比计算器：已实现原工具内增伤率计算功能。（1/1）
* 素问治疗量计算器：已实现原工具内治疗量计算功能。（1/1）
* 属性收益计算器：已实现原工具内期望伤害计算/属性变化伤害提升计算/属性提升收益曲线图；面板对比器暂无实现计划。（3/4）
* 内功收益计算器：已实现原工具内属性提升率计算/内功数据存储、计算/动态计算基础词条加成；内功特性增伤表、周天增伤率简表暂无实现计划。（3/5）

## 内容来源及鸣谢

B站<b>@白宝正不正</b>：[属性计算器不再纠结词条特技装备收益与选择](https://www.bilibili.com/video/BV1G94y1x7sW) （增伤百分比计算器）

B站<b>@白宝正不正</b>：[新手向治疗素问养成一图流全攻略](https://www.bilibili.com/video/BV1DP411Y7MK) （素问治疗量计算器）

贴吧<b>@星语困</b>： [独家首发！！逆水寒手游属性收益计算器！](https://tieba.baidu.com/p/8530450428) （属性收益计算器）

NGA<b>@UID:42330896</b>： [[攻略]内功计算器1.2.2南问雪版](https://g.nga.cn/read.php?tid=38987417&rand=308) （内功收益计算器）

[arkntools/arknights-toolbox](https://github.com/arkntools/arknights-toolbox) （设计思路参考）

B站<b>@乾珏Zzz</b>：[不用在辛苦找人调号啦！PVE属性收益计算器，解决你装备内功选择困难症](https://www.bilibili.com/video/BV1f6421G7Vm/)（计算公式参考）

## 开发契机和历程

<details> 
<summary>契机</summary>

从手游开服开始，在网络上就有各路大神根据各类数据结果推测出公式并制作计算器供大家使用。

彼时帮会里的也有在使用这些计算器的小伙伴，但是部分使用手机的小伙伴反馈操作不是很方便；然后我就想着能不能做一个在线使用的计算器，让需要使用的人直接点击链接填入需要的数据就能计算。

以网页呈现的原因是为了尽量降低使用门槛，微信小程序会限制使用范围（且帮会群是QQ群）。
</details>

<p/>

<details> 
<summary>历程</summary>

不过这个想法搁置了很久一段时间，因为自己彼时工作事情比较多再加上准备使用的技术栈不熟（是VUE，而自己主做WPF桌面开发）。

后面了解到有适合自己技术栈的实现方法，便开始了计算器公式的分析拆解和在线计算器开发工作。

第一版内有增伤百分比计算器/素问治疗量计算器/属性收益计算器，但因为界面设计不太满意，于是在制作完后又搁置了一段时间。

再到后面有看到内功收益计算器，于是先拆解了算法；在解决了命中计算的问题后，重新搭建了项目框架，最后有了现在的在线计算器。

对比原来第一版的计算器界面相比大气，不会像第一版的界面太过窄巴。
</details>

## 部署指南

出这个指南的原因是，担心自己后续可能因为各类事务繁忙不能再投入很多精力到这里；提前写好部署指南，希望能帮到需要的人。

我会在写好README后将最新可部署的内容传在Releases上，可移步查看。

***

发布条件：VS2022（我自己的是17.8.0） + .NET 8 SDK

部署条件：支持静态网站托管的服务器

### 发布步骤

1.克隆本项目源码至本地

2.打开NshmCalculator.sln解决方案文件

3.在解决方案资源管理器上右键解决方案，选择“还原Nuget包”

4.待还原完毕后选中项目“NshmCalculator.MudClient”，点击发布

5.新弹出的窗口会提示发布目标，这里选择“文件夹”即可

6.指定发布的文件夹位置并点击完成

7.完成后会有发布界面，点击“显示所有设置”，在弹出的窗口内，点击“文件发布选项”，勾选“在发布前删除所有文件”即可。

8.点击发布，等待文件发布完毕。

### 部署步骤

1.找到发布时设定的文件夹

2.请根据静态网站托管的要求进行内容托管即可，一般将wwwroot内的所有文件直接放在网站根目录即可。

### 已知问题

* 如将发布后的资产推送到Github仓库，可能会在浏览器控制台出现报错现象，暂时未发现影响正常使用的情况。


* 如果网站内容（Index.html）不在根目录可能需要自行处理跳转问题

## 应该不会有人需要的二开指南

该项目基于.Net 8 + Blazor WebAssembly进行开发，使用了MudBlazor作为页面主控件库，使用Blazor-ApexCharts展示页面图表。

项目结构如下：

* NshmCalcuator.Shared：该项目内为模型类和计算类，和页面逻辑进行分离。
* NshmCalculator.MudClient：该项目为前端页面项目，主要包含了布局结构、自定义组件和页面等
* NshmCalculator.Test：该项目为单元测试项目，主要目标为对Shared项目中计算类进行单元测试。目前对计算类的覆盖率为100%，且正确率为100%

开发工具建议：VS2022 或 Rider 2023.3

VS2022支持热重载，对组件进行布局调整的时候可直接热重载，无需重启项目，比较方便。

Rider除热重载、发布之外可满足对该项目的开发需求，布局比较养眼。

如果真的有需要二开可Issue联系我，我会收到相关邮件的。