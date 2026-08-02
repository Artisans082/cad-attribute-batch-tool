# 批量修改块属性 — AutoCAD 插件

> An AutoCAD plugin (WPF) to batch modify attributes of block references.

针对 **AutoCAD 2025 / 2026** 的 in-process 插件（**.NET 8 + WPF**，MVVM 架构）。
在图纸中框选多个**带属性的块参照**，即可在 WPF 窗口中批量查看、修改属性值并一键写回。

[![License: AGPL-3.0](https://img.shields.io/badge/License-AGPL--3.0-blue.svg)](https://www.gnu.org/licenses/agpl-3.0)

## ✨ 功能特性

- 🎯 **框选即取**：按 `BATMOD` 命令后框选任意多个带属性的块，自动过滤掉无关实体
- 📋 **表格化批量编辑**：以表格展示每个块的 `块名 / 标记 / 原值 / 新值`，直接在"新值"列编辑
- ⚡ **智能写回**：只把**有变化**的属性写回图形数据库，减少无谓写入
- 🔒 **事务安全**：读取用只读事务，写入加文档锁 + 写事务，失败自动回滚
- 🧩 **MVVM 分层**：界面（View）与业务逻辑（ViewModel/Service）完全解耦，便于维护与测试

## 📸 界面预览

<!-- 在此放入 WPF 窗口截图，例如：
![MainWindow](docs/screenshot.png)
-->

## 🔧 环境要求

| 项目 | 要求 |
|------|------|
| AutoCAD | 2025 / 2026（64 位） |
| .NET | .NET 8 SDK（`net8.0-windows`） |
| 开发工具 | Visual Studio 2022+（或 VS Code + C# 扩展） |
| 目标版本 | AutoCAD 2024 及以下需改用 `net48`（见 FAQ） |

## 📁 项目结构

```
Batch Attribute Modification/
├── BatchAttributeModification.csproj   # net8.0-windows / Library / UseWPF
├── Batch Attribute Modification.slnx   # 解决方案文件
├── ApplicationPlugin.cs                # 插件入口 + BATMOD 命令
├── lib/                                # AutoCAD API DLL（git 忽略，需自行复制）
├── Models/
│   └── AttributeItem.cs                # 属性行模型（块名/标记/原值/新值）
├── ViewModels/
│   ├── ViewModelBase.cs                # INotifyPropertyChanged 基类
│   ├── RelayCommand.cs                 # ICommand 通用实现
│   └── MainViewModel.cs                # 属性集合 + 确定/取消命令
├── Views/
│   ├── MainWindow.xaml                 # DataGrid 批量编辑界面
│   └── MainWindow.xaml.cs
├── Services/
│   ├── BlockSelector.cs                # 框选带属性的块（DXF 过滤）
│   ├── AttributeReader.cs              # 只读事务读取属性
│   └── AttributeWriter.cs              # 加锁 + 写事务批量写回
├── Resources/Styles/Styles.xaml        # 界面样式
└── README.md
```

## 🚀 快速开始

### 1. 放置依赖

将 AutoCAD 2025/2026 安装目录下的三个 DLL 复制到项目的 `lib\` 文件夹：

```
C:\Program Files\Autodesk\AutoCAD 2025\AcMgd.dll
C:\Program Files\Autodesk\AutoCAD 2025\AcDbMgd.dll
C:\Program Files\Autodesk\AutoCAD 2025\AcCoreMgd.dll
```

> 项目已通过 `HintPath` 引用它们，且 `Copy Local = false`（不会复制到输出目录）。
>
> ⚠️ **`lib\` 下的 Autodesk 专有 DLL 已被 `.gitignore` 忽略、不会提交到仓库**。
> 每次克隆后都需按上述步骤自行复制；请勿将这些二进制随源码分发
> （详见 [NOTICE](NOTICE)）。

### 2. 编译

```bash
dotnet build
```

> 请使用 **x64** 平台编译（AutoCAD 为 64 位）。

### 3. 加载插件

在 AutoCAD 命令行输入：

```
NETLOAD
```

选择编译输出的 `BatchAttributeModification.dll`。

### 4. 使用

1. 命令行输入 **`BATMOD`**
2. 在图纸中**框选**要修改的带属性块（可多选），回车确认
3. 在弹出的窗口中修改"新值"列
4. 点击 **确定** 写回（点击 **取消** 则放弃修改）

## 🧩 技术架构

```mermaid
flowchart LR
    A[BATMOD 命令] --> B[BlockSelector 框选]
    B --> C[AttributeReader 读取]
    C --> D[MainWindow 弹窗编辑]
    D --> E[AttributeWriter 写回]
```

- **命令层**：`ApplicationPlugin.BatchModify()` 串联完整流程
- **服务层**：`BlockSelector` / `AttributeReader` / `AttributeWriter` 封装全部 CAD API 操作
- **UI 层**：`MainWindow` 仅做布局与数据绑定
- **逻辑层**：`MainViewModel` 持有数据、暴露命令，不引用任何 CAD API

## 🐞 调试

在 Visual Studio 中：

1. 右键项目 → **属性** → **调试** → 启动外部程序设为 AutoCAD 的 `acad.exe`
2. 按 **F5** 启动 AutoCAD 并加载插件，即可断点调试

## ❓ 常见问题

**Q：目标 AutoCAD 是 2024 及以下版本怎么办？**
把 `BatchAttributeModification.csproj` 的 `TargetFramework` 改为 `net48`，
并在 `lib\` 放入对应的 .NET Framework 4.8 版 DLL 后重新编译。

**Q：编译报 `MSB3021/MSB3027` 文件被占用？**
说明 DLL 正被运行中的 AutoCAD 加载。先关闭 AutoCAD（或卸载插件）再重新编译。

**Q：编译有 `MSB3277` 版本冲突警告？**
属无害的传递引用警告，AutoCAD 运行时自带对应程序集，不影响使用。

**Q：命令名会不会与其它插件冲突？**
`BATMOD` 已使用常见前缀；如冲突可在 `ApplicationPlugin.cs` 中修改命令名。

## 🗺️ 后续计划

- [ ] 按块名 / 标记筛选与批量替换
- [ ] 修改进度条（大量块时）
- [ ] 可选 `PaletteSet` 停靠面板模式
- [ ] 撤销 / 预览功能

## 基于 **GNU Affero General Public License v3.0（AGPL-3.0）** 开源发布，
详见根目录的 [LICENSE](LICENSE) 文件。

> 免责声明：本项目与 Autodesk 公司无任何关联或背书；AutoCAD 及相关商标、
> 其 API（`AcMgd.dll` 等）的版权均归 Autodesk 所有。本许可证仅适用于本项目的
> 原创代码，不涉及 Autodesk 的专有组件

本项目仅供学习交流使用。AutoCAD 相关 API 版权归 Autodesk 所有。

