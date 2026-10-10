# IoTEmbedded 引入与能力基线

核查日期：2026-10-10，Asia/Shanghai。子模块路径：`external/IoTEmbedded`；上游：`https://github.com/IoTSharp/IoTEmbedded.git`；固定提交：`e09e21ff14a7505b4c4c65e084b19293c54bb1e0`，提交日期 2026-06-01，说明 `Enhance EEPROM Basic Script Management`。这里只报告此提交的源码和工程事实，没有编译或烧录 IoTEmbedded。

## Git 状态

用户明确要求以 Git 子模块引入。本工作区原无 Git 仓库，接入时初始化本地 `main`，新增 `.gitmodules` 与模式 `160000` 的 gitlink，指向上述提交。使用深度 1 拉取，没有配置自动追随分支，没有修改上游源码。接入当时父仓无提交/远端，属于本地证据；之后已按GOV-03创建并推送IoTSharp/HmiCraft私有仓库，当前状态见[仓库知识](../knowledge/repository-and-naming.md)。

操作和进程记录在 `artifacts/process/iotembedded-*-20261010-a1.*`。`iotembedded-gitlink` 回执证明索引中的 `.gitmodules` 和固定 gitlink；`iotembedded-revision` 保留准确提交；`iotembedded-lvgl-tracked-scan` 对该提交全部受跟踪文本搜索 `lvgl / lv_timer_handler / lv_disp_drv / lv_display_create`，退出 1 表示零命中。搜索有 20 秒上限，不扩展到其他仓库。

## 可复核的事实及其影响

下表的“已有”只代表源码/工程存在，不代表本项目或某块实物已验收。

| 领域 | 固定提交中的事实 | 对 HMI 接入的影响 |
|---|---|---|
| 真实目录 | 源码采用 `Application/Board/Bus/Display/Interpreter/Storage/Protocol/Platform` 等模块；README 和 ARCHITECTURE 仍描述尚未落地的 `Core/App/Bsp/Core/Public` | 接入必须按真实头文件和 AGENTS；不能链接不存在的 Core/Public |
| C 语言约定 | AGENTS 要求纯 C，模块 `Inc/` 与 `Src/`，模块前缀 include；CubeMX/第三方文件受保护 | 本项目设备运行时采用 C 边界；C# 仅在设计器和构建工具端 |
| STM32 工程 | `projects/stm32/f1/stm32f103vetx` 与 `l4/stm32l475vetx`；HAL + FreeRTOS/CMSIS-RTOS2，VisualGDB 的 `find_bsp`/`add_bsp_based_executable` | 不是通用的根 CMake 库，不能假设 `add_subdirectory(external/IoTEmbedded)` 即可工作；需要模块选择和应用注入入口 |
| 当前应用 | `Application/Src/app.c` 默认初始化网络、设备、BASIC、显示；`app_rtos.c` 创建固定任务 | HMI 产品有自己的入口与任务组合，不能把旧的设备业务任务全部强制带入 |
| 显示 | `Display/Inc/display_driver.h` 暴露文字、点、线、圆等操作；ST7789 驱动和 Pandora SPI3 适配存在 | 缺少 LVGL 所需的通用像素区域写入、步长/格式、完成通知及缓冲归属合同；不应逐像素调用 PSET 拼凑正式刷屏 |
| 板卡显示 | Pandora L475 描述为 240×240 RGB565 ST7789；F103 profile 的 `BSP_HAS_DISPLAY=0` | F103 工程存在不代表有可运行的触摸屏；Pandora 不能直接代表 800×480 产品 |
| 按键和触控 | Pandora `Keys` 资源仍为 `DRIVER_PENDING`，说明待 GPIO 配置与消抖；没有本次可确认的触控/编码器事件层 | 需要可复用输入驱动与事件接口，再由 HMI 决定导航、点击和业务映射 |
| 外设能力 | UART/RS485、Modbus、MQTT、板级资源表存在；上游路线图明确 GPIO/Timer/Flash 和统一 I2C/SPI 等尚待收敛 | ADC/PWM/数字输入输出必须先形成目标能力和驱动合同，不能按芯片外设数量直接宣称板卡可用 |
| BASIC 引擎 | MY-BASIC 内核、函数 registry、能力位、加载/运行、错误定位存在；`app_basic_run_once` 同步调用 `mb_run` | 应复用引擎。HMI 扩展需要外部函数注册、事件投递、执行预算、取消/让出能力与稳定句柄；当前启动运行一次接口不是完整 HMI 事件宿主 |
| 显示争用 | BASIC 显示函数可直接走旧绘图接口；应用会绘制启动画面 | HMI 启动后需明确单一显示所有者；禁止 BASIC 旧 SCREEN/PSET 路径和 LVGL 同时写屏 |
| 脚本存储 | `app_eeprom.h` 定义两个 4096 字节脚本槽，当前头 256 字节，净脚本上限 3840 字节；有 candidate/active/previous 生命周期代码 | 不能把两个 EEPROM 脚本槽当作图片、动画、字体和 HMI 包仓库；需独立资源存储抽象 |
| L475 持久化 | L475 配置 `BSP_HAS_AT24C=0`；相应 EEPROM read/write 返回 HAL_ERROR；QSPI/TF 在资源表中有映射说明 | “有脚本槽代码”不代表 Pandora 已可持久化完整设计包；应验证 Flash/文件系统后端，防止借源码存在掩盖目标缺口 |
| 非 STM32 | 上游宣称覆盖 RTOS/bare-metal/低资源 Linux，但路线图仍将 Linux profile 标为未开始；当前树未包含 ESP-IDF 目标 | ESP32 等性价比候选会带来 IoTEmbedded 平台移植工作，不能当作零成本替换 MCU |
| 构建工具 | `tools/VisualGDBBuild` 是 .NET 8 控制台工具，提供 build/diagnose/flash 等入口 | 未来用具名配置发现工具链；规划阶段未运行 flash，也未安装或扫描 GNU 工具链 |

