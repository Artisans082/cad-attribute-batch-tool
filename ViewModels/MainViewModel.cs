// 提供 EventArgs、EventHandler 等事件相关类型
using System;
// 提供 IEnumerable<T> 接口（构造函数入参）
using System.Collections.Generic;
// 提供 ObservableCollection<T>：集合变化时自动通知界面（DataGrid 实时刷新）
using System.Collections.ObjectModel;
// 提供 ICommand 接口
using System.Windows.Input;
// 引入数据模型层
using BatchAttributeModification.Models;

namespace BatchAttributeModification.ViewModels
{
    /// <summary>
    /// 批量修改属性的主 ViewModel。
    /// 负责：持有全部待修改属性行、提供"确定/取消"命令、通知窗口关闭。
    /// 不引用任何 CAD API，便于单元测试与界面解耦。
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        /// <summary>
        /// 待编辑的属性行集合。
        /// 使用 ObservableCollection 使 DataGrid 能实时反映集合变化。
        /// </summary>
        public ObservableCollection<AttributeItem> Items { get; }

        /// <summary>"确定"按钮绑定的命令：确认修改并请求关闭窗口。</summary>
        public ICommand ApplyCommand { get; }

        /// <summary>"取消"按钮绑定的命令：直接请求关闭窗口。</summary>
        public ICommand CloseCommand { get; }

        /// <summary>
        /// 请求关闭窗口的事件。
        /// 由 View（MainWindow）订阅；点击确定/取消后触发，View 负责关闭自身。
        /// </summary>
        public event EventHandler? RequestClose;

        /// <summary>
        /// 构造函数：接收读取到的属性行，初始化集合与命令。
        /// </summary>
        /// <param name="items">从 AutoCAD 读取的属性行集合。</param>
        public MainViewModel(IEnumerable<AttributeItem> items)
        {
            // 把传入的集合装载进可观察集合
            Items = new ObservableCollection<AttributeItem>(items);
            // 绑定"确定"命令到内部处理方法
            ApplyCommand = new RelayCommand(OnApply);
            // 绑定"取消"命令：直接触发关闭请求
            CloseCommand = new RelayCommand(OnClose);
        }

        /// <summary>
        /// 点击"确定"后的处理。
        /// 编辑结果已实时写入 Items，这里只需请求关闭窗口；
        /// 关闭后由命令流程（ApplicationPlugin）读取 Items 并执行批量写回。
        /// </summary>
        /// <param name="parameter">命令参数（此处不使用）。</param>
        private void OnApply(object? parameter)
        {
            // 触发关闭请求（数据都已在 Items 上，无需额外提交）
            RequestClose?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// 点击"取消"后的处理：直接请求关闭窗口，不执行写回。
        /// </summary>
        /// <param name="parameter">命令参数（此处不使用）。</param>
        private void OnClose(object? parameter)
        {
            RequestClose?.Invoke(this, EventArgs.Empty);
        }
    }
}
