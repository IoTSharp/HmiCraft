# lvgl-editor 在线版与 PR 整合分析

初始核查日期：2026-10-10。用户要求评估在线版，整合 `IoTSharp/lvgl-editor` 和两个开放PR。该次仅接入固定main并复核关键差异，当时未合并PR；后续2026-10-11修订PR #3的正式合并、三目标与浏览器验证见下节，不把历史静态分析当运行证明。

后续用户决定 D32～D34：可视化与三种输出都由已接入的 LvglEditor 承担，HmiCraft 初始化编辑器时指定所需目标/profile并消费结果。以下固定源码/PR/测试记录保留原核查时间；实施在子仓 [GEN 路线](../../external/LvglEditor/ROADMAP.md) 维护，父仓 OUT 追踪集成；职责见 [统一设计器与三目标](../knowledge/designer-and-targets.md)。

## 2026-10-11 修订合并与三目标实施

用户后续确认 D35～D38：修订 PR #3 后正式合并 GitHub main，保留 C 后端/测试并加入 BASIC；新建和设置可切目标，独立默认 C、HmiCraft 自有入口默认 BASIC，按语言保留手写逻辑。PR #3 本轮起点重新确认OPEN，head=`3647ce025bc65cd14c968c1d329dc40d593f7e51`、base=`0d0d62f655442dd5624a00390b7652222a7b3fc8`。这是本轮起点；修订head `a397fa8389c481b44df47ebef37c6c555b1ef56f` 已正式合并为 `9e458f8b7b7eda8898c4633535b68f73679af02e`，合并时间为北京时间2026-10-11 02:40:28；包哈希补修与revision .5已推送子仓main `d325fce517a0fd3a4939ebe8e1b325125054f139`；父仓本轮gitlink固定为 `0ef198a084032994a8e9d3788cc4c2a2c56affa0`。

本批以 main 的 C 源码/文档/测试为整合树基线，引入已审查 LispBM/i18n 源及三目标接口；保留 PR #3 原 head 祖先供来源追溯，其他大幅画布改动暂不采纳。BASIC 先基础布局/事件和包合同，HMI API 是 draft，`deployable=false`；不支持的图/绑定/动画/图片/字体/高级样式诊断阻止导出，runtime/单板另行推进。

C 后端统一接公共输出接口，新增 CubeMX/CubeIDE HAL、RT-Thread 标准 BSP/SCons profile 的保存/目录/适配合同，默认 generic 保留旧工程；具体 .ioc/BSP 构建与板卡未验证。BASIC 初步 source map 和 C/Lisp 空映射分别记录。

子仓维护英文门面、三语 README、英中路线与 AGENTS；UI 默认英文可切中俄，完整界面覆盖独立验证；父仓继续中文。见 [GEN-00～12 中文路线](../../external/LvglEditor/ROADMAP.zh-CN.md)。以下 2026-10-10 差异/失败保留原结果。
## 2026-10-10 固定基线与实际接入（历史）

| 对象 | 固定提交 | 事实 |
|---|---|---|
| 上游 main | `0d0d62f655442dd5624a00390b7652222a7b3fc8` | 2026-03-30，已作为 `external/LvglEditor` Git 子模块接入 HmiCraft |
| PR #2 | `d8a0efe19eddcb58b4c4b7bd6a7ad65361862754` | 草稿，5 个提交、13 个变更文件；OmniHost 桌面外壳 |
| PR #3 | `3647ce025bc65cd14c968c1d329dc40d593f7e51` | 非草稿，58 个提交、341 个变更文件；LispBM/VESC 专用方向的大幅修改 |

两个 PR 均基于上述 main。GitHub 当次返回 mergeable=true 只表示当时可做 Git 合并，不代表产品方向、许可、构建或行为通过。PR 状态是时间快照，采纳前应再次核对 head SHA。

私有仓库的初始接入仅固定 main 源码。完整 PR 历史头在根 `analysis/work/lvgl-editor-baseline` 只读复核，不进入父仓；父仓中的子模块不包含未提交修改。

## 可复用能力和差异

