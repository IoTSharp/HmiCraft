# 统一可视化设计器与三条输出主线

确认日期：2026-10-10；最新补充：2026-10-11。来源：用户明确提出三条输出主线、lvgl-editor 子模块统一承担可视化，并进一步确认 **三种输出也归 LvglEditor；HmiCraft 初始化时选择所需目标**。以此替代此前把三后端归父仓的工程建议。用户随后确认修订 PR #3 后正式合并、新建/设置可切目标、基础 BASIC 布局/事件及包合同先行。本文记录职责和验收边界，设备链路未验收。

## 已确认方向与当前事实

- HmiCraft 的可视化设计统一基于 `external/LvglEditor`，页面、画布、控件树、布局、属性、样式、资源和事件/逻辑编辑在这套工作台中演进。
- `external/LvglEditor` 已是真实 Git 子模块，来源 `IoTSharp/lvgl-editor`，原固定基线为 `0d0d62f655442dd5624a00390b7652222a7b3fc8`。2026-10-11修订PR #3已正式合并为 `9e458f8b7b7eda8898c4633535b68f73679af02e`；包哈希补修与revision .5已推送子仓main `d325fce517a0fd3a4939ebe8e1b325125054f139`，父仓本轮gitlink固定为 `0ef198a084032994a8e9d3788cc4c2a2c56affa0`。
- C/LVGL、LispBM/VESC、BASIC/IoTEmbedded 都是正式规划主线，各自安排里程碑与验收。BASIC/IoTEmbedded 继续承担自有工业 HMI 设备路线。
- 三种生成器、输出配置、资源转换/打包、目标校验和编辑器公共模型在 LvglEditor 维护，权威实施任务见 [子模块 ROADMAP](../../external/LvglEditor/ROADMAP.md)。独立默认 C，HmiCraft 自有设备入口默认 BASIC；新建/设置使用同一目标合同。
- 现有 Avalonia 切片与 `.hmistudio.json` 保留为兼容和回归基线；后续可视化功能在 lvgl-editor 工作台实现。桌面入口规划为同一 Web UI 的宿主，.NET 继续承担适合它的校验、工具、网关和本地集成。
- 当前已有固定Web源码、统一三目标接口及首批C/LispBM/BASIC源码包；完整模型映射、两条脚本runtime交付和设备链路仍未完成。历史Web类型/字体失败与首轮合并CI失败保留，不以源码整合替代完整验收。

## 仓库和维护职责

| 归属 | 负责内容 | 交付边界 |
|---|---|---|
| `external/LvglEditor` / IoTSharp/lvgl-editor | 可视化/工程读写、公共模型、初始化目标配置、三生成后端、资源转换/打包、目标校验和 source map | GEN 路线在子仓维护；独立运行，不能依赖 HmiCraft 源码或在线服务；改动在子仓独立验证/提交，父仓固定可获取 SHA |
| HmiCraft 父仓 | 初始化编辑器时指定输出目标/允许范围，提供产品设备与 runtime API/profile，适配旧工程，消费生成产物并完成传输/调试/设备集成 | 宿主使用 LvglEditor 的生成入口和结果；产品 UI 通过约定扩展接入，不在父仓再实现三种生成器；宿主接口尚待实现 |
| `external/IoTEmbedded` / IoTSharp/IoTEmbedded | BASIC 目标所需通用 MCU/RTOS/BSP、驱动、协议、解释器与存储 | 不依赖 HmiCraft 工程格式，也不成为 C/VESC 两目标的强制依赖 |
| 本仓 `firmware/` | BASIC/IoTEmbedded 的 LVGL 控件封装、HMI API、包/资源、事件/变量/IO 绑定与板卡产品集成 | 普通设计变化更新脚本/资源包；原生能力变化按固件升级处理 |
| 各目标集成维护者 | C 的固件/BSP 接口；VESC 的 LispBM/LVGL 桥接与资源/下载适配 | 各自固定版本、设备 profile、构建/运行和设备证明，不与 BASIC 验收相互替代 |

LvglEditor 内部目录和初始化/扩展 API 由 GEN-01/02/10 确定，父仓 OUT-01/LVE-05 对接这些任务；本批实现、类型/测试/构建和设备证据分别记录。

## 公共模型与输出合同

