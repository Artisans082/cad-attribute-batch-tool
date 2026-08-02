// 提供泛型集合类型
using System.Collections.Generic;
// AutoCAD 应用服务：Document（文档对象）
using Autodesk.AutoCAD.ApplicationServices;
// AutoCAD 数据库服务：ObjectId、BlockReference、AttributeReference、OpenMode 等
using Autodesk.AutoCAD.DatabaseServices;
// 引入数据模型层
using BatchAttributeModification.Models;

namespace BatchAttributeModification.Services
{
    /// <summary>
    /// 读取服务：在只读事务中读取所选块的全部属性信息，
    /// 转换为可编辑的 <see cref="AttributeItem"/> 列表供界面展示。
    /// </summary>
    public static class AttributeReader
    {
        /// <summary>
        /// 读取属性。
        /// </summary>
        /// <param name="doc">当前活动文档，用于获取数据库（Database）。</param>
        /// <param name="blockIds">选中的块参照 ObjectId 集合。</param>
        /// <returns>所有属性的 AttributeItem 列表（初始新值 = 原值，等待用户编辑）。</returns>
        public static List<AttributeItem> Read(Document doc, IEnumerable<ObjectId> blockIds)
        {
            // 存放读取结果的列表
            var items = new List<AttributeItem>();
            // 从文档获取图形数据库
            var db = doc.Database;

            // 启动只读事务：读取操作无需加文档锁，但需事务保证数据一致性
            using (var tr = db.TransactionManager.StartTransaction())
            {
                // 遍历每个被选中的块参照
                foreach (var id in blockIds)
                {
                    // 以只读方式打开块参照实体
                    var br = tr.GetObject(id, OpenMode.ForRead) as BlockReference;
                    //（类型不符则跳过）
                    if (br == null) continue;
                    // 遍历该块参照的属性集合（AttributeCollection）
                    foreach (ObjectId attId in br.AttributeCollection)
                    {
                        // 以只读方式打开属性参照
                        var att = tr.GetObject(attId, OpenMode.ForRead) as AttributeReference;
                        if (att == null) continue;

                        // 组装数据模型：新值初始等于原值，等待用户在界面中修改
                        items.Add(new AttributeItem
                        {
                            ObjectId  = attId,          // 记录 ObjectId，供写回时定位实体
                            BlockName = br.Name,        // 所属块名
                            Tag       = att.Tag,        // 属性标记（字段名）
                            OldValue  = att.TextString, // 原值
                            NewValue  = att.TextString  // 新值（初始与原值相同）
                        });
                    }
                }

                // 提交只读事务
                tr.Commit();
            }

            return items;
        }
    }
}
