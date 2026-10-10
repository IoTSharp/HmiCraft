# HmiCraft · 可编程人机界面平台交接

更新：2026-10-11，Asia/Shanghai。工作区 `D:\source\HMIlStudio` 从空目录开始，已初始化 Git `main`。最新用户决定与本轮验证列于下节；历史分析和验证保留原时间边界。


## 2026-10-11 三目标修订整合（已正式合并；Node20 CI与Pages部署通过）

- 用户已确认先修订 PR #3 再正式合并 GitHub main，保留 C/LVGL、增加 LispBM/VESC 和 BASIC/IoTEmbedded；独立默认 C，HmiCraft 自有设备入口默认 BASIC，新建/设置可切换并保留每语言用户模块。不支持功能定位诊断阻止导出；首批基础 BASIC 生成及包合同，设备路线继续。
- PR 起点重新核对 head=`3647ce025bc65cd14c968c1d329dc40d593f7e51`、base=`0d0d62f655442dd5624a00390b7652222a7b3fc8`；本批以 main C 树/文档/测试为基线，引入已审查 LispBM/i18n 和三目标接口，保留原 PR head 祖先，其他通用画布变更暂不采纳。修订head和GitHub merge状态已回读；跨realm包哈希补修与revision .5已推送子仓main `d325fce517a0fd3a4939ebe8e1b325125054f139`；父仓本轮gitlink固定为 `0ef198a084032994a8e9d3788cc4c2a2c56affa0`。
- C/LVGL 的 GEN-13 已确认 generic/stm32cube-hal/rt-thread-scons profile，首批保存/重开/导出、目录/SConscript/integration-contract 合同；具体 .ioc/BSP 构建和板卡未验证。
- 公共 `src/output/index.ts`/`types.ts`/`basic.ts` 提供首批源码/包合同，`OutputBundle.deployable=false`。BASIC `hmicraft-hmi-basic/1-draft` 未在 IoTEmbedded 注册；基础控件/事件之外的图/绑定/动画/图片/字体/高级样式需诊断拒绝，runtime/API/资源完整性/设备仍独立验收。
- 仅子仓门面新增英文 README.md、README.zh-CN.md、README.ru.md 与英中 ROADMAP、AGENTS；共享 revision `2026-10-11.6`、GEN-00～13 共14任务。UI 默认英文且可切中俄，文档翻译不代表完整 UI 覆盖。父仓继续中文，D35～D38 和 LVE-11/GEN-13/知识库/分析同步。
- 文档同步脚本首次退出0：5文件/13任务/28链接。首次 whitespace 检查保留了 README 末行 CRLF 失败，已规范 LF；最终文档检查与进程状态另附本轮汇总。文档代理未提交、未推送，未安装工具或连接设备，交付文档和验证日志保留，未创建独占 MCP 服务。
- 2026-10-11 本地最终证据：子仓509通过/48项C编译跳过，实际npm run build（tsc -b+Vite）退出0；两控件/内置事件/三语言slot样例三目标ZIP/JSON导出、三语无reload切换及BASIC保存/reload恢复通过，九ZIP路径/字节/SHA256独立检查通过。父 eng/verify runId `verify-20261011-021113-dfb682` 通过。PR #3 于 `2026-10-10T18:40:28Z`（北京时间2026-10-11 02:40:28）正式合并；修订head为 `a397fa8389c481b44df47ebef37c6c555b1ef56f`，merge提交为 `9e458f8b7b7eda8898c4633535b68f73679af02e`，已fetch的origin/main与已审查树一致。GEN-11为done-remote，仅记录远端合并回执。首次合并后的Pages CI run `38076733467` 失败：506通过、3个包哈希案例失败、48项C编译跳过，原因是Node20跨realm ArrayBuffer处理；跨realm包哈希补修与revision .5文档已提交并推送子仓main `d325fce517a0fd3a4939ebe8e1b325125054f139`；Node20.20.2 Pages run `38077281534` 已completed/success，509通过/48项C编译跳过、revision .5文档同步、tsc/Vite及build/deploy两个job均成功；部署完成于 `2026-10-10T18:49:42Z`（北京时间2026-10-11 02:49:42）。该代码补修回执对应d325fce；最终纯文档提交 `0ef198a084032994a8e9d3788cc4c2a2c56affa0` 已正常推送并回读子仓main，Node20.20.2 Pages run `38077585386` 对该SHA的509项测试/48项C编译跳过、revision .6同步、tsc/Vite及build/deploy均成功，于北京时间2026-10-11 02:54:07完成；父仓gitlink固定到此可获取提交；首轮失败及本地回执均保留。GEN-09/12保留partial，小样例源码包不替代资源广样例、完整UI覆盖、固定LVGL/.ioc/BSP构建、脚本runtime/bridge与设备证明。
- 父映射状态已协调为partial；子GEN-03/13记录统一C/profile代码及本地验证，GEN-11为done-remote，仅完成远端合并回执；GEN-09/12继续partial。文档revision2026-10-11.6保持三语/双语同步；先前失败证据不覆盖。
- 最终子仓 `0ef198a084032994a8e9d3788cc4c2a2c56affa0` 已正常推送/回读main且工作区干净，父gitlink固定到该SHA；最终Node20.20.2 Pages run `38077585386` 的509项通过/48 C编译跳过、revision .6/类型/构建/部署均成功。本任务父9份文档/113个本地链接无断链，首次只读链接工具的自动变量错误回执保留。进程审计85份回执/265身份、匹配存活0；4个复用PID未动，五份其他会话文档SHA256未变并保留未提交状态。证据见 `artifacts/process/pr3-final-resource-audit-20261011.json` 与最终CI日志；完整边界见主整合验证报告。

