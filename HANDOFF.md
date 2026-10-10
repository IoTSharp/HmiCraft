# HmiCraft · 可编程人机界面平台交接

更新：2026-10-10，Asia/Shanghai。工作区 `D:\source\HMIlStudio` 从空目录开始，已初始化 Git `main`。最新用户决定与本轮验证列于下节；历史分析和验证保留原时间边界。

## HmiCraft 知识、在线版与首次仓库交付

- 中文名“可编程人机界面平台”，英文名**HmiCraft**；IoTSharp/HmiCraft已创建并推送main，用户最新明确**保持公开**。原始根提交已匿名化重写、带明确lease强推成功，可达历史扫描零命中，旧本地提交对象已清理；最终SHA以本轮回读为准。
- 必要资料及历史回执已完整移动到仓库之外，原analysis/artifacts目录不再位于工作区；同盘目录移动，未删除研究文件。58项文件/gitlink、30份Markdown、129个本地链接和106个路线任务的内容审计通过；移出前87个已记录进程身份均无仍匹配进程。
- 研究范围覆盖多类商业与开源产品；仓库保留匿名结论、需求和设计选择，实际依赖名称与许可准确保留。详细来源和历史回执只留仓库外本地目录，规则见 [研究规范](docs/knowledge/research-and-attribution.md)。
- `docs/knowledge/` 已保存名称、技术/设备分工、开源与商业边界、开发者与OEM客户、自售设备、第三方OEM、定制成果归属。GOV/BIZ/LVE任务和 ADR 0003 记录归属、依赖与验收。许可仍为 MIT/MPL-2.0 方案，没有新增根 LICENSE。
- 用户新增 lvgl-editor 在线方向；推荐 Web 主入口、本地工程、可选桌面/本地桥，复用同一 UI；保留 .NET 核心及已有 Avalonia 切片。BASIC 布局、用户脚本分离、IoTEmbedded/LVGL 设备分工继续有效。
- `external/LvglEditor` 已作为第二个真实 Git 子模块固定到 `0d0d62f655442dd5624a00390b7652222a7b3fc8`，源码未改。上游 README 声明 MIT，但缺正式许可文件；LVE-02/GOV-04 在公开分发前处理。
- PR #2（`d8a0efe19eddcb58b4c4b7bd6a7ad65361862754`）是 OmniHost 桌面草稿，macOS 未实现，资源路径、私有包源和 .NET 版本需验证。PR #3（`3647ce025bc65cd14c968c1d329dc40d593f7e51`）大幅转向 LispBM/VESC；通用画布/资源/状态/i18n 改进逐项选择，不整体替代 BASIC。**未合并或采纳两个 PR 的代码，未运行它们的构建。**
- Web main 验证：安装依赖退出0；`tsc -b` 退出2；Vitest 390项中386通过/4个字体断言失败（退出1）；静态 Vite bundle退出0。特定 Linux/Emscripten 的 `compile.test.ts` 未运行；未执行浏览器 GUI smoke、离线缓存或设备下载。Web 仍为 partial，不能发布就绪。
- .NET 回归 runId `verify-20261010-225923-c29f48`，以及文本行尾规范化后的最终回归 `verify-20261010-230650-b648f3`：restore/build/console合同/format全退出0，Release 0警告/0错误，11/11合同通过；未改桌面源码，未重复 GUI。
- 修复 `.gitignore`：根 `/analysis/`、`/artifacts/` 继续排除原始材料/回执，自编 `docs/analysis/` 能被正常提交。原 外部样本 资源、安装包、凭据及 `node_modules/dist` 不进入父仓。
- 验证入口：[本轮仓库与Web验证](docs/verification-hmicraft-bootstrap-2026-10-10.md)；方案：[Web/PR整合](docs/analysis/lvgl-editor-integration.md)。Web 原始回执前缀 `artifacts/process/hmicraft-web-*`，分析 checkout `analysis/work/lvgl-editor-baseline` 保留。

### 最新实施顺序

