# HmiCraft 仓库、知识与在线基线验证

日期：2026-10-10，Asia/Shanghai。范围：项目名称/知识库/商业边界/路线、两个固定子模块、Web main 的构建与测试基线、.NET 回归、首次私有仓库交付。原程序安装、C#/XAML 和上游子模块源码未修改；没有公开站点、发布制品或设备操作。

## 固定对象

- 工作区：`D:\source\HMIlStudio`；品牌 HmiCraft，中文名可编程人机界面平台。
- IoTEmbedded：`e09e21ff14a7505b4c4c65e084b19293c54bb1e0`。
- LvglEditor main：`0d0d62f655442dd5624a00390b7652222a7b3fc8`。
- PR #2：`d8a0efe19eddcb58b4c4b7bd6a7ad65361862754`；PR #3：`3647ce025bc65cd14c968c1d329dc40d593f7e51`。仅分析 checkout 保存这两个 head，未合并到父仓固定依赖。
- PowerShell 7.6.6、Node v24.15.0、.NET SDK 10.0.401；长命令用 `eng/Invoke-BoundedProcess.ps1` 限时和记录身份。

## Web main 实测

所有回执位于根 `artifacts/process/`，被忽略并保留在本机；下表中的回执前缀均有 stdout/stderr、process/owned-processes 文件。

| 步骤 | 结果 | 回执前缀 |
|---|---|---|
| 固定 lockfile 安装依赖，禁止安装脚本 | 退出0，316个包，31秒 | `hmicraft-web-npm-ci-20261010-a1` |
| TypeScript `tsc -b` | **退出2**，类型检查失败，12秒 | `hmicraft-web-typecheck-20261010-a1` |
| Vitest，排除环境专属 C 编译文件 | **退出1**，11文件，390项中386通过/4失败，25秒 | `hmicraft-web-tests-20261010-a1` |
| Vite 静态 bundle，关闭编译预览 | 退出0，4秒；仅转译/打包 | `hmicraft-web-bundle-20261010-a1` |

TypeScript 问题并不限于被排除的测试文件：包括 `GeneratedCode` 作为 `Record<string,string>` 的不匹配、`LvglComponent` 未定义、新工程配置缺 `useBuiltinSymbols`、未使用声明等。原始日志完整保留，没有删除类型检查或更改配置让结果通过。

4 项失败均涉及自定义字体：`generator.test.ts` 的字体声明，`ui.c.test.ts` 的字体名称，`ui.h.test.ts` 的字体声明与分段标题。需要对照资源使用/裁剪/命名契约修复实现或测试，不能据此直接认定全部是测试陈旧。

复现命令在 `external/LvglEditor` 中执行，并设置 `VITE_ENABLE_COMPILE_PREVIEW=false`：

```text
npm ci --ignore-scripts --no-audit --no-fund --fetch-retries=1 --fetch-timeout=20000
node node_modules/typescript/bin/tsc -b
node node_modules/vitest/vitest.mjs run --maxWorkers=2 --no-file-parallelism --testTimeout=5000 --hookTimeout=10000 --exclude src/codegen/__tests__/compile.test.ts
node node_modules/vite/bin/vite.js build
```

本机安装采用单请求代理；在本机运行长命令仍须使用有界监督入口。`compile.test.ts` 硬编码另一台 Linux 主机的 Emscripten/LVGL 路径，本机未准备该环境，因此未运行；此排除只用于得到可执行测试的基线，不能成为正式完整门禁。没有扫描/安装编译器。

没有执行两个 PR 的构建/测试，没有浏览器 GUI smoke、断网安装/恢复、Web BASIC 生成、真实下载或硬件验收。上游 main 的普通 `npm run build` 包含类型检查，所以不能称完整构建成功。当前 Web 整合总状态为 **partial，未达到发布门禁**。

## .NET 回归

运行 `eng/verify.ps1`，runId `verify-20261010-225923-c29f48`：restore、Release build、核心 console 合同、format verify 的退出码均为0；Release 0警告/0错误，核心合同11/11通过。合同使用 `dotnet run`，没有声称执行 `dotnet test`。测试独占临时目录已清理。

暂存行尾规范化后再次完成最终门禁，runId `verify-20261010-230650-b648f3`，四步同样全部退出0、0警告/0错误、11/11合同通过；测试临时目录回收成功。没有因Web失败重跑或放宽其测试条件。

本轮未改桌面源码，未重复 Windows GUI smoke；之前的8项桌面 smoke 和截图记录见 [桌面验证](verification-2026-10-10.md)。本次结果不扩展为 Linux/macOS、原工程格式或设备证明。

## 仓库与知识交付

名称/开源与商业边界/OEM与定制/项目职责进入 `docs/knowledge/`；ADR 0003 与 GOV/BIZ/LVE 路线追踪新决定。根 `.gitignore` 锚定 `/analysis/`、`/artifacts/`，使自编 `docs/analysis/` 报告可跟踪，原二进制、CHM提取和下载不进入初始提交。

GitHub 已按用户授权创建 `IoTSharp/HmiCraft`，private，默认分支main。首次推送后，GitHub GraphQL回读的仓库名、可见性、分支和SHA正确；两个固定子模块提交均能通过上游对象查询取得。创建/推送/回读退出码均为0，原回执保留在仓库外本地目录。随后用户要求匿名表达并重写旧历史，本报告不保留旧提交定位信息；最新验证见[匿名化交付](verification-research-anonymization-2026-10-10.md)。

首次内容审计：56项文件/gitlink、28份Markdown、112个本地链接、104个路线任务，全部显式引用有效，两个子模块clean且HEAD/索引一致；未发现所检查的常见凭据模式。审计回执 `analysis/evidence/hmicraft-bootstrap-audit-initial.json`，这不是完整安全审计。

首次暂存空白检查退出2：`eng/Invoke-BoundedProcess.ps1`末行及自有示例JSON为CRLF/混合行尾。在保留失败回执后按已有 `.editorconfig` 规范为LF，对比确认除换行之外文本相同；修复后检查退出0。回执分别为 `hmicraft-staged-whitespace-20261010-a1` 和 `a2`。没有改变脚本行为或JSON语义。

## 资源与证据保留

两个子模块保留；分析checkout、详细来源、失败回执和历史保留目录归档至仓库外本地资料。Web `node_modules`、`dist` 为被忽略的验证产物，不进入父仓。不清理共享工具或原安装。

首次进程审计逐PID查询触及40秒期限，失败回执保留；改为同一组已记录PID的有界批量查询后退出0，32份回执/85个身份无仍匹配进程，未终止任何共享进程。短进程未采到的CIM字段保留未知。资料搬迁、文档匿名化和历史重写的最终结果见后续验证。