## 2026-10-10 决定：统一编辑器与三目标（历史）

- 用户确认 C/LVGL、LispBM/VESC、BASIC/IoTEmbedded 三条正式输出主线，并明确可视化和三种输出都归 LvglEditor；HmiCraft 初始化时配置输出目标/profile并消费结果。D32～D34、OUT-01～06 与 [职责知识](docs/knowledge/designer-and-targets.md) 已登记，取代早期 LispBM 可选插件及父仓拥有生成器的规划。
- 本轮起点为父仓 `ff81ef0`，工作区干净，`main...origin/main` 无本地差异。`external/LvglEditor` 已在索引中以 mode 160000 固定为 `0d0d62f655442dd5624a00390b7652222a7b3fc8`，子模块 HEAD 一致且工作区干净；沿用既有接入，没有重复添加、更新 SHA 或改动子仓源码。
- 子仓新增 [ROADMAP](external/LvglEditor/ROADMAP.md) 的 GEN-00～10，定义初始化配置、默认/允许目标、版本化 profile、持久化冲突处理、三生成器、资源打包、用户代码与分目标验证；子仓 README/CHANGELOG 同步。本轮仅本地文档修改，子模块 HEAD/gitlink 仍在上述 SHA，新增文档尚未包含在固定提交中。
- LvglEditor 负责公共模型/三后端/资源输出；HmiCraft 负责宿主初始化、旧工程适配、消费结果和设备交付；IoTEmbedded 负责 BASIC 通用底层。现有 Avalonia/.hmistudio.json 保留兼容与回归基线，接口尚待实现。
- 实施顺序为子仓 GEN-00 → GEN-01/02 → GEN-03/04/05，并由 GEN-06/07 支撑；GEN-10 验证宿主接口。父仓 LVE-05/OUT-01 接入，OUT-02～06 映射各目标闭环。HMI-01/02/03 实现归子仓，HMI runtime/部署归父仓。
- 本轮仅同步父子仓规划，没有实现初始化接口、合并 PR、设备操作、提交或推送。历史 Web 类型/字体失败仍未修复；.NET 验证 `verify-20261010-233929-4c273d` 的 restore/Release build/console合同/format 均退出0，0警告/0错误、11/11合同通过，测试独占目录已清理；随后仅继续修改文档，未重复 GUI 或 Web 测试。日志在 `artifacts/process/verify-20261010-233929-4c273d-*`。
- 最终文档检查覆盖16份文件、111个本地链接，新增断链0；首次检查发现子仓 README 的 `./LICENSE` 缺文件，已从固定 HEAD 确认是既有问题，保留 LVE-02/GOV-04。GEN-00～10 共11个唯一任务，父子仓 whitespace 检查均退出0。
- 四份验证进程回执记录的16个身份均无仍匹配进程；短 smoke 未生成 owned-processes 清单，但启动回执 PID 已不存在。测试临时目录已回收，本轮未安装工具或启动服务，构建/验证日志保留。