目标结构：HmiCraft 初始化并传入输出配置 → LvglEditor 工作台/公共模型/目标校验 → LvglEditor 内部三个生成后端 → HmiCraft 消费产物并对接设备。公共模型保存稳定控件 ID、页面、布局、样式、资源、变量、基础动作与事件；生成语言不作为唯一工程来源。

初始化配置草案见子仓路线：目标 ID 为 `c-lvgl`、`lispbm-vesc`、`basic-iotembedded`，宿主给出默认目标、允许范围、是否允许切换和版本化 profile。旧的未配置入口保留 C；显式请求未知/未实现/不匹配目标必须定位诊断并阻止导出。工程重开检查保存目标与宿主限制，不自动覆盖目标或用户代码。新建与项目设置可选择目标，独立默认 C、HmiCraft 自有入口默认 BASIC；实际宿主挂载仍由 GEN-10/LVE-05 验证。

| 目标 | 计划产物 | 运行/更新方式 | 独立验收 |
|---|---|---|---|
| C/LVGL | C/头文件、资源及集成清单 | 由目标工具链编译并集成进固件；资源能否独立更新由该 profile 声明 | 固定 LVGL/BSP 编译、主机事件/资源验证、指定设备运行 |
| LispBM/VESC | Lisp 布局/事件代码、资源及目标清单 | 在具备匹配 LispBM/LVGL 接口的固件上加载；并非任意 VESC 固件都支持 | 固定解释器/桥接/资源格式、脚本执行及指定设备上传/回读 |
| BASIC/IoTEmbedded | BASIC 布局、用户事件脚本、资源及设计包 | 由 IoTEmbedded + HmiCraft HMI runtime 加载；日常更新包 | 包校验、预算/取消/事件、主机 runtime、指定板卡更新及恢复 |

1. 目标配置分别固定 LVGL、runtime/API、资源格式、内存/外设能力和板卡版本；导出前对不支持的控件/动作明确诊断。
2. 基础动作可以逐目标生成；手写 C、Lisp、BASIC 代码按目标保存，不承诺自动互译。切换目标保留全部用户逻辑，报告缺少的目标实现。
3. 生成代码与用户代码分离，重生成/重命名不能覆盖用户逻辑；稳定 ID 和 source map 关联错误。
4. 现有 Web JSON 与 `.hmistudio.json` 通过显式版本映射迁移；未知项拒绝或无损保留，不静默丢失，不未经决定改扩展名。
5. 浏览器视觉预览、生成/编译、目标脚本执行及真实设备是不同证明；共同样例逐目标记录，不宣称三者天然等价。

## 实施与 PR 采纳

- LVE-08 先关闭既有 Web 基线问题；LVE-04 验证统一工作台的编辑、保存、重开与导出。
- LvglEditor GEN-01/02/10 定义公共模型、初始化配置和宿主合同；父仓 OUT-01/LVE-05 接入。OUT-02/03/04 映射 GEN-03/04/05 三生成后端及各自运行验收，不在父仓重复实现。
- PR #3 的通用编辑能力归 LVE-09；LispBM 生成/资源部分在子仓 GEN-04 实现，父仓 OUT-03 跟踪集成，并保留 C 后端和 BASIC 接口。2026-10-11已修订并正式合并，GEN-11为done-remote，仅对应已核实的GitHub合并回执；首轮Pages CI失败已补修d325fce，Node20.20.2 run38077281534源码测试/构建/Pages部署成功，目标runtime与设备仍未验。
- OUT-05 负责宿主旧工程映射，对接 GEN-02/07 的用户代码保护；OUT-06 对接 GEN-09 的共同语料和设备验收。
- PR #2 的桌面宿主继续按 LVE-10 验证，复用同一工作台；其支持平台不由本决策推定。

编辑器实施见 [LvglEditor ROADMAP](../../external/LvglEditor/ROADMAP.md)，宿主/设备集成见 [HmiCraft ROADMAP](../../ROADMAP.md)，确认来源见 [D32～D38](decisions.md)，固定基线和历史失败见 [整合分析](../analysis/lvgl-editor-integration.md)。

## 文档、语言与首批 BASIC 边界

