# 设备路线规划与子模块接入验证

日期：2026-10-10，Asia/Shanghai。范围：需求、跨项目职责、联网硬件方案、IoTEmbedded 固定子模块和现有桌面代码回归。本轮没有修改 C#/XAML 或子模块源码，没有实现设备运行时。

## 本轮回归

使用 PowerShell 7.6.6 执行 `eng/verify.ps1`，runId：`verify-20261010-215957-c53cf3`，入口退出码 0。

| 步骤 | 退出码 | 结果 |
|---|---|---|
| restore | 0 | 固定 SDK/依赖还原成功 |
| Release build | 0 | 0 警告、0 错误 |
| console 核心合同 | 0 | 11/11 通过；独占测试临时目录已移除 |
| format verify | 0 | 无需格式修改 |

逐步 stdout/stderr、PID/启动时间/父进程/参数与已记录子进程在 `artifacts/process/verify-20261010-215957-c53cf3-*`。核心测试使用 `dotnet run`，没有声称执行 `dotnet test`。

本轮未修改桌面代码，未再次运行 GUI smoke。前一轮的 8 项 Windows smoke 及截图记录见 [桌面验证](verification-2026-10-10.md)，不能将其写成本轮刚执行。Linux/macOS、设备 C 编译、LVGL/BASIC 执行和实物 IO 均未验证。

## 依赖与证据

`iotembedded-final-git-20261010-a1` 退出码 0：父索引存在 `.gitmodules` 和 mode `160000` 的 `external/IoTEmbedded`，gitlink 与子模块 HEAD 同为 `e09e21ff14a7505b4c4c65e084b19293c54bb1e0`，子模块工作区 clean。父仓 `main` 尚无提交/远端，代码和文档主要未跟踪；没有提交、push 或 PR。

硬件厂商 HTML 和来源/摘要记录保留在 `analysis/evidence/hardware-fetch-{1,3}.json`。候选与预算仅用于工程选择；未采购、烧录、连接外设或做工业认证。

文档链接、路线任务引用、HTML 摘要和本轮已记录进程由 `analysis/work/audit-device-planning.ps1` 有界核对，结果保存为 `analysis/evidence/device-planning-audit-final.json`。进程检查只针对已有身份回执，不推导整台机器没有其他进程；历史失败回执及明确保留目录不删除。

小输入试运行和完整审计均退出 0。完整审计检查 8 份进程回执、22 个已记录身份，未发现仍匹配身份的活动进程；13 份文档的 60 个本地链接均存在，80 个路线任务的显式引用无缺失，3 张硬件 HTML 的 SHA-256 均匹配。审计本身由有界监督器记录，正常退出；没有执行删除或终止动作。HTML、子模块和日志作为交付证据保留。

主机预览还需 IEB-11 通用主机 port，已补入 [路线图](../ROADMAP.md) 和 [设备职责](design/embedded-runtime.md)，避免将同一 C/BASIC runtime 的可移植性当作现成功能。
