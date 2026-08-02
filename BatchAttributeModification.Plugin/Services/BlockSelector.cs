// 提供泛型集合类型
using System.Collections.Generic;
// AutoCAD 应用服务：Document（文档对象）
using Autodesk.AutoCAD.ApplicationServices;
// AutoCAD 数据库服务：ObjectId（实体标识）
using Autodesk.AutoCAD.DatabaseServices;
// AutoCAD 编辑器交互：SelectionFilter、TypedValue、PromptStatus 等
using Autodesk.AutoCAD.EditorInput;

namespace BatchAttributeModification.Services
{
    /// <summary>
    /// 选择服务：让用户在 AutoCAD 图纸中框选"带属性的块参照"。
    /// 通过 SelectionFilter 过滤选择集，保证只选中我们关心的实体。
    /// </summary>
    public static class BlockSelector
    {
        /// <summary>
        /// 弹出选择提示，仅允许选择"含属性的块参照"。
        /// </summary>
        /// <param name="doc">当前活动文档，用于获取编辑器（Editor）。</param>
        /// <returns>选中实体的 ObjectId 列表；用户取消时返回空列表。</returns>
        public static List<ObjectId> Select(Document doc)
        {
            // Editor 负责与用户进行命令行交互（提示选择、写消息等）
            var ed = doc.Editor;

            // 构造选择过滤器，用 DXF 组码限定可选实体类型：
            //   组码 0  = INSERT → 只允许选块参照（BlockReference）
            //   组码 66 = 1     → 要求该块参照带属性（Attrib 存在）
            var filter = new SelectionFilter(new[]
            {
                new TypedValue(0, "INSERT"),
                new TypedValue(66, 1)
            });

            // 弹出选择提示，等待用户框选后回车确认
            var result = ed.GetSelection(filter);

            // 用户取消（Esc）或未选中任何实体时，返回空列表由上层终止流程
            if (result.Status != PromptStatus.OK)
            {
                ed.WriteMessage("\n未选择任何块，操作已取消。");
                return new List<ObjectId>();
            }

            // 取出选中的实体 ObjectId 列表
            var ids = new List<ObjectId>(result.Value.GetObjectIds());
            // 在命令行反馈选中数量
            ed.WriteMessage($"\n已选择 {ids.Count} 个带属性的块。");
            return ids;
        }
    }
}
