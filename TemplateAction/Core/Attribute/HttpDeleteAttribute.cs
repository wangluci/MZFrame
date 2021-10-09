using System;
namespace TemplateAction.Core
{
    public class HttpDeleteAttribute : ActionNodeAttribute
    {
        public override void ConfigAction(ActionNode node)
        {
            node.AllowHttpMethod |= (byte)ActionNode.TAHttpMethod.Delete;
        }
    }
}