## HmiCraft 知识、在线版与首次仓库交付

- 中文名“可编程人机界面平台”，英文名**HmiCraft**；IoTSharp/HmiCraft已创建并推送main，用户最新明确**保持公开**。原始根提交已匿名化重写、带明确lease强推成功，可达历史扫描零命中，旧本地提交对象已清理；最终SHA以本轮回读为准。
- 必要资料及历史回执已完整移动到仓库之外，原analysis/artifacts目录不再位于工作区；同盘目录移动，未删除研究文件。58项文件/gitlink、30份Markdown、129个本地链接和106个路线任务的内容审计通过；移出前87个已记录进程身份均无仍匹配进程。
- 研究范围覆盖多类商业与开源产品；仓库保留匿名结论、需求和设计选择，实际依赖名称与许可准确保留。详细来源和历史回执只留仓库外本地目录，规则见 [研究规范](docs/knowledge/research-and-attribution.md)。
- `docs/knowledge/` 已保存名称、技术/设备分工、开源与商业边界、开发者与OEM客户、自售设备、第三方OEM、定制成果归属。GOV/BIZ/LVE任务和 ADR 0003 记录归属、依赖与验收。许可仍为 MIT/MPL-2.0 方案，没有新增根 LICENSE。
- 用户新增 lvgl-editor 在线方向；推荐 Web 主入口、本地工程、可选桌面/本地桥，复用同一 UI；保留 .NET 核心及已有 Avalonia 切片。BASIC 布局、用户脚本分离、IoTEmbedded/LVGL 设备分工继续有效。
- `external/LvglEditor` 已作为第二个真实 Git 子模块固定到 `0d0d62f655442dd5624a00390b7652222a7b3fc8`，源码未改。上游 README 声明 MIT，但缺正式许可文件；LVE-02/GOV-04 在公开分发前处理。
- PR #2（`d8a0efe19eddcb58b4c4b7bd6a7ad65361862754`）是 OmniHost 桌面草稿，macOS 未实现，资源路径、私有包源和 .NET 版本需验证。PR #3（`3647ce025bc65cd14c968c1d329dc40d593f7e51`）大幅转向 LispBM/VESC；通用画布/资源/状态/i18n 改进逐项选择，不整体替代 BASIC。**2026-10-10该次基线核查尚未合并或采纳两个PR的代码，未运行它们的构建；2026-10-11修订PR #3已正式合并，见顶部新回执。**
- Web main 验证：安装依赖退出0；`tsc -b` 退出2；Vitest 390项中386通过/4个字体断言失败（退出1）；静态 Vite bundle退出0。特定 Linux/Emscripten 的 `compile.test.ts` 未运行；2026-10-10当时未执行浏览器GUI smoke、离线缓存或设备下载；2026-10-11小样例浏览器旅程已通过，离线缓存/设备与完整覆盖仍未验，Web继续partial。
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

最终细节见[子仓验证报告](external/LvglEditor/docs/verification-pr3-2026-10-11.md)与[共同样例](external/LvglEditor/samples/three-targets.lvgl.json)。

本轮源码、浏览器、正式合并、CI与资源状态见[主整合验证报告](docs/verification-pr3-2026-10-11.md)。
