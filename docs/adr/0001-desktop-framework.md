# ADR 0001：.NET 10 + Avalonia 桌面工作台

日期：2026-10-10。状态：已采用；Windows 本地验证，Linux/macOS 待验证。

## 决策依据

用户明确选择跨平台桌面、先 Windows，再 Linux/macOS，并优先离线编辑和模拟。商业样本 A 主程序的 PE 导入包含 QtCore4/QtGui4/QtNetwork4/QtXml4，且无 CLR 数据目录；新实现不能依赖“把 WinForms 反编译回来”。工具涉及可视画布、属性、项目树、文件对话框、字体、鼠标捕获和长期桌面交互。

选择 .NET 10 + Avalonia，包版本固定为本次从官方 NuGet 查询并成功构建的 12.1.4；SDK 固定 10.0.401。Avalonia 上游 README 明确列出 Windows、macOS、Linux 和 MIT 许可证。使用 Avalonia 自身控件及绘制，不使用商业 XPF。

| 方案 | 适合之处 | 本项目取舍 |
|---|---|---|
| WinForms | Windows 工具成熟、原生控件和既有 C# 团队迁移成本低 | 不满足用户的三桌面平台目标，不作为主 UI |
| WPF | 成熟 XAML/编辑器模型 | 平台目标与 WinForms 相同，不作为主 UI |
| Avalonia | 跨桌面平台、XAML、统一控件/渲染，适合自定义画布 | 采用；需分别验证 DPI、字体、输入法、文件选择和窗口生命周期 |
| .NET MAUI | 适合兼顾移动端的原生应用 | 当前重点是三桌面平台及复杂编辑器，且没有移动端需求，未选 |
| Uno / WebView 方案 | 可走 WinUI 或 Web 生态 | 当前没有迁移资产和 Web 交付要求，未引入第二套界面体系 |

表中“适合”是设计判断，不是对所有方案完成了同等原型或性能评测。未来因真实平台验证结果调整时，保持 Core 和文档格式独立，以新 ADR 记录。

## 代价与验收

- 文件格式、模拟、测试与 UI 分层；今后更换 UI 不重写领域模型。
- 不把 Windows 驱动、ARM 编译器或固件二进制链接进跨平台程序集。
- Windows 当前运行截图只是本机证据；Linux/macOS 必须用原生宿主运行同一工程用例。
- 当前不承诺 Native AOT；AOT、包大小和启动时间另行发布验证。
- 包许可应随发布记录生成依赖清单；新工程本身未默认选择许可证。

## 依据记录

- Avalonia 官方源仓 `AvaloniaUI/Avalonia` 的 `readme.md`，本地快照 `analysis/evidence/avalonia-upstream-readme.md`。
- NuGet 官方包 `Avalonia` / `Avalonia.Desktop` / `Avalonia.Themes.Fluent` 12.1.4，实际还原记录和 `project.assets.json`。
- 安装 PE 清单 `analysis/evidence/installation-inventory.json`，`匿名安装盘点记录`。
- 用户本会话两项明确选择；本地桌面 smoke 与内核合同回执。
