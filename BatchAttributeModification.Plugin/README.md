# 批量修改块属性 — AutoCAD 插件

针对 **AutoCAD 2025 / 2026** 的 in-process 插件（.NET 8 + WPF），
用户框选图纸中的多个带属性块后，在 WPF 窗口中批量修改属性值并写回。

> 若目标是 AutoCAD 2024 及以下，请把 `BatchAttributeModification.csproj` 的
> `TargetFramework` 改为 `net48`，并在 `lib\` 放入对应的 .NET Framework 4.8 版 DLL 后重新编译。

## 目录结构

```
BatchAttributeModification.Plugin/
├── BatchAttributeModification.csproj   # net48 / Library / UseWPF
├── ApplicationPlugin.cs                # 插件入口 + BATMOD 命令
├── lib/                                # ← 手动放置 AutoCAD DLL（见下方）
├── Models/
│   └── AttributeItem.cs                # 属性行模型（块名/标记/原值/新值）
├── ViewModels/
│   ├── ViewModelBase.cs                # INotifyPropertyChanged 基类
│   ├── RelayCommand.cs                 # ICommand 实现
│   └── MainViewModel.cs                # 属性集合 + 确定/取消命令
├── Views/
│   ├── MainWindow.xaml                 # DataGrid 批量编辑界面
│   └── MainWindow.xaml.cs
├── Services/
│   ├── BlockSelector.cs                # 框选带属性的块
│   ├── AttributeReader.cs              # 只读事务读取属性
│   └── AttributeWriter.cs              # 加锁 + 写事务批量写回
└── Resources/Styles/Styles.xaml        # 界面样式
```

## 使用步骤

1. **放置依赖**：在 `lib\` 目录放入 AutoCAD 2024 安装目录下的
   `AcMgd.dll`、`AcDbMgd.dll`、`AcCoreMgd.dll`。
2. **编译**：Visual Studio（选择 x64）或命令行：
   ```
   dotnet build
   ```
3. **加载插件**：打开 AutoCAD，命令行输入 `NETLOAD`，选择编译输出的
   `BatchAttributeModification.dll`。
4. **执行**：命令行输入 `BATMOD` → 框选带属性的块 → 在窗口里修改"新值" → 点"确定"。

## 调试

在 Visual Studio 中把调试启动程序（Start external program）设为
`acad.exe`（AutoCAD 安装目录），按 F5 即可启动 AutoCAD 并断点调试插件。

## 说明

- 命令名 `BATMOD` 建议保持唯一前缀，避免与其他插件冲突。
- 读属性用只读事务；写回时通过 `doc.LockDocument()` + 写事务，仅处理有变化的行。
- `MainWindow` 以 AutoCAD 主窗口为 Owner 弹出模态窗口；如需边改边看图纸效果，
  可改为 `PaletteSet` 停靠面板。
