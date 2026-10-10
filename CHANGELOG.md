# Changelog

## Unreleased

### Added（2026-10-10 历史基线）

- 商业样本 A 安装清单、REA artifact 证据、帮助功能矩阵、设备和宏契约报告。
- .NET 10 / Avalonia 桌面重写方案及按证据分层的路线图。
- 独立 JSON 工程模型、版本与输入校验、受限文件读写、临时文件替换保存。
- 单画面基础控件编辑、拖动、属性面板、撤销/重做、离线变量与按钮/指示灯/数值联动。
- 内核合同测试、桌面 smoke、进程和时间有界的验证工具。
- 初始化本地 Git，按固定提交引入 `external/IoTEmbedded` 子模块；保留上游源码原状。
- IoTEmbedded 源码能力基线、设备执行层职责、VB6 式 BASIC 生成与事件方案、工业硬件联网选型及 E0/IEB/HMI/INT 路线。
- 仓库内知识库、决策登记、开源/闭源边界、设备/OEM/定制业务规则，以及 GOV/BIZ 路线的归属、依赖和验收。
- 在线编辑器 `external/LvglEditor` 固定子模块、两个开放 PR 的关键差异研究及 LVE 整合路线；未整体合并上游 PR。
- Web 基线实测及 LVE-08～10 分批任务：保留类型检查失败、4 项字体测试失败和静态打包成功的不同证据；BASIC 接入和浏览器旅程仍待实现/验证。
- 创建并推送 `IoTSharp/HmiCraft` 私有仓库，核对默认分支、提交SHA和两上游子模块提交可获取性。

### 2026-10-11 三目标修订整合

- 已按用户要求修订并正式合并PR #3为 `9e458f8b7b7eda8898c4633535b68f73679af02e`；保留C/LVGL、采纳LispBM/VESC，首批基础BASIC布局/事件、目标选择及包合同已有源码和小样例浏览器证据，`deployable=false`；跨realm包哈希补修及revision .5已推送 `d325fce517a0fd3a4939ebe8e1b325125054f139`，CI/部署与父仓gitlink分别记录。
- 新建/项目设置统一目标选择，独立默认 C、HmiCraft 自有入口默认 BASIC；按语言保护用户代码，不支持项诊断阻止导出。runtime、资源转换和单板继续既定设备路线。
- 子仓英文门面、英中俄 README、英中路线和 AGENTS，revision/任务状态同步检查；UI 默认英文可切中俄，界面验收独立。父仓保留中文，D35～D38 与 LVE-11/GEN-13 同步；原失败及历史未合并记录保留其时间边界。

- 新增 C/LVGL 下 generic/stm32cube-hal/rt-thread-scons profile 的持久化/目录/适配合同路线，原 C 统一 backend；具体 .ioc/BSP/工具链构建和板卡仍待验收。

### Changed（2026-10-10 历史规划与治理）

- 确认可视化设计统一由已固定接入的 `external/LvglEditor` 承担，桌面复用同一 Web 工作台；新增三目标职责知识和 OUT 路线，C/LVGL、LispBM/VESC、BASIC/IoTEmbedded 均为正式规划主线。本次沿用现有子模块 SHA，未合并 PR 或实现目标后端。
- 按用户进一步确认，将三生成器、资源打包、公共模型和初始化目标配置归 LvglEditor，在子仓新增 GEN 路线；HmiCraft 初始化时配置目标/profile 并消费结果，承担设备运行与部署。子仓规划为本地未提交修改，未更新 gitlink。
- 研究文档改为匿名来源、功能结论和本项目选择；详细身份/路径/型号/摘要与原始材料移至仓库外本地研究资料，既有提交历史按同一规则重写。
- 研究盘点工具移除特定产品文件列表，改为显式传入目标文件；支持仅盘点目录，保留有界执行。
- 用户最新决定保持仓库公开；初始根提交已匿名化重写并强制推送，当前可达历史扫描通过，详细资料与回执已移出仓库。

