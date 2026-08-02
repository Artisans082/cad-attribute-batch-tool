// 提供 Action、Predicate、EventHandler 等基础委托类型
using System;
// 提供 ICommand 接口，是 WPF 命令绑定的核心
using System.Windows.Input;

namespace BatchAttributeModification.ViewModels
{
    /// <summary>
    /// 通用命令实现（ICommand）。
    /// 把按钮等控件的点击事件绑定到 ViewModel 中的方法，
    /// 从而实现"界面与业务逻辑解耦"（MVVM 的关键环节）。
    /// </summary>
    public class RelayCommand : ICommand
    {
        // 命令真正要执行的逻辑（接收一个可选参数）
        private readonly Action<object?> _execute;
        // 可选：判断命令当前是否可执行（为 null 表示始终可执行）
        private readonly Predicate<object?>? _canExecute;

        /// <summary>
        /// 构造函数。
        /// </summary>
        /// <param name="execute">要执行的方法（必填，不能为 null）。</param>
        /// <param name="canExecute">可执行性判断方法（可选，默认始终可执行）。</param>
        public RelayCommand(Action<object?> execute, Predicate<object?>? canExecute = null)
        {
            // 若未提供执行方法则抛出异常，尽早暴露配置错误
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        /// <summary>判断命令是否可执行；未提供判断逻辑时始终返回 true。</summary>
        public bool CanExecute(object? parameter)
        {
            // 未提供判断逻辑时视为始终可执行
            return _canExecute?.Invoke(parameter) ?? true;
        }

        /// <summary>执行命令绑定的方法。</summary>
        public void Execute(object? parameter)
        {
            _execute(parameter);
        }

        /// <summary>
        /// 可执行性变化事件。
        /// 关联到 WPF 的 CommandManager.RequerySuggested，当界面状态变化时自动重新查询。
        /// </summary>
        public event EventHandler? CanExecuteChanged
        {
            add
            {
                CommandManager.RequerySuggested += value;
            }
            remove
            {
                CommandManager.RequerySuggested -= value;
            }
        }
    }
}
