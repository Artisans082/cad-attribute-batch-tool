
// 提供 WindowInteropHelper：把 WPF 窗口的 Owner 设为 AutoCAD 主窗口（Win32 句柄）
using System.Windows.Interop;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.Runtime;
using BatchAttributeModification.Services;
// 项目内 ViewModel 层：主界面逻辑
using BatchAttributeModification.ViewModels;
// 项目内 View 层：WPF 窗口
using BatchAttributeModification.Views;

// 声明本程序集中包含 AutoCAD 命令方法的类，便于 AutoCAD 加载时快速扫描注册命令
[assembly: CommandClass(typeof(BatchAttributeModification.ApplicationPlugin))]

namespace BatchAttributeModification
{
    /// <summary>
    /// 插件入口。
    /// 实现 IExtensionApplication 以在程序集加载/卸载时获得回调；
    /// 并通过 CommandMethod 特性注册命令行命令。
    /// </summary>
    public class ApplicationPlugin : IExtensionApplication
    {
        /// <summary>程序集被加载（如 NETLOAD）时由 AutoCAD 调用，可在此做初始化。</summary>
        public void Initialize() { }

        /// <summary>程序集被卸载时由 AutoCAD 调用，可在此释放资源。</summary>
        public void Terminate() { }

        /// <summary>
        /// 批量修改块属性的主命令。
        /// 用户在命令行输入 BATMOD 触发，流程：
        ///   ① 框选带属性的块 → ② 读取属性 → ③ 弹出 WPF 窗口修改 → ④ 批量写回。
        /// </summary>
        [CommandMethod("BATMOD")]
        public static void BatchModify()
        {
            // 获取当前活动文档；若没有打开的图纸则直接返回
            var doc = Application.DocumentManager.MdiActiveDocument;
            if (doc == null) return;
            // Editor 用于与用户交互：提示选择、输出命令行消息等
            var ed = doc.Editor;

            // ① 让用户在图纸中框选带属性的块参照（SelectionFilter 做过滤）
            var ids = BlockSelector.Select(doc);
            if (ids.Count == 0) return;   // 用户取消或未选中任何块，终止流程

            // ② 在只读事务中读取属性，生成可编辑的内存列表
            var items = AttributeReader.Read(doc, ids);
            if (items.Count == 0)
            {
                ed.WriteMessage("\n所选块不包含任何属性。");
                return;
            }

            // ③ 构造 ViewModel 与 WPF 窗口，并以 AutoCAD 主窗口为 Owner 弹出（模态）
            var vm = new MainViewModel(items);
            var win = new MainWindow(vm);
            new WindowInteropHelper(win).Owner = Application.MainWindow.Handle;
            win.ShowDialog();   // 阻塞等待用户点击"确定 / 取消"

            // ④ 用户确认后，把修改过的属性批量写回图形数据库
            var count = AttributeWriter.Write(doc, vm.Items);
            // 在命令行反馈执行结果
            ed.WriteMessage($"\n批量修改完成，共修改 {count} 个属性。");
        }
    }
}
