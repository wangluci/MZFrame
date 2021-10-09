using System;
namespace TemplateAction.Core
{
    public class HttpPostAttribute : ActionNodeAttribute
    {
        public override void ConfigAction(ActionNode node)
        {
            node.AllowHttpMethod |= (byte)ActionNode.TAHttpMethod.Post;
        }
    }
}