| 领域 | 已读源码 | 整合判断 |
|---|---|---|
| Web 工作台 | React/TypeScript、Zustand、Monaco、React Flow、页面与属性/事件 UI | 优先复用现有 Web 编辑能力，避免重新开发全部画布；不据 README 宣称生产就认定稳定 |
| 工程模型 | `src/types/index.ts` 包含稳定 ID、嵌套控件、样式状态、事件、动画；`projectStore.ts` 用 IndexedDB | 与当前 C# JSON 不是同一格式，需版本化公共模型与显式映射，不做直接字段拼接 |
| 逻辑图 | 触发/条件/动作/数据节点、执行/数据连接、自定义 `c_code_block` | 基础节点可映射 BASIC 事件；C 代码块不自动转 BASIC，保留并诊断未支持能力 |
| 资源 | Base64 图片/字体、像素格式、字形范围、C 数组名称 | 可复用资源编辑概念，发布时转换为 HmiCraft 资源索引/文件；不直接把全部 Base64 JSON 放 MCU |
| 生成器 | 生成 `ui.c`、事件/逻辑 C 与头文件；配置可选 LVGL 8/9 | C 后端作为正式输出继续维护；增加 LispBM/VESC 与 BASIC/IoTEmbedded 后端，能力/version/source map 采用共同合同 |
| LVGL 版本 | README 仍描述 v8 限制，但 projectStore 与 codegen 默认 v9 | 文档漂移，不能按 README 选设备 ABI；实际生成代码还需编译验证 |
| 预览 | Canvas、预编译 WebAssembly 与服务端 C 编译入口并存 | 视觉预览不等于同一 BASIC runtime；三类证据分开，未来对齐同包和事件轨迹 |
| 编译服务 | Vite 插件有 `/api/compile` 与固定 Linux Emscripten 路径 | 开发辅助代码不能直接当公网服务；首个静态 Web 构建关闭此功能，服务化另定隔离/资源/访问合同 |
| 测试 | 存在 Vitest 测试目录，与 README“尚无测试”说法不符 | 以实际 runner 为准；C 编译集成测试硬编码其他机器 `/home/xcssa/...` 路径，本机未具备其环境 |

关键来源均在固定子模块：[工程模型](../../external/LvglEditor/src/types/index.ts)、[工程存储](../../external/LvglEditor/src/store/projectStore.ts)、[逻辑图](../../external/LvglEditor/src/components/LogicEditor/types.ts)、[生成器](../../external/LvglEditor/src/codegen/generator.ts)、[资源](../../external/LvglEditor/src/resources/types.ts)、[Vite 配置](../../external/LvglEditor/vite.config.ts)、[编译服务](../../external/LvglEditor/vite-plugin-compile.ts)。

## 2026-10-10 两个 PR 的采纳建议（历史）

### PR #2：桌面外壳，独立后置

PR 地址：`https://github.com/IoTSharp/lvgl-editor/pull/2`。采用 .NET 8 的 OmniHost，新增菜单和桌面打包。实际路径为 `desktop/Program.cs`、`desktop/LvglEditor.Desktop.csproj`，不是 PR 文字描述中的另一层目录。

关键缺口：macOS 路径直接抛 `PlatformNotSupportedException`；项目静态资源 `../../dist` 相对于 `desktop/` 的路径需要构建核对；依赖私有包源凭据；未配置凭据时 workflow 会成功跳过，不代表产物可运行。当前 HmiCraft 为 .NET 10，若采用还需目标框架和平台依赖决策。

建议把菜单接线和本地宿主作为独立选择性任务；先证明窗口、资源路径、文件访问和设备桥，再谈三平台发布。不能整体采纳草稿后宣称桌面跨平台完成。本轮未安装 OmniHost 包或执行该 PR。

### PR #3：提取通用改进，保留独立目标后端

PR 地址：`https://github.com/IoTSharp/lvgl-editor/pull/3`。它把生成器改为 LispBM，目标为外部 VESC Express P4 Dashboard 项目；还改动主题、双语 UI、画布结构、资源转换和 WASM，并删除原 C 生成器、许多文档和原生成器测试。

建议分成三组处理：

1. **待逐项采纳的通用改进**：画布交互、状态拆分、资源编辑、主题/本地化基础，先明确修复对象与回归语料。
2. **正式目标后端**：LispBM/VESC 生成与资源格式按 OUT-03 独立集成和测试；与 C/LVGL、BASIC/IoTEmbedded 并列，保留各自运行与交付合同。
3. **不作为 HmiCraft 默认的变更**：覆盖公共品牌、取消中文默认、整体删除原文档/C 后端测试、将普通打包绕开 TypeScript 检查。保留可追溯材料，再按最终产品决定迁移。

PR README 自述推荐 `build:web` 绕过现有 TypeScript 问题；这不是完整构建通过证据。2026-10-10该次仅作关键结构/目标差异复核，尚未逐行审计341文件，也未执行PR #3的tests/build；后续整合树验证与该原始PR头分开记录。

