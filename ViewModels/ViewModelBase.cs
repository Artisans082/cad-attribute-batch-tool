// 提供 INotifyPropertyChanged 接口
using System.ComponentModel;
// 提供 CallerMemberName 特性
using System.Runtime.CompilerServices;

namespace BatchAttributeModification.ViewModels
{
    /// <summary>
    /// ViewModel 基类。
    /// 封装 INotifyPropertyChanged 的通用实现，供所有 ViewModel 继承复用，
    /// 是"属性变更 → 界面自动刷新"这一机制的核心。
    /// </summary>
    public abstract class ViewModelBase : INotifyPropertyChanged
    {
        /// <summary>属性变化事件，WPF 数据绑定依赖它实现界面自动刷新。</summary>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// 触发属性变化通知。
        /// [CallerMemberName] 会自动取调用方的属性名，无需手动传字符串。
        /// </summary>
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// 设置属性值并自动通知变化的通用方法（减少样板代码）。
        /// 当新值与旧值相同时返回 false（不触发通知）；有变化时写入并通知，返回 true。
        /// </summary>
        protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
        {
            // 值未变化则直接返回，避免无意义通知
            if (Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }
    }
}
