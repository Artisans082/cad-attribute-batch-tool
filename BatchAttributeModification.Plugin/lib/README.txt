请将以下 AutoCAD 2025 / 2026 的程序集 DLL 复制到此文件夹（lib\）：

  AcMgd.dll
  AcDbMgd.dll
  AcCoreMgd.dll

这些文件位于 AutoCAD 安装目录，例如：
  C:\Program Files\Autodesk\AutoCAD 2025\

注意：
- AutoCAD 2025 / 2026 基于 .NET 8，项目 TargetFramework = net8.0-windows。
- 项目已在 .csproj 中通过 HintPath 引用它们，且 Copy Local = false（不会复制到输出目录）。
- 目标 CAD 版本不同，请复制对应版本目录下的 DLL，保持版本一致。
- 若目标是 AutoCAD 2024 及以下（.NET Framework 4.8），请把 csproj 改为 net48 并放入 4.8 版 DLL。
