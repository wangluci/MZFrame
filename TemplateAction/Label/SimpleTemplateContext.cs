using System;
namespace TemplateAction.Label
{
    /// <summary>
    /// 简历模板上下文
    /// </summary>
    public class SimpleTemplateContext : AbstractTemplateContext
    {
        public SimpleTemplateContext()
        {

        }
        public override string Include(string src)
        {
            return string.Empty;
        }
    }
}