## 在线版定位建议

建议以 **Web 为主要编辑入口，桌面/本地桥作为离线与设备访问补充**。复用一套 UI 和工程合同，不同时重写两套完整画布；保留 .NET 在公共模型、校验、资源/包工具、网关和本地集成中的价值。原 Avalonia 切片保留，不在路线调整时删除现有工程读取能力。

在线版不等于强制云端保存或持续联网。第一阶段用静态站点加浏览器本地工程存储/导入导出；协作、账户、云存储、服务器编译和设备运维是独立能力。现有 IndexedDB 存储也不等于已完成离线安装/缓存，需要单独验证断网启动、资源和工程恢复。

不能默认网页可以无条件访问所有串口/USB/网络设备。LVE-07 用实际目标浏览器和设备验证权限/传输，并在需要时提供有限权限的本地桥；不把普通网页访问转为任意本地命令执行。

共同链路为：HmiCraft 等宿主初始化输出配置 → LvglEditor 的工作台/公共模型/校验/三生成器 → 产物交给宿主/集成方运行与交付。BASIC 路径继续输出资源包到同一 BASIC 设备 runtime；C 与 LispBM 使用各自的编译/脚本和设备适配。图形/事件/资源固定向量逐目标验证；C# 与 TypeScript 不要求互相嵌入同一程序集。

```mermaid
flowchart LR
    Host[HmiCraft 初始化目标 / profile] --> Web
    subgraph Editor[LvglEditor 拥有可视化和三种输出]
    Web[Web 编辑器 / 浏览器本地工程] --> Model[版本化公共模型与能力校验]
    Desktop[可选桌面宿主 / 同一 Web UI] --> Model
    Model --> CTarget[C / LVGL 源码与资源]
    Model --> LispTarget[LispBM / VESC 脚本与资源]
    Model --> Generator[BASIC 生成器 / 用户脚本 / 资源处理]
    Generator --> Package[独立设计包]
    end
    CTarget --> CIntegration[目标固件编译集成]
    LispTarget --> VescIntegration[VESC 桥接固件与部署]
    Package --> Preview[同一 C BASIC + LVGL 主机预览]
    Package --> Transfer[受限下载接口 / 本地桥]
    Transfer --> Device[IoTEmbedded + HmiCraft Runtime + LVGL]
```

本图是目标架构。现有 C# JSON 与上游 Web JSON 尚未互通，当前 Web WASM 预览不是图中的同一 BASIC runtime。

### 实施批次与取舍

| 批次 | 交付 | 完成依据 |
|---|---|---|
| W0 / LVE-08 | 清理固定 main 的类型和字体回归，参数化 C 编译测试环境 | 保留初始失败；类型检查、单元测试、目标 C 编译各有独立结果；类型检查不能从正式 build 中删除 |
| W1 / LVE-04 | 浏览器新建、编辑、本地保存、导入导出；随后离线缓存与恢复 | 浏览器旅程及关页/断网/数据恢复证据；IndexedDB 不是备份，明确工程导出/恢复入口 |
| W2 / LVE-05、OUT-01～06 | LvglEditor 按 GEN 实现初始化接口/三后端；HmiCraft 消费产物 | 六基础控件先行；未知控件/代码节点明确拒绝或保留，不静默丢失；初始化请求与产物目标一致，每目标分别验证 |
| W3 / LVE-06/07 | 同 BASIC/LVGL 预览、目标板包下载及回读 | 区分浏览器画布、主机 runtime 和实板；日常包更新不改变基础固件 SHA |
| 可并行评估 / LVE-09 | PR #3 的画布/资源/状态/i18n 改进 | 按依赖拆补丁，每项有明确收益、测试、来源及回退提交；不能只挑一段代码遗漏依赖 |
| 按需后置 / LVE-10 | PR #2 的菜单和桌面宿主 | 菜单与宿主分别验收；Windows 先闭环，Linux/macOS 各自验收，不能沿用成功跳过的 CI |

PR 采纳台账需记录来源 PR/head、涉及文件、依赖补丁、保留/替换行为、许可来源、前后验证与采纳提交。2026-10-10该次仅有关键结构评估和固定main接入，未产生PR采纳提交；2026-10-11已完成W0本地修复及审查后PR #3修订合并，保留C和加入BASIC。其他通用补丁、PR #2及目标运行/设备继续按独立任务推进。

## 2026-10-10 许可、构建与证据边界（历史）

