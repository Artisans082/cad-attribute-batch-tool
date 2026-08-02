// 提供泛型集合类型
using System.Collections.Generic;
// AutoCAD 应用服务：Document（文档对象，提供 LockDocument）
using Autodesk.AutoCAD.ApplicationServices;
// AutoCAD 数据库服务：ObjectId、AttributeReference、OpenMode 等
using Autodesk.AutoCAD.DatabaseServices;
// 引入数据模型层
using BatchAttributeModification.Models;

namespace BatchAttributeModification.Services
{
    /// <summary>
    /// 写回服务：把用户在界面中修改后的属性值批量写回图形数据库。
    /// 只处理值发生变化的行；写操作必须加文档锁并开启写事务，以保证数据安全。
    /// </summary>
    public static class AttributeWriter
    {
        /// <summary>
        /// 批量写回修改的属性。
        /// </summary>
        /// <param name="doc">当前活动文档（用于加锁与获取数据库）。</param>
        /// <param name="items">包含修改后新值的属性行集合。</param>
        /// <returns>实际修改的属性数量（没有任何变化时返回 0）。</returns>
        public static int Write(Document doc, IEnumerable<AttributeItem> items)
        {
            // 只筛选出确实发生变化的行，减少无谓的数据库写操作
            var changed = new List<AttributeItem>();
            foreach (var item in items)
            {
                if (item.IsModified)
                {
                    changed.Add(item);
                }
            }

            // 没有任何变化时直接返回，无需开启事务
            if (changed.Count == 0) return 0;

            var db = doc.Database;

            // 文档锁：防止写操作与 AutoCAD 内部编辑冲突（修改数据库必须持有）
            using (doc.LockDocument())
            // 启动写事务：对实体的修改在 Commit 时统一生效，异常时自动回滚
            using (var tr = db.TransactionManager.StartTransaction())
            {
                // 遍历每个需要修改的属性行
                foreach (var item in changed)
                {
                    // 以"可写"方式打开属性参照实体（类型不符则跳过）
                    var att = tr.GetObject(item.ObjectId, OpenMode.ForWrite) as AttributeReference;
                    if (att == null) continue;

                    // 写入新的属性值
                    att.TextString = item.NewValue;
                }

                // 提交事务，使所有修改真正写入数据库
                tr.Commit();
            }

            // 返回实际修改的属性数量
            return changed.Count;
        }
    }
}