1. LVE-08 修 Web 类型/字体回归并完成浏览器基础旅程，不能以 Vite 打包绕过类型门禁。
2. E0-04/05 冻结公共模型、BASIC API、包、单板与资源预算；LVE-04/05 将 Web 工程接到同一 BASIC/资源链路。
3. IEB-01/05/06/11 与 HMI-03～06/09 补通用主机 port、同一 BASIC/LVGL runtime，先六控件及事件。
4. LVE-07、IEB显示/输入/IO/协议、HMI-07/08 与 INT-03 完成单板包更新及实际反馈。
5. LVE-09 逐项采纳 PR #3 通用改进；LVE-10 按真实离线/设备需求处理 PR #2 菜单和桌面宿主。许可、品牌迁移、工业和商业门禁分别推进。

以下保留早期交付记录，其中无提交/无远端等描述仅代表当时状态，不替代本节最终仓库事实。

## 用户决策与当前状态

- 使用 REA 分析 商业样本 A，按分析结果规划 C# 重写。
- 已选择跨平台桌面，先 Windows，再 Linux/macOS；优先离线编辑与模拟，先找自带样例。
- 用户批准官方 Ghidra 12.1.4 安装及接入 REA，安装、摘要校验和配置写入已完成。
- 首轮安装/文档/设备配置分析、路线图及离线初版已交付。整个程序的函数级逆向、旧工程兼容和设备兼容仍未完成。

## 本轮新增需求与交付

- 用户明确本项目是 VB6 式设计器，设备端使用 IoTEmbedded + LVGL；基础固件内置控件库，布局也由设计器生成 BASIC，用户 BASIC 与生成代码分离。普通更新只下发资源和脚本。
- 用户确认：持续事件循环、定时器/变量变化/界面/外设操作，脚本受预算限制；基础动作可配置，复杂逻辑 BASIC；旧 C 宏独立规划。
- 用户确认工业 HMI 4.3～7 寸、24V、触摸、RS485；GPIO/温度/位置/计数/开关/模拟量采集、ADC/PWM、串口/Modbus/TCP；高速计数由硬件完成，不做 BASIC 轴运动闭环。
- 用户确认 OPC UA 首版经网关转 Modbus/TCP，屏端不直接运行 OPC UA；新设备闭环优先，旧工程和旧屏兼容后置。
- 硬件尚未指定，用户要求联网研究成本/性能；已记录厂商规格、价格快照和参考配置，建议 P4 工业架构、STM32H7 备选，S3 作为低成本评估候选。具体 SKU/IO通道/工业等级/预算/性能还需冻结和实测，不是已采购配置。
- `external/IoTEmbedded` 已作为真正 Git 子模块引入，固定 `e09e21ff14a7505b4c4c65e084b19293c54bb1e0`（2026-06-01）。源码未改、未编译/运行/烧录上游；没有安装工具链或 LVGL。
- 新文档：[设备职责与合同](docs/design/embedded-runtime.md)、[框架能力基线](docs/analysis/iotembedded-baseline.md)、[硬件选型](docs/design/hardware-selection.md)、[ADR 0002](docs/adr/0002-embedded-hmi-runtime.md)、[设备目录入口](firmware/README.md)。主 ROADMAP 新增 E0 / IEB / HMI / INT 路线和责任项目，原兼容线保留。
- 关键源码发现：没有已接入 LVGL；显示接口缺像素块/完成合同；按键待驱动；BASIC 同步运行/4KiB脚本槽不等于 HMI runtime；L475 无 AT24C 后端；尚无本轮可确认的 Modbus TCP/OPC UA；README/ARCHITECTURE 部分目录描述领先于实际源码。
- 源码与网络读取回执位于 `artifacts/process/iotembedded-*-20261010-a1.*`（协议第二次为 a2）和 `analysis/evidence/hardware-fetch-{1,3}.json`。前三张厂商 HTML 已保留，web 403 后按规定单请求代理获取；没有购买、提交 PR 或对外写入。

## 工具与目标