- 新设备 LVGL/BASIC 闭环成为主线，旧 外部样本 格式/C 宏/旧屏兼容后置。
- 明确首版布局生成 BASIC、资源/脚本独立更新、持续事件调度、工业采集与 OPC UA 网关转 Modbus/TCP 的需求；这些是方案变更，设备能力尚未实现。
- 中文项目名采用“可编程人机界面平台”，英文品牌为 HmiCraft；用户确认在 IoTSharp 组织创建同名私有仓库并推送，旧技术标识暂保留。

### Fixed

- 将原始分析与构建产物的忽略规则限定到根目录，避免 `docs/analysis/` 的自编分析报告被 `analysis/` 规则误排除。
- 首次暂存检查发现两份自有文本存在CRLF/混合行尾；按 `.editorconfig` 规范为LF，内容未变，保留失败与修复后的检查回执。
- 文件读写期间阻止编辑与工程切换，避免保存旧快照后清除新修改状态。
- 拖拽捕获丢失和窗口失焦时统一结束拖动，记录撤销与未保存状态。

没有 外部样本 格式、宏执行、真实 PLC/屏幕下载或跨平台发行完成声明。

### 2026-10-11 中途验证回执（保留历史）

- 2026-10-11 已有证据：子仓 Vitest 495通过/48项C编译跳过，tsc -b/Vite/docs sync退出0；真实浏览器C generic/STM32 HAL/RT三个profile ZIP下载通过。父 eng/verify runId `verify-20261011-021113-dfb682` 通过。Lisp/BASIC与三语浏览器仍在验证，正式远端merge待回执；固定LVGL/.ioc/BSP构建、脚本runtime/bridge和设备未验。
- 父OUT-01/03/04/06、HMI-02/03及LVE-09改partial；C统一backend与三profile持久化/目录/导出已实现，固定目标编译和设备门禁保留。

### 2026-10-11 本地最终验证

- 2026-10-11 本地最终证据：子仓509通过/48项C编译跳过，实际npm run build（tsc -b+Vite）退出0；两控件/内置事件/三语言slot样例三目标ZIP/JSON导出、三语无reload切换及BASIC保存/reload恢复通过，九ZIP路径/字节/SHA256独立检查通过。父 eng/verify runId `verify-20261011-021113-dfb682` 通过。PR #3 于 `2026-10-10T18:40:28Z`（北京时间2026-10-11 02:40:28）正式合并；修订head为 `a397fa8389c481b44df47ebef37c6c555b1ef56f`，merge提交为 `9e458f8b7b7eda8898c4633535b68f73679af02e`，已fetch的origin/main与已审查树一致。GEN-11为done-remote，仅记录远端合并回执。首次合并后的Pages CI run `38076733467` 失败：506通过、3个包哈希案例失败、48项C编译跳过，原因是Node20跨realm ArrayBuffer处理；跨realm包哈希补修与revision .5文档已提交并推送子仓main `d325fce517a0fd3a4939ebe8e1b325125054f139`；Node20.20.2 Pages run `38077281534` 已completed/success，509通过/48项C编译跳过、revision .5文档同步、tsc/Vite及build/deploy两个job均成功；部署完成于 `2026-10-10T18:49:42Z`（北京时间2026-10-11 02:49:42）。该代码补修回执对应d325fce；最终纯文档提交 `0ef198a084032994a8e9d3788cc4c2a2c56affa0` 已正常推送并回读子仓main，Node20.20.2 Pages run `38077585386` 对该SHA的509项测试/48项C编译跳过、revision .6同步、tsc/Vite及build/deploy均成功，于北京时间2026-10-11 02:54:07完成；父仓gitlink固定到此可获取提交；首轮失败及本地回执均保留。GEN-09/12保留partial，小样例源码包不替代资源广样例、完整UI覆盖、固定LVGL/.ioc/BSP构建、脚本runtime/bridge与设备证明。
- 子仓门面revision2026-10-11.6同步，验证报告与共同样例已链接；历史失败、中途495项回执及首轮合并CI失败保留，远端合并回执已记录。

本轮源码、浏览器、正式合并、CI与资源状态见[主整合验证报告](docs/verification-pr3-2026-10-11.md)。