固定 main 的 README 声明 MIT，但 GitHub license 元数据为 null，完整 Git 树未找到 LICENSE/LICENCE/COPYING/NOTICE 文件。不能仅凭 README 徽章对整仓和所有资源做 MIT 分发声明。已纳入 LVE-02/GOV-04；本次仅接入私有集成仓库的固定依赖，未添加许可或公开发布。

Node/npm 依赖安装采用固定 lockfile，`npm ci --ignore-scripts --no-audit --no-fund`；没有执行项目安装脚本。测试关闭远程 C 编译预览，并将依赖固定 Emscripten 机器路径的 `compile.test.ts` 单独列为未运行，其他测试与 TypeScript/build 分别记录。没有扫描或安装未知编译器。

实际结果：`tsc -b` 退出 2，既有问题包含 `GeneratedCode` 类型契约、缺少 `LvglComponent` 导入、新工程缺 `useBuiltinSymbols` 和未使用声明。Vitest 已执行 11 个文件、390 项，386 项通过、4 项字体相关断言失败；这些差异需要先确认字体契约，不能只改断言让测试通过。关闭编译预览的 Vite 静态 bundle 退出 0，仅证明转译和打包成功。

回执名称在 `artifacts/process/`：`lvgl-editor-*` 是源码/PR 快照，`hmicraft-lvgl-submodule-*` 是接入，`hmicraft-web-*` 是实际 Web 检查。该次2026-10-10结果见 [仓库和在线基线验证](../verification-hmicraft-bootstrap-2026-10-10.md)，当时没有浏览器GUI、设备或PR通过声明；后续小样例浏览器与正式合并证据见2026-10-11回执。

## 2026-10-11 当前实施验证回执

2026-10-11 本地最终证据：子仓509通过/48项C编译跳过，实际npm run build（tsc -b+Vite）退出0；两控件/内置事件/三语言slot样例三目标ZIP/JSON导出、三语无reload切换及BASIC保存/reload恢复通过，九ZIP路径/字节/SHA256独立检查通过。父 eng/verify runId `verify-20261011-021113-dfb682` 通过。PR #3 于 `2026-10-10T18:40:28Z`（北京时间2026-10-11 02:40:28）正式合并；修订head为 `a397fa8389c481b44df47ebef37c6c555b1ef56f`，merge提交为 `9e458f8b7b7eda8898c4633535b68f73679af02e`，已fetch的origin/main与已审查树一致。GEN-11为done-remote，仅记录远端合并回执。首次合并后的Pages CI run `38076733467` 失败：506通过、3个包哈希案例失败、48项C编译跳过，原因是Node20跨realm ArrayBuffer处理；跨realm包哈希补修与revision .5文档已提交并推送子仓main `d325fce517a0fd3a4939ebe8e1b325125054f139`；Node20.20.2 Pages run `38077281534` 已completed/success，509通过/48项C编译跳过、revision .5文档同步、tsc/Vite及build/deploy两个job均成功；部署完成于 `2026-10-10T18:49:42Z`（北京时间2026-10-11 02:49:42）。该代码补修回执对应d325fce；最终纯文档提交 `0ef198a084032994a8e9d3788cc4c2a2c56affa0` 已正常推送并回读子仓main，Node20.20.2 Pages run `38077585386` 对该SHA的509项测试/48项C编译跳过、revision .6同步、tsc/Vite及build/deploy均成功，于北京时间2026-10-11 02:54:07完成；父仓gitlink固定到此可获取提交；首轮失败及本地回执均保留。GEN-09/12保留partial，小样例源码包不替代资源广样例、完整UI覆盖、固定LVGL/.ioc/BSP构建、脚本runtime/bridge与设备证明。
本地通过未改变历史2026-10-10固定main失败；真实浏览器C产物下载不等同固定LVGL/目标工具链编译，三语切换/保存恢复和小样例导出已验证，完整UI覆盖及资源广样例仍未验；正式合并和补修结果已分别记录，Node20.20.2 CI/Pages部署已通过，最终文档提交 `0ef198a084032994a8e9d3788cc4c2a2c56affa0` 的Pages run `38077585386` 已成功，父仓固定到该SHA。

最终细节见[子仓验证报告](../../external/LvglEditor/docs/verification-pr3-2026-10-11.md)与[共同样例](../../external/LvglEditor/samples/three-targets.lvgl.json)。

本轮源码、浏览器、正式合并、CI与资源状态见[主整合验证报告](../verification-pr3-2026-10-11.md)。
