using System;

namespace TemplateAction.Core
{
    public class HttpGetAttribute : ActionNodeAttribute
    {
        public override void ConfigAction(ActionNode node)
        {
            node.AllowHttpMethod |= (byte)ActionNode.TAHttpMethod.Get;
        }
    }
}