- REA `rea-agents@6.3.0`；CLI `C:\Users\mysti\AppData\Roaming\npm\rea.ps1`；实际入口 `C:\Users\mysti\AppData\Roaming\npm\node_modules\rea-agents\scripts\rea.mjs`。
- 技能 `C:\Users\mysti\.agents\skills\reverse-engineer-anything\SKILL.md`；早期通过 CLI/独立 stdio 客户端取证。2026-10-10 本轮收尾的工具发现已可见 `mcp__rea__*`，但未重新打开二进制或验证桥接；工具可见不等于会话运行成功。不要反复运行 setup。
- Ghidra `C:\Users\mysti\.local\share\ghidra\ghidra_12.1.4_PUBLIC`；JDK `C:\Program Files\Microsoft\jdk-21.0.7.6-hotspot`。官方 ZIP SHA-256 `ddac49f903da9d5bac833e5cc79395098b9c33cfd3279be5f31bd00387d2d4db`，回执 `analysis/evidence/ghidra-install.json`。
- Codex 配置 `C:\Users\mysti\.codex\config.toml` 已登记 Ghidra/JDK；备份 `C:\Users\mysti\.codex\config.toml.before-rea-ghidra-20261010`。
- .NET SDK 10.0.401：`C:\Program Files\dotnet\dotnet.exe`；PowerShell 7.6.6：`C:\Program Files\PowerShell\7\pwsh.exe`；Node v24.15.0；7-Zip：`C:\Program Files\7-Zip\7z.exe`。
- 研究样本安装与入口的真实身份保留在忽略的私有研究清单，不纳入仓库文档。
- 匿名商业样本 A 为原生 x86 PE，无 CLR；源安装只读，未运行原应用/模拟器或连接设备。原摘要仅保留在私有证据中。

## 已有证据与代码

- [首轮分析](docs/analysis/overview.md)：6,370 文件盘点、REA artifact 身份、导入/导出、已知与未知。
- [帮助分析](docs/analysis/manual-analysis.md)：46 功能域、10 用户旅程、648 页面/目录索引；文档声明需运行证据对照。
- [设备契约](docs/analysis/device-and-script-contracts.md)：72 唯一型号、139 PLC 配置、51 DLL、34 宏 API 声明及异常；ABI/协议未知。
- 安装 `Project` 为空，安装清单中没有旧工程样例；不能拿自建 JSON 示例冒充原样例。
- [原生诊断](docs/analysis/native-troubleshooting.md)：REA 主 EXE 300 秒和模拟器 120 秒索引超时；Windows P0 拒绝 DLL。直接 Ghidra 对一个小型DLL同摘要副本分析成功、退出 0，只证明该引擎/JDK 通路，REA 桥接仍未验证。
- REA launcher 自动分析结束后才启动桥接，timeout 映射丢失详细 diagnostics。下一次先保存启动日志并用受支持的小型 PE application 验证，不能仅增大超时重跑大程序。
- 早期 finally 关闭回执被后次覆盖，只保留最后一份 `rea-close-finally.json`；现 `eng/rea-session.mjs` 按 plan 分开命名并拒绝覆盖回执。记录的相关进程已退出，不能补造早期每次关闭的独立证据。
- `.NET 10 + Avalonia 12.1.4`；`src/HmiStudio.Core` 负责工程/校验/JSON/离线状态，`src/HmiStudio.Desktop` 提供编辑工作台。只编辑第一画面，保留其他画面，变量面板前四项；当前 UI code-behind 是首个切片，后续提取命令/ViewModel。
- 六类控件、选择/拖动/删除、属性/绑定、100 步撤销重做、未保存保护、打开/保存、离线 Boolean 与 Number 模拟。独立 `.hmistudio.json`，不支持 `.hs/.sm/.knr/.laz`。示例 `samples/offline-demo.hmistudio.json` 为自有 800×480 泵站。

## 前一轮桌面验证

