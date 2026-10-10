# 2026-10-10 本机验证

工作区：`D:\source\HMIlStudio`。运行环境：Windows、PowerShell 7.6.6、.NET SDK 10.0.401；依赖锁定 Avalonia 12.1.4。本轮格式整理之后，统一入口 `eng/verify.ps1 -DesktopSmoke` 完整执行成功。

运行标识：`verify-20261010-211642-2818da`。各步均由 `eng/Invoke-BoundedProcess.ps1` 限时监督，保存标准输出、标准错误、根进程身份及观察到的子进程身份。

| 步骤 | 结果 | 回执 |
|---|---|---|
| 依赖还原 | 退出 0 | `artifacts/process/verify-20261010-211642-2818da-restore.stdout.log` |
| Release 构建 | 退出 0；0 警告、0 错误 | `artifacts/process/verify-20261010-211642-2818da-build.stdout.log` |
| 核心合同测试 | 退出 0；11/11 | `artifacts/process/verify-20261010-211642-2818da-contracts.stdout.log` |
| format verify | 退出 0；无需修改 | `artifacts/process/verify-20261010-211642-2818da-format.stdout.log` |
| Windows 桌面 smoke | 退出 0；8 项通过 | `artifacts/verify-20261010-211642-2818da/desktop/desktop-smoke.json` |

桌面检查覆盖真实窗口渲染、添加、撤销、重做、拖动中断收尾、文件操作门禁、保存/读取以及离线 Boolean 切换。截图 `artifacts/verify-20261010-211642-2818da/desktop/desktop-smoke.png` 已人工检查：画布、属性面板、对象列表、变量和离线模拟状态正常显示。

桌面进程另输出 `Ignore caches that are heterogeneous` 和 libpng `iCCP: known incorrect sRGB profile` 警告。该次窗口及检查完成，警告保留在 stdout/stderr，未据此作无警告运行声明；“0 警告”仅指 .NET 构建。

核心测试验证 schema/未知字段拒绝、ID/名称去重、几何/颜色/数值与绑定约束、快照及模拟状态隔离、非法写拒绝、保存取消/失败保留旧字节、替换失败清理自身临时文件、空值与超大文档拒绝。测试为 console runner，使用 `dotnet run`，并非 `dotnet test`。

桌面 smoke 的拖动中断检查直接调用收尾逻辑，没有合成操作系统级鼠标捕获事件；文件操作门禁检查没有在慢网络磁盘实测。文件选择框、输入法、DPI/多屏、辅助功能、长路径、并发保存和断电恢复仍未完成验收。

原软件未运行；原文件格式、宏、PLC、触摸屏、Linux 和 macOS 均不在本轮通过范围。失败的早期构建、REA 超时和直接 Ghidra 路径诊断回执保留。

随后仅修改交接/报告和 REA 客户端的回执命名与独占写入，不修改已通过上述验证的 C# / XAML / 项目文件。REA 客户端修改另做 Node 语法检查，没有借此宣称重新验证原生桥接。
