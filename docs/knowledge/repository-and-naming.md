# 名称、仓库与首次发行

更新：2026-10-10，Asia/Shanghai。

## 已确认与待定

- 中文名称：**可编程人机界面平台**。
- 英文名称：**HmiCraft**，用户已选择，用于产品品牌和 GitHub 仓库，不加 IoTSharp 前缀。
- GitHub 归属：**IoTSharp 组织**，用户已确认。组织归属与品牌前缀是两件事。
- 远程仓库：**IoTSharp/HmiCraft**，可见性 **public**；最初按private创建，用户后续明确确认保持公开。许可证定稿、正式版本与硬件交付仍分别验收。
- 当前技术标识：`HmiStudio`、`HmiStudio.slnx`、`.hmistudio.json`；不因品牌讨论直接改动持久格式或程序集。

英文品牌与解释性副标题分开：**HmiCraft — Programmable HMI Platform**。产品族可用 HmiCraft Studio / Runtime / Kit / Panel，尚未实现的产品不宣称已上市。公开网页搜索只是初筛，不代表商标或域名已核验；建仓前目标查询返回 404，随后创建和私有推送已完成。

## 当前 Git 事实

本工作区`D:\source\HMIlStudio`的main已推送到`https://github.com/IoTSharp/HmiCraft.git`。原根提交已匿名化重写，带明确lease强推成功；GitHub确认默认分支main，重写根提交为`a6b97ce7ebcca1eccad94a2e048e27070755baca`。随后回读`isPrivate=false`，用户明确确认保持公开。本地旧提交对象已清理，后续文档收尾提交的最终SHA以Git记录和回执为准。

创建、推送、可见性变更观察、历史重写和远端回读都有仓库外本地回执。当前源码公开，但未部署站点、发布正式版本或固件；源码可访问不自动等于项目许可证已经定稿。

IoTEmbedded 固定提交为 `e09e21ff14a7505b4c4c65e084b19293c54bb1e0`；子模块当前未修改。父仓仅记录 gitlink，不把整份上游历史/源码复制进父仓提交。

在线版基础新增 `external/LvglEditor`，固定 `0d0d62f655442dd5624a00390b7652222a7b3fc8`。两个 PR 的 head 在忽略的分析 checkout 中复核，不作为已合并源码；缺少 LICENSE 文件的现状需在公开分发前核查。

## 初始仓库纳入范围

纳入：现有自有 `src/`、`tests/`、`samples/`、`eng/`、`firmware/` 的真实文件，根工程配置/协作文件，`docs/` 下自编报告、知识、方案与路线，以及 `.gitmodules` 和固定 gitlink。商业实施未开始，不提供空壳模块。

不纳入：原始材料/CHM提取/下载/分析副产物、带研究身份的历史回执、本机配置、凭据、客户内容、生产机密。必要研究资料单独保留在仓库外本地目录。`bin/obj`与构建产物不提交；根`.gitignore`仍保留`/analysis/`、`/artifacts/`防误收规则，`docs/analysis/`仅放匿名结论。

## GOV-03 验收

1. 英文名、准确 owner/name 和可见性已明确；先查是否存在同名仓库，不覆盖已有远端。
2. 初始内容清单、许可状态、排除项和子模块固定 SHA 审阅完成。
3. 按项目门禁验证本地树，保留失败与成功记录；明确未执行的硬件/平台范围。
4. 精确提交已确认范围，创建/连接目标远端并推送；记录本地提交、远端分支 SHA、仓库 URL/可见性和子模块可获取性。
5. 只有实际完成才把 GOV-03 标为 done-local；源代码推送不等于开源发行、许可证生效、设备通过或商品上市。

英文品牌确认后的 UI/程序集/包名迁移属于 GOV-07：先列显示名称与技术标识映射，保留旧工程读取，再执行涉及桌面代码的 GUI smoke。当前文档改名不自动执行该迁移。
