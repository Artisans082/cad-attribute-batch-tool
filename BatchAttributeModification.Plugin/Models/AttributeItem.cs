// 提供 INotifyPropertyChanged 接口，使属性变化能通知界面刷新
using System.ComponentModel;
// 提供 CallerMemberName 特性，自动获取调用成员名，省去手写属性名字符串
using System.Runtime.CompilerServices;
// AutoCAD 数据库服务：ObjectId、AttributeReference 等类型所在命名空间
using Autodesk.AutoCAD.DatabaseServices;

namespace BatchAttributeModification.Models
{
    /// <summary>
    /// 属性块中的一行属性数据模型。
    /// 承载：所属块名、属性标记（Tag）、原始值、待修改的新值。
    /// 同时实现 INotifyPropertyChanged，使 DataGrid 中"新值"列可双向绑定并实时刷新。
    /// </summary>
    public class AttributeItem : INotifyPropertyChanged
    {
        // 新值的私有存储字段（属性变化时触发通知）
        private string _newValue = string.Empty;

        /// <summary>
        /// 该属性在图形数据库中的 ObjectId。
        /// 用于写回阶段定位并打开对应实体，不参与界面展示。
        /// </summary>
        public ObjectId ObjectId { get; set; }

        /// <summary>所属块的名称（用于界面区分不同块）。</summary>
        public string BlockName { get; set; } = string.Empty;

        /// <summary>属性标记，即属性的固定字段名（如"编号""型号"）。</summary>
        public string Tag { get; set; } = string.Empty;

        /// <summary>属性的原始值（只读展示，作为修改前的参照）。</summary>
        public string OldValue { get; set; } = string.Empty;

        /// <summary>
        /// 修改后的新值（可编辑列）。
        /// 赋值时会同时通知"新值"与"IsModified"两条属性，驱动界面与写回逻辑更新。
        /// </summary>
        public string NewValue
        {
            get
            {
                return _newValue;
            }
            set
            {
                // 仅当值真正发生变化时才触发通知，避免无谓的界面刷新
                if (_newValue != value)
                {
                    _newValue = value;
                    OnPropertyChanged();                       // 通知"NewValue"已变化
                    OnPropertyChanged(nameof(IsModified));     // 联动通知"IsModified"
                }
            }
        }

        /// <summary>
        /// 是否发生了修改（新值 != 原值）。
        /// 批量写回时只处理这些行，减少不必要的数据库写操作。
        /// </summary>
        public bool IsModified
        {
            get
            {
                return NewValue != OldValue;
            }
        }

        /// <summary>属性变化事件，WPF 绑定系统订阅它实现界面自动更新。</summary>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// 触发属性变化通知。
        /// [CallerMemberName] 会自动填充调用它的属性名，因此无需显式传参。
        /// </summary>
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
