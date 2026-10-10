# HmiCraft · 可编程人机界面平台知识库

更新：2026-10-11，Asia/Shanghai。这里存放随源码版本维护的项目知识。中文名与英文名HmiCraft已确认，IoTSharp组织仓库已推送，用户最新明确保持公开；实际状态见[仓库与命名](repository-and-naming.md)。

## 从哪里开始

| 要解决的问题 | 入口 | 主维护角色 |
|---|---|---|
| 用户到底确认了什么，还有什么未定 | [决策登记](decisions.md) | 产品/架构 |
| 可视化设计由谁维护，三种输出如何分工 | [统一设计器与三目标](designer-and-targets.md) | 编辑器/编译器/设备集成 |
| 平台开源什么，商业和客户代码放哪里 | [开源与商业边界](open-source-boundaries.md) | 架构/发布 |
| 自售设备、第三方 OEM、定制如何分工 | [业务与交付](business-and-oem.md) | 产品/硬件/交付 |
| 名称、Git、远程仓库与首次发行 | [仓库与命名](repository-and-naming.md) | 仓库维护者 |
| 多来源研究如何匿名表达，详细资料放哪里 | [研究与归属](research-and-attribution.md) | 产品/研究 |
| 当前实现、下一任务、验收条件 | [路线图](../../ROADMAP.md) / [交接](../../HANDOFF.md) | 对应路线负责人 |

## 产品事实

这是VB6式可视化设计器与独立设备执行层组成的平台。用户确认可视化设计统一基于lvgl-editor，共用工程模型并支持C/LVGL、LispBM/VESC、BASIC/IoTEmbedded三条正式输出主线。Web为主要编辑入口，现有C#/.NET/Avalonia切片作为兼容与回归基线保留；自有设备路线将布局生成BASIC，与用户脚本和资源打包，固件内置IoTEmbedded、LVGL及HMI API。子仓三后端统一源码接口、首批BASIC生成与包合同已实现，小样例三目标ZIP/JSON及语言切换/保存恢复通过；HmiCraft宿主消费、目标runtime和设备链路尚未贯通。

优先 Windows，再 Linux/macOS；先新设备闭环，旧 外部样本 工程/C 宏/旧屏兼容后置。首版监视采集与外设事件，高速计数交给硬件；OPC UA 经独立网关转 Modbus/TCP。

用户新增在线版优先研究方向，整合现有 lvgl-editor 和两个 PR 的适用部分。已接入固定 Web 基线；C# 核心合同与原桌面切片保留，Web工程→基础BASIC源码包的子仓接线已完成；HmiCraft宿主消费和IoTEmbedded运行链路尚未完成。见 [在线版整合分析](../analysis/lvgl-editor-integration.md)。

最新输出归属见 D34，修订合并/切换/语言补充见 D35～D37：可视化、公共模型、三后端生成及初始化配置均在 LvglEditor；HmiCraft 初始化时指定目标和设备/API profile，消费结果并对接运行/部署。子仓 [GEN 路线](../../external/LvglEditor/ROADMAP.md) 为实施来源，父仓 OUT/LVE 负责集成映射。2026-10-11首批基础BASIC生成/包合同与PR #3修订合并已有回执（merge `9e458f8b7b7eda8898c4633535b68f73679af02e`）；包哈希补修及revision .5已推送main `d325fce517a0fd3a4939ebe8e1b325125054f139`，Node20.20.2 Pages run `38077281534`源码测试/构建/Pages部署成功；子仓 README 三语/路线双语，UI 默认英文可切中俄，界面覆盖独立验收；父仓继续中文。

客户包括开发者/小团队和设备厂商；业务包括自售设备、第三方 OEM 和定制。开源核心帮助接入，硬件、适配验证、维护和独立效率工具形成收费交付。当前没有固件产品、订单或商业发行完成声明。

## 技术知识入口

- [设计器与设备架构](../architecture.md)、[设备职责与运行合同](../design/embedded-runtime.md)。
- [IoTEmbedded 固定源码基线](../analysis/iotembedded-baseline.md)、[硬件候选与成本研究](../design/hardware-selection.md)。
- [原程序首轮分析](../analysis/overview.md)、[帮助功能矩阵](../analysis/manual-analysis.md)、[设备/脚本契约](../analysis/device-and-script-contracts.md)、[REA 原生诊断](../analysis/native-troubleshooting.md)。
- [桌面技术决策](../adr/0001-desktop-framework.md)、[设备运行时决策](../adr/0002-embedded-hmi-runtime.md)、[质量门禁](../quality-gates.md)。
- [命名和商业研究备忘](../design/product-and-business-proposal.md)：保留研究与价格假设；后续已确认决定以本知识库为准。

## 维护规则

1. 新信息先标注“用户确认 / 规划基线 / 已验证事实 / 待定”，并关联稳定路线 ID。
2. 改决策时更新登记和对应专题；ROADMAP 管执行状态，HANDOFF 管本轮 Git/验证/资源状态，避免四处复制不同结论。
3. 历史提案和失败证据保留，不用后续成功覆盖。商业报价、性能和销量假设不可改写成已验证结果。
4. 原始提取、详细身份和历史回执放仓库外本地目录；`docs/analysis/`只保留匿名结论。当前文件与可达历史均检查，不能只依赖`.gitignore`。

本轮源码、浏览器、正式合并、CI与资源状态见[主整合验证报告](../verification-pr3-2026-10-11.md)。
