using System;
namespace TemplateAction.Core
{
    public class HttpPutAttribute : ActionNodeAttribute
    {
        public override void ConfigAction(ActionNode node)
        {
            node.AllowHttpMethod |= (byte)ActionNode.TAHttpMethod.Put;
        }
    }
}