## 关键来源

- [仓库约束](../../external/IoTEmbedded/AGENTS.md)、[上游路线图](../../external/IoTEmbedded/ROADMAP.md)、[上游架构文本](../../external/IoTEmbedded/docs/ARCHITECTURE.md)。
- [显示驱动合同](../../external/IoTEmbedded/src/Display/Inc/display_driver.h)、[ST7789 总线合同](../../external/IoTEmbedded/src/Display/Inc/display_st7789.h)、[Pandora 显示适配](../../external/IoTEmbedded/src/Board/Pandora/Src/pandora_display.c)。
- [板级资源表](../../external/IoTEmbedded/src/Board/Pandora/Src/pandora_board_resources.c)、[板级能力宏](../../external/IoTEmbedded/src/Board/Inc/bsp_board.h)。
- [应用入口](../../external/IoTEmbedded/src/Application/Src/app.c)、[任务入口](../../external/IoTEmbedded/src/Application/Src/app_rtos.c)。
- [BASIC 宿主](../../external/IoTEmbedded/src/Interpreter/Src/app_basic.c)、[BASIC 头文件](../../external/IoTEmbedded/src/Interpreter/Inc/app_basic.h)、[函数 registry](../../external/IoTEmbedded/src/Interpreter/Src/app_basic_registry.c)。
- [脚本槽](../../external/IoTEmbedded/src/Storage/Inc/app_eeprom.h)、[EEPROM 后端](../../external/IoTEmbedded/src/Storage/Src/bsp_eeprom.c)、[L475 构建入口](../../external/IoTEmbedded/projects/stm32/l4/stm32l475vetx/CMakeLists.txt)。

## 应回到 IoTEmbedded 的通用改动

这些是需求分析产出的计划，尚未改动子模块；对应路线 ID 见主 ROADMAP。

| ID | 通用改动 | 必须保持的边界 |
|---|---|---|
| IEB-01 | 可选择的模块构建、外部应用/任务注入、统一 feature profile | 默认示例工程仍可构建；框架不反向依赖 HmiStudio 或 `.hmipkg` |
| IEB-02 | 显示区域传输、RGB565/stride/旋转、同步或异步完成及缓存/DMA 合同 | 不在底层公共头泄漏 LVGL 控件对象；兼容旧绘图入口的明确使用方式 |
| IEB-03 | 触摸、按键、编码器驱动；采样/消抖与事件采集 | 不决定某个实体键切哪一页、运行哪段 HMI 脚本 |
| IEB-04 | GPIO/ADC/PWM/UART/RS485 等具名资源服务与真实能力报告 | 不把业务名称硬编码为 MCU 引脚；明确时序、单位、只读/可写与错误状态 |
| IEB-05 | Flash/文件/分区后端和受限流式读写、原子元数据存储 | 不解析本项目的画面/控件/动画描述；保留旧 EEPROM 行为兼容 |
| IEB-06 | BASIC 扩展注册、受限加载、调度让出/取消/预算、外部存储读取 | 不添加依赖 HmiStudio 的函数实现；HMI 专用 API 在本仓注册 |
| IEB-07 | 资源预算、任务健康、版本与能力元数据 | 报告框架和板卡能力；HMI 格式/控件能力由上层合并 |
| IEB-08 | 按最终硬件新增/补齐 BSP 与 RTOS port | STM32 继续遵守现有 CubeMX/VisualGDB 规则；其他架构单独工程，不伪装成 STM32 |
| IEB-09 | 通用传输、分区提交/恢复与固件升级钩子 | HMI 包的兼容/激活策略归 HmiStudio；设备运维命令不混入云端商业逻辑 |
| IEB-10 | Modbus TCP client 与 RTU/串口/TCP 通用驱动合同 | 现有 RTU 不代表 TCP 已完成；HMI 地址映射归本仓；首版 OPC UA 网关不进入框架 |
| IEB-11 | 通用主机 port：可注入时钟、存储与外设服务，构建同一 BASIC 内核 | 不把 STM32 HAL 带进 Windows 预览；不复制第二套 BASIC；HMI 窗口、LVGL 主机显示/输入适配留父仓 |

协议补查：`iotembedded-protocol-scan-20261010-a2` 对 `src/Protocol`、`src/Interpreter`、`docs/basic` 搜索 OPC UA、Modbus TCP、单词 MBAP，零命中。第一次查询 `mbap` 无词边界命中了 BASIC 的 `MBAPI`，该旧回执保留但不能作 Modbus TCP 证据。

先把接口提案与验证用例提交给对应维护角色，再实施小范围改动。当前仅完成固定依赖接入与静态能力分析；子模块成功拉取、头文件可读或上游路线标记完成均不构成硬件验收。
