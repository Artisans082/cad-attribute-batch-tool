// 提供 WPF 的 Window 基类
using System.Windows;
// 提供 EventArgs 类型
using System;
// 引入 ViewModel 层
using BatchAttributeModification.ViewModels;

namespace BatchAttributeModification.Views
{
    /// <summary>
    /// 批量修改属性的 WPF 窗口（View）。
    /// 只负责布局与数据绑定，不含任何 CAD 业务逻辑，符合 MVVM 分层。
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// 构造函数：通过构造参数注入 ViewModel 并完成绑定。
        /// </summary>
        /// <param name="viewModel">承载属性数据与命令的 MainViewModel 实例。</param>
        public MainWindow(MainViewModel viewModel)
        {
            // 初始化 XAML 中定义的界面元素（必须最先调用）
            InitializeComponent();
            // 把 ViewModel 设为数据上下文，XAML 中的 {Binding} 均基于它解析
            DataContext = viewModel;
            // 订阅 ViewModel 的关闭请求：用户点击"确定/取消"后关闭本窗口
            viewModel.RequestClose += OnRequestClose;
        }

        /// <summary>
        /// 关闭请求事件的处理：关闭本窗口。
        /// </summary>
        private void OnRequestClose(object? sender, EventArgs e)
        {
            Close();
        }
    }
}