`eng/verify.ps1 -DesktopSmoke` 全部成功，runId `verify-20261010-211642-2818da`：Release 0 警告/0 错误、11/11 console 核心合同、format verify、8 项 Windows 桌面 smoke；截图已看过。详见 [本轮验证](docs/verification-2026-10-10.md)。桌面运行另有 libpng/profile 和缓存警告，未影响该轮通过；不要称运行无警告。没有 Linux/macOS、旧程序格式或硬件验收。

启动：在工作区执行 `dotnet run --project src/HmiStudio.Desktop -c Release`。

## 下一轮顺序（以新设备主线为准）

本轮规划后的 `eng/verify.ps1` 已通过，runId `verify-20261010-215957-c53cf3`：restore、Release、11/11 console 核心合同和 format 的退出码均为 0；构建 0 警告/0 错误。未改 C#/XAML 或子模块源码，未重跑 GUI。最终 Git 核对 `iotembedded-final-git-20261010-a1` 退出 0，gitlink/HEAD 一致、子模块 clean、父仓无提交。详见 [设备规划验证](docs/verification-device-planning-2026-10-10.md)。本轮进程/链接/证据摘要审计位置为 `analysis/evidence/device-planning-audit-final.json`，原有失败和保留目录继续保留。

完整审计退出 0：8 份回执/22 个进程身份无仍匹配的活动进程，13 份文档/60 个本地链接无缺失，80 个路线任务显式引用有效，3 张硬件 HTML 摘要一致。审计自身已正常退出，未做删除或进程终止。

1. E0-04/05：冻结生成 BASIC/HMI API/包/profile/资源预算，确定一个板卡和可验证屏幕组合；不再等待完整 外部样本 逆向。
2. IEB-01/02/05/06/08/11：在子项目补通用应用/显示/存储/BASIC/板卡与主机平台扩展；HMI-01/02 在本仓补 VB6 式事件编辑与生成器。
3. HMI-03～06/09：同包、同生成 BASIC 的主机 LVGL 与设备运行时，先六基础控件。
4. IEB-03/04/10、HMI-07/08：输入/IO/传感器/Modbus，包下载和重启回读，完成 INT-03。
5. HMI-10、INT-04/05：OPC UA 网关、更新失败/断电恢复、协议并发和性能。此后扩展报警/配方/控件与工业样机验收。

R0-04/05/06、R5/R6/R8 仍有价值，但作为兼容支线后置；不要把新的 BASIC 布局运行等同于旧 C 宏兼容。

完整交付 ID、依赖与验收见 [ROADMAP](ROADMAP.md)。本轮授权的是需求/联网选型/路线图和子模块接入，已做完可审阅方案；当前没有设备运行时实现或硬件通过声明。真实烧录、外设写入和原设备下载仍需明确目标与操作范围。

## 资源与清理

- 分析证据、原文档提取、工具下载及历史回执归档到仓库外本地目录；旧报告中的analysis/artifacts路径按归档内相对路径读取。具体位置由本地交接提供，不将身份映射提交。
- 构建与测试日志在 `artifacts/`；测试独占临时目录已清理，失败回执保留。
- 子智能体的两次 Ghidra 诊断及七次文档命令已退出；短进程部分 CIM 字段未采到，原报告明确保留未知。最终验证进程审计见 `analysis/evidence/final-process-audit.json`。
- 早期安装临时目录 `C:\Users\mysti\AppData\Local\Temp\codex-rea-install-20261010-a340b774` 的清理被自动审批策略拒绝，未绕过；原安装回执位于 `C:\Users\mysti\.codex\install-receipts\rea-20261010-a340b774.json`。
- 两个 Ghidra 诊断独占目录 `analysis/work/native-diagnostic-runtime-noanalysis`（空）与 `analysis/work/native-diagnostic-runtime-analysis`（90 文件、33,402,801 字节）删除也被自动审批策略拒绝，仅返回 `blocked by policy`。未更换工具绕过，继续保留，见 `analysis/evidence/native-diagnostic-retention.json`。
- 不清理全局 Ghidra、JDK、REA、共享缓存或原安装。不能按进程名称杀任务；必须匹配 PID、创建时间、命令和父链。
