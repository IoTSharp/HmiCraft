# ADR 0003：HmiCraft 品牌、开放平台与在线整合

日期：2026-10-10。状态：名称/仓库/客户与经营方向由用户确认；开源边界为规划基线；具体许可、Web 与桌面宿主实施和 PR 采纳仍需相应验收。

## 决策

中文名“可编程人机界面平台”，英文名HmiCraft。用户授权创建并推送IoTSharp/HmiCraft，最初private，随后明确确认保持public；组织名不作为产品前缀。现有HmiStudio技术标识与文件格式先保留，品牌迁移另有路线。

面向开发者/小团队及设备厂商，自售设备，支持第三方 OEM 和定制。公共平台与基础闭环开放，独立效率工具、量产资料、客户专属成果分别管理。runtime/模板拟 MIT、社区工具拟 MPL-2.0，按文件权属和第三方清单定稿后才能加入许可文件。

按用户新增方向，固定接入 IoTSharp/lvgl-editor，评估 Web 主入口与共用 UI 的桌面/本地桥。当前只接入上游 main；两个 PR 需要选择性复核，LispBM/VESC 目标不替代用户已确认的 BASIC/IoTEmbedded。

## 影响

- ADR 0001 的 Avalonia 是现有桌面切片决定；后续 UI 入口按 LVE 路线演进，不再默认扩展两套完整画布。
- ADR 0002 的固件/包/BASIC/事件/设备分工继续有效。
- 原 C# 核心、样例与合同测试继续保留；Web 模型和它的映射需有版本、未知项处理与共同语料。
- 在线不等于强制云端保存；离线、文件/设备访问和服务器编译单独设计、验证。
- 私有推送、开源许可生效、公开发行、硬件与商品交付均是不同状态。

详见 [知识库](../knowledge/README.md)、[开源边界](../knowledge/open-source-boundaries.md)、[在线整合分析](../analysis/lvgl-editor-integration.md)、[路线图](../../ROADMAP.md)。
