# HmiStudio 设备执行层

这里用于本产品的 C 设备运行时、LVGL 控件封装、BASIC 的 HMI 扩展，以及基于 IoTEmbedded 的产品固件组合。当前只有经过源码分析的设计与路线，尚无可运行设备固件。

基础框架位于 Git 子模块 `../external/IoTEmbedded`。通用 HAL/BSP、显示与输入驱动、GPIO/ADC/PWM/总线/协议、BASIC 内核和持久化改进归该子项目；画面包、控件、变量绑定、事件动作及产品配置归当前项目。

完整设计：[设备运行时职责与合同](../docs/design/embedded-runtime.md)。固定依赖与能力缺口：[IoTEmbedded 基线](../docs/analysis/iotembedded-baseline.md)。交付顺序与门禁：[ROADMAP](../ROADMAP.md)。

后续纯 C 模块使用 `Inc/` 与 `Src/` 分层；原生控件和 LVGL 编入基础固件，用户通常只下发资源和 BASIC 脚本包。具体目录只随实际实现创建，不提供返回成功的空壳。不会把现有 .NET Desktop 可执行文件当作 MCU 固件。

子模块检出指令在父仓具备可用提交后为：

```console
git submodule update --init external/IoTEmbedded
```

不得自动追随 upstream HEAD；新改动须遵守子模块自己的 AGENTS 和构建门禁。尚未选定硬件或编译工具链，本轮未下载 LVGL、安装交叉编译器、编译或烧录设备。