- 仅 LvglEditor 门面为英文 README.md，附 README.zh-CN.md/README.ru.md，路线为英文/中文；共享 revision、任务 ID/状态和能力限制，运行同步检查。HmiCraft 父仓继续中文。
- UI 默认英文，可切换中文和俄语；目标新建/设置、诊断、导出和语言持久化单独验证，README 翻译不等于完整 UI 国际化。子仓 AGENTS 固化约束，任务 GEN-12/LVE-11。
- 公共入口 `src/output/index.ts` 提供 `generateTargetSource(input)` 同步源码预览、`generateTargetProject(input)` 异步资源/摘要/清单生成；`OutputBundle` 带 target/files/issues/manifest/sourceMap，`deployable=false`，仍是源码合同产物。
- BASIC 首批支持容器和六基础控件（obj/btn/label/slider/bar/switch/checkbox）的绝对像素布局、基本 default 样式和 navigate/show/hide/enable/disable/setText/setValue；用户事件代码按目标保存。图、绑定、动画、图片/字体及高级样式不支持时拒绝导出。
- 生成 `main.bas`、`generated/layout.bas`、`generated/events.bas`、`user/events.bas`、`api-contract.json`、`source-map.json`、`manifest.json`；`hmicraft-hmi-basic/1-draft` 尚未在 IoTEmbedded 注册，nested import 仍需未来 HmiCraft 包加载器。runtime/API 消费和板卡继续后续路线。
- D38：原 C 统一 backend，c-lvgl 内的 cIntegrationProfile 默认 generic，可选 stm32cube-hal（CubeMX/CubeIDE HAL）与 rt-thread-scons（标准 BSP/SCons），保存/重开/导出保留；首批目录与 integration-contract.json，不含具体 .ioc/BSP 构建或板卡证明。BASIC 有首批 source map，C/Lisp 当前为空，不能说三目标定位映射已完成。

## 2026-10-11 实施证据更新

2026-10-11 本地最终证据：子仓509通过/48项C编译跳过，实际npm run build（tsc -b+Vite）退出0；两控件/内置事件/三语言slot样例三目标ZIP/JSON导出、三语无reload切换及BASIC保存/reload恢复通过，九ZIP路径/字节/SHA256独立检查通过。父 eng/verify runId `verify-20261011-021113-dfb682` 通过。PR #3 于 `2026-10-10T18:40:28Z`（北京时间2026-10-11 02:40:28）正式合并；修订head为 `a397fa8389c481b44df47ebef37c6c555b1ef56f`，merge提交为 `9e458f8b7b7eda8898c4633535b68f73679af02e`，已fetch的origin/main与已审查树一致。GEN-11为done-remote，仅记录远端合并回执。首次合并后的Pages CI run `38076733467` 失败：506通过、3个包哈希案例失败、48项C编译跳过，原因是Node20跨realm ArrayBuffer处理；跨realm包哈希补修与revision .5文档已提交并推送子仓main `d325fce517a0fd3a4939ebe8e1b325125054f139`；Node20.20.2 Pages run `38077281534` 已completed/success，509通过/48项C编译跳过、revision .5文档同步、tsc/Vite及build/deploy两个job均成功；部署完成于 `2026-10-10T18:49:42Z`（北京时间2026-10-11 02:49:42）。该代码补修回执对应d325fce；最终纯文档提交 `0ef198a084032994a8e9d3788cc4c2a2c56affa0` 已正常推送并回读子仓main，Node20.20.2 Pages run `38077585386` 对该SHA的509项测试/48项C编译跳过、revision .6同步、tsc/Vite及build/deploy均成功，于北京时间2026-10-11 02:54:07完成；父仓gitlink固定到此可获取提交；首轮失败及本地回执均保留。GEN-09/12保留partial，小样例源码包不替代资源广样例、完整UI覆盖、固定LVGL/.ioc/BSP构建、脚本runtime/bridge与设备证明。
C统一backend/profile持久化与导出目录合同已实现；Lisp/BASIC基础生成合同已通过本地测试，但宿主消费/脚本执行/设备与完整资源能力继续对应OUT/HMI门禁。

最终细节见[子仓验证报告](../../external/LvglEditor/docs/verification-pr3-2026-10-11.md)与[共同样例](../../external/LvglEditor/samples/three-targets.lvgl.json)。

本轮源码、浏览器、正式合并、CI与资源状态见[主整合验证报告](../verification-pr3-2026-10-11.md)。
