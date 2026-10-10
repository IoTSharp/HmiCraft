# PR #3 三目标整合验证 — 2026-10-11

时区：Asia/Shanghai。本报告区分源码合并、生成合同、浏览器行为、远端 CI/Pages 与设备验收；历史失败及后续成功分别保留。

## 正式合并与固定依赖

[IoTSharp/lvgl-editor PR #3](https://github.com/IoTSharp/lvgl-editor/pull/3) 已按用户确认修订并正式合并。起始 main 为 `0d0d62f655442dd5624a00390b7652222a7b3fc8`，原贡献者 head 为 `3647ce025bc65cd14c968c1d329dc40d593f7e51`；实施提交 `0f2652f7b6c4f7950303f76cf195c7a7dba111fc` 保留原 C 生成器与测试，选择性纳入已审查 LispBM/i18n 和必要模型，加入第三条 BASIC 路线。原 PR 的其他画布/资源大幅重构后置。

修订 head `a397fa8389c481b44df47ebef37c6c555b1ef56f` 保留原 PR head 祖先，树与实施提交相同。正常推送到允许维护者编辑的原贡献者分支后，GitHub API 回读 `merged=true`、`state=closed`，合并提交为 `9e458f8b7b7eda8898c4633535b68f73679af02e`。合并时间为 **2026-10-11 02:40:28 北京时间**（2026-10-10 18:40:28 UTC）；fetch 的 origin/main 与已审查树一致，没有 force push。

合并后的 Node 20 补修为 `d325fce517a0fd3a4939ebe8e1b325125054f139`。最终三语文档 revision `.6` 提交为 **`0ef198a084032994a8e9d3788cc4c2a2c56affa0`**，均已正常推送子仓 main；GitHub API 已回读 main 指向最终提交。HmiCraft 本轮 gitlink 固定到该可获取版本，子仓工作区干净；IoTEmbedded 仍固定 `e09e21ff14a7505b4c4c65e084b19293c54bb1e0`。

## 本批实现

- 三目标稳定标识为 `c-lvgl`、`lispbm-vesc`、`basic-iotembedded`；共用工程模型、目标校验、源码/包生成入口与所有导出。新建和项目设置可修改，旧工程/独立编辑器默认 C，宿主配置可给 HmiCraft 自有设备入口设置 BASIC 默认与允许目标。
- 各语言手写事件/逻辑分别保存，切换不互译、不覆盖；回归覆盖 custom→builtin→custom 编辑、保存/重开/恢复。未知、禁用、冲突或不支持功能给出定位诊断并阻止导出。
- C 是统一后端，内含 `generic`、`stm32cube-hal`、`rt-thread-scons` profile。保存、重开、导出目录、`integration-contract.json` 和 RT-Thread SConscript 已实现；外部平台 profile 不用内置 Web 编译替代目标工具链。
- LispBM/VESC 已接公共输出；缓存有完整 SHA256 与容量上限，字体/LVGL 版本/资源引用和符号冲突提前校验。原 C 字体失败已修复，稳定 ID 动作引用对应实际 C 声明，snake/camel 两种命名均回归。
- BASIC 提供七类基础控件的像素布局、基础样式/动作/用户事件，生成和用户模块分离，稳定 ID source map、manifest、真实 SHA256 与 `hmicraft-hmi-basic/1-draft` 合同。
- 子仓默认英文 README/UI，README 英中俄、ROADMAP 英中同步；界面三语切换不 reload。父仓保持中文。规则在子仓 AGENTS 与父仓知识库登记。

## 验证结果

| 验证 | 实际结果 | 边界 |
|---|---|---|
| 子仓完整 Vitest，本机 Node 24 | 509 通过 / 48 跳过，退出 0 | 跳过的是明确要求配置 emcc/LVGL 的 C 编译案例 |
| 最后代码补修后的完整生产构建 | `npm run build` 退出 0 | 执行 tsc -b 与 Vite；仍有 chunk 大小警告 |
| 补修合同与类型 | 35 个 output/profile 合同通过，tsc 退出 0 | 未弱化哈希算法、长度或断言；Node 20 另由远端验证 |
| GitHub Linux / Node 20.20.2 | 509 通过 / 48 跳过；类型、生产构建、文档检查通过 | run `38077281534`，源码 SHA `d325fce517a0fd3a4939ebe8e1b325125054f139` |
| GitHub Pages | build/deploy 均 success | 上述 run 于北京时间 2026-10-11 02:49:42 完成；静态站点部署不代表设备交付 |
| 最终文档提交的 GitHub CI/Pages | run `38077585386`：509 通过 / 48 跳过；revision .6、类型/构建/部署均 success | 对应最终子仓 SHA `0ef198a084032994a8e9d3788cc4c2a2c56affa0`；部署 job 于北京时间 2026-10-11 02:54:07 完成 |
| 三语文档同步 | 三 README、双 ROADMAP revision `2026-10-11.6`、14 个任务状态、44 个链接一致 | 本机与最终提交的远端检查均通过 |
| 父仓本任务文档 | 9 份文件、113 个本地链接，断链 0；whitespace 退出 0 | 首次只读链接工具误用了 PowerShell 自动变量，失败回执保留；更正后检查通过 |
| Windows Chrome 真实浏览器 | 新建默认英文/C；C 三 profile ZIP 下载；同一两控件样例三目标 ZIP/JSON 导出 | 含内置事件和三个独立语言代码 slot；未替代目标运行 |
| 保存与语言切换 | 英中俄无 reload 切换，保存/reload 恢复 BASIC，三语言用户代码保留 | 最后生产页面两控件和状态栏计数一致 |
| 独立包检查 | 九 ZIP 的路径、逐文件字节长度、SHA256 通过 | 包含最后生产构建的 C/Lisp/BASIC；C 引用与声明一致 |
| HmiCraft `eng/verify.ps1` | Release 0 警告/0 错误；11/11 console 核心合同；format 通过 | runId `verify-20261011-021113-dfb682`；不是 dotnet test |

共同样例：[three-targets.lvgl.json](../external/LvglEditor/samples/three-targets.lvgl.json)。子仓报告：[PR #3 verification](../external/LvglEditor/docs/verification-pr3-2026-10-11.md)。实施与后续门禁：[中文 GEN 路线](../external/LvglEditor/ROADMAP.zh-CN.md) / [父仓 OUT/设备路线](../ROADMAP.md)。

## 保留的失败与证据

首次合并触发的 [Pages run 38076733467](https://github.com/IoTSharp/lvgl-editor/actions/runs/38076733467) 为 **506 通过 / 3 失败 / 48 跳过**，build/deploy 未执行。原因是 Node 20 WebCrypto 拒绝 jsdom 跨 realm 的裸 ArrayBuffer；补修传入完整复制的 Uint8Array 视图，保留真实 SHA256。后续 [代码补修 run 38077281534](https://github.com/IoTSharp/lvgl-editor/actions/runs/38077281534) 与 [最终文档 run 38077585386](https://github.com/IoTSharp/lvgl-editor/actions/runs/38077585386) 均实测通过。原失败日志与后续成功日志分别保留。

原类型/字体失败、中途 495 项通过、俄语重复字典键的类型失败、包审计脚本误匹配 token、浏览器 daemon 被有界工具清理及页面点击 detached 回执均保留。后续修正及 fresh snapshot 的成功不覆盖这些历史结果。

本机回执位于忽略的 `artifacts/process/`：`pr3-final-contracts-20261011-r3`、`pr3-final-production-build-20261011-r4`、`pr3-followup-production-build-20261011`、`pr3-backend-node20-view-tests-20261011k`、`pr3-production-browser-journey-20261011-r3`、`pr3-package-roundtrip-20261011-r3`、`pr3-formal-merge-readback-20261011`、`pr3-node20-fix-run-snapshot-20261011-2`、`pr3-node20-fix-success-log-20261011`、`pr3-final-docs-run-snapshot-20261011-1`、`pr3-final-docs-success-log-20261011` 和 `pr3-final-child-main-readback-20261011`。截图/下载包/审计脚本位于 `artifacts/pr3-browser/`；这些本机证据不是可复现源码依赖。

## 尚未验收与资源保护

所有生成包仍为 `source-contract`、`deployable=false`。IoTEmbedded 未注册 HmiCraft HMI BASIC API 与嵌套导入包 loader；实际 HmiCraft 宿主挂载、runtime、完整图片/字体转换、绑定/动画/图表、预算取消/IO 和包更新恢复继续路线图。C 自定义字体仍需外部转换；BASIC 有首批 source map，C/Lisp 映射为空。

没有执行固定 LVGL 的 native/emcc 编译、具体 STM32 `.ioc`/CubeIDE 工程或 RT-Thread BSP/SCons 目标构建、LispBM 解释器/bridge、BASIC 包运行、固件升级、设备下装、PLC 写入、实板或工业验收。没有新一轮 Avalonia 桌面改动；本轮未重复 Windows 桌面 smoke，Windows Chrome 不证明 Linux/macOS 桌面验收。

两个 named Chrome 会话已关闭，两个 Vite 服务器在有界生命周期后按身份回收。CLI 临时文件已归档，六个本任务临时翻译/审计脚本精确删除，日志/截图/包交付物保留。进程审计依据 PID、创建时间、命令与父进程，不按名称杀进程；没有安装工具、创建独占 MCP 服务或修改全局代理。

北京时间 2026-10-11 02:54:25 的 `pr3-final-resource-audit-20261011.json` 复核了85份 owned-process 回执、265个进程身份，仍匹配的本任务进程为0。4个已复用PID按时间/命令/父链排除并保留，未终止其他会话进程；五份受保护文档的 SHA256 均未变化。后续 Git/API 短进程同样使用有界入口，在退出后回收其已确认归属的树。

父仓已有的 `AGENTS.md`、`docs/adr/0002-embedded-hmi-runtime.md`、`docs/adr/0003-web-and-product-governance.md`、`docs/architecture.md`、`docs/design/embedded-runtime.md` 属于受保护的其他会话修改。本任务不暂存它们，并在提交前后与 `pr3-protected-parent-paths-20261011.json` 的 SHA256 核对；仅提交本任务文档与可获取的子模块 gitlink。
