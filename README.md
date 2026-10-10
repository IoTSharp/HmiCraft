# HmiCraft · 可编程人机界面平台

英文品牌 **HmiCraft**、中文名 **可编程人机界面平台**；**IoTSharp/HmiCraft 已创建并推送，按用户最新决定保持公开**。当前代码标识仍为 `HmiStudio`，工程扩展名仍为 `.hmistudio.json`，后续命名迁移单独验收。

构建设计器和设备执行层：主入口按 **Web 编辑器 + 可选桌面/本地桥** 推进，复用 lvgl-editor 的画布，按 VB6 式工作流编辑页面、属性与事件。已有 **C# / .NET 10 + Avalonia** 离线切片保留。设计结果计划生成 **BASIC + 资源包**，设备基于 **IoTEmbedded + LVGL** 独立运行；桌面集成先 Windows，再 Linux/macOS。

基础固件内置 LVGL、控件库、BASIC 扩展和驱动；通常只更新布局 BASIC、业务脚本及图片/字体/动画资源。IoTEmbedded 负责通用 MCU/RTOS/驱动/解释器/存储，当前项目负责设计器、生成器、HMI 运行时、包/事件/变量/IO绑定和部署。首款按 4.3～7 寸、24V、触摸、RS485 的工业 HMI 研究；OPC UA 首版经网关转 Modbus/TCP。

当前是首个可运行切片，产品能力按自身路线逐步建设。已实现独立工程格式、单画面编辑、六类基础控件、撤销/重做、属性编辑和离线变量模拟。外部工程导入、宏、报警、历史、配方、PLC 通信及设备下载尚未实现。

**BASIC 生成、LVGL 固件、外设控制、网关和设计包下装均尚未实现。** 已完成它们的需求/职责规划和 IoTEmbedded 固定子模块接入。新设备闭环优先；综合商业产品、开源方案与用户需求形成独立结论，旧工程、旧 C 宏和原屏下装是后置兼容支线。

用户新增在线版整合方向：`external/LvglEditor` 已固定接入现有 React/TypeScript 编辑器。它目前主要生成 C，与 HmiCraft 的 BASIC 包链路尚未贯通；Web 主入口、离线/桌面共用 UI、本地设备桥和两个 PR 的选择性采纳见 [整合分析](docs/analysis/lvgl-editor-integration.md)。原 Avalonia 切片保留作现有能力基线。

Web 固定基线已实际检查：静态 bundle 成功，TypeScript 检查失败；已执行的 390 项 Vitest 中 386 项通过、4 项失败，依赖特定 Linux/Emscripten 环境的 C 编译测试未运行。尚未浏览器交互验收或部署站点，见 [在线基线验证](docs/verification-hmicraft-bootstrap-2026-10-10.md)。

## 启动

需要 .NET SDK 10.0.401；NuGet 依赖在 `Directory.Packages.props` 固定。

```powershell
dotnet run --project src/HmiStudio.Desktop -c Release
```

程序直接打开 800×480 离线泵站示例。在编辑模式下添加、选择、拖动控件，或在右侧修改属性。进入离线模拟后，按钮/Boolean 变量控制指示灯，Number 变量驱动数值显示；停止后恢复初始值。保存使用 `.hmistudio.json`，示例文件见 `samples/offline-demo.hmistudio.json`。

工程文件支持多个画面的存储，但当前界面只编辑第一个画面；不会把其他画面删除。变量定义与画面管理界面在后续路线中。当前变量面板显示前四项，尚无完整变量表。

**不能直接打开或生成 `.hs/.sm/.knr/.laz`。** 新工程文件不是原触摸屏下载文件，当前程序不与设备通信。

## 分析和规划

