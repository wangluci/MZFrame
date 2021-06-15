namespace MyNet.MQ.Customer
{
    public class TNode : CNode
    {
        public override bool AnyChildrenMatch(Token token)
        {
            return false;
        }
    }
}
