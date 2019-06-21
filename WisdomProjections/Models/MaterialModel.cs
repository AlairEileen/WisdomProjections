using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WisdomProjections.Models
{
    /// <summary>
    /// 特效数据模型
    /// </summary>
    public class MaterialModel
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public TagModel Tag1 { get; set; }
        public TagModel Tag2 { get; set; }
        public string ResourceFileName { get; set; }
        public string ResourceFileRealName { get; set; }
    }
    /// <summary>
    /// 标签数据模型
    /// </summary>
    public class TagModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public List<TagModel> Tag2 { get; set; }
    }
    /// <summary>
    /// 特效json模型
    /// </summary>
    public class MaterialJsonModel
    {
        public List<MaterialModel> MaterialModels { get; set; }
        public List<TagModel> TagModels { get; set; }
    }
}