- [项目知识库](docs/knowledge/README.md)：产品定位、已确认决策、开源边界、商业/OEM/定制规则和仓库计划。
- [决策登记](docs/knowledge/decisions.md)：区分用户确认、规划结论、源码事实和未定项。
- [研究与匿名规范](docs/knowledge/research-and-attribution.md)：多类商业/开源研究形成自身结论；详细来源仅保留在仓库外本地，实际依赖准确归属。
- [路线图](ROADMAP.md)：阶段、依赖、交付 ID、验收和下一顺序。
- [开源与商业边界](docs/knowledge/open-source-boundaries.md) / [设备、OEM 与定制业务](docs/knowledge/business-and-oem.md)。
- [设备执行层与项目分工](docs/design/embedded-runtime.md)：VB6 式设计、生成 BASIC、固件/资源包、持续事件、IO 和协议职责。
- [硬件选型草案](docs/design/hardware-selection.md)：P4/STM32H7/S3 候选、厂商价格证据、工业配置、成本与验证门槛。
- [IoTEmbedded 能力基线](docs/analysis/iotembedded-baseline.md)：固定提交的真实能力、缺口和上游改进清单。
- [设备目录入口](firmware/README.md) / [设备架构决策](docs/adr/0002-embedded-hmi-runtime.md)。
- [原程序首轮分析](docs/analysis/overview.md)：安装身份、REA 证据、已知与未知。
- [帮助文档分析](docs/analysis/manual-analysis.md)：46 项功能矩阵、10 条用户旅程及格式冲突。
- [设备与脚本契约](docs/analysis/device-and-script-contracts.md)：型号、PLC 配置、宏 API、文件关联及异常。
- [REA / Ghidra 原生诊断](docs/analysis/native-troubleshooting.md)：已验证的引擎能力、适配器限制和未解决的超时。
- [架构](docs/architecture.md) / [桌面框架决策](docs/adr/0001-desktop-framework.md)。
- [质量门禁](docs/quality-gates.md) / [交接](HANDOFF.md)。

## 验证

```powershell
dotnet build HmiStudio.slnx -c Release --disable-build-servers -m:1 -p:UseSharedCompilation=false
dotnet run --project tests/HmiStudio.Core.Tests -c Release --no-build
dotnet format HmiStudio.slnx --verify-no-changes --no-restore
```

有界验证入口：`eng/verify.ps1`。桌面 smoke 会打开本任务窗口、测试编辑/文件/模拟流程、保存截图并退出；它不代替鼠标交互、辅助功能、各平台或真实硬件验收。

2026-10-10 本次仓库交付的 .NET 回归：Release 0 警告 / 0 错误、11/11 核心合同和格式检查通过，见 [本轮验证](docs/verification-hmicraft-bootstrap-2026-10-10.md)。前一轮 8 项 Windows 桌面 smoke 已通过并检查截图，见 [桌面验证](docs/verification-2026-10-10.md)；本轮未改桌面代码，未重复 GUI smoke。Web 基线的失败与未测范围另列，不能用 .NET 的通过代替。

详细研究来源、原始材料、提取内容与历史回执仅存放在仓库之外的本地研究目录。构建验证可生成被忽略的 `artifacts/`；带研究身份的材料必须另行归档。新实现不复制外部固件、图标或图库。当前源码仓库公开，项目许可证尚未定稿，也尚未发布正式版本或固件。

面向开发者/小团队和设备厂商：计划自售设备与套件，支持第三方 OEM，并提供定制服务。开源核心、独立商业工具与客户专属成果的范围已进入规划；MIT/MPL-2.0 组合仍待发布前按文件定稿，目前没有新增根 LICENSE。研究记录见 [商业方案](docs/design/product-and-business-proposal.md)。

## 基础框架子模块

`external/IoTEmbedded` 固定为 `e09e21ff14a7505b4c4c65e084b19293c54bb1e0`；`external/LvglEditor` 固定为 `0d0d62f655442dd5624a00390b7652222a7b3fc8`。两个子模块源码均未修改。父仓 `main` 已推送到 `https://github.com/IoTSharp/HmiCraft.git`，GitHub 可见性与提交回读已核对。

有仓库权限的开发者克隆父仓后，运行 `git submodule update --init external/IoTEmbedded external/LvglEditor` 检出固定版本；不要自动追随远端 HEAD。首次交付记录见 [仓库状态](docs/knowledge/repository-and-naming.md) 和 [验证](docs/verification-hmicraft-bootstrap-2026-10-10.md)。

设备代码和宿主验证使用独立构建入口，不在启动设计器时编译或烧录固件。没有因为引入框架而安装交叉编译器或运行上游应用。
