
using System.Collections.Generic;

namespace MyNet.MQ.Customer
{
    /// <summary>
    /// 消息队列的路径树
    /// </summary>
    public class MQTree
    {
        private static readonly Token ROOT = new Token("root");
        private static readonly CNode NO_PARENT = null;
        private CNode root;

        public MQTree()
        {
            CNode mainNode = new CNode();
            mainNode.Token = ROOT;
            this.root = mainNode;
        }


        enum NavigationAction
        {
            MATCH, GODEEP, STOP
        }
        private NavigationAction Evaluate(Topic topic, CNode cnode)
        {
            if (Token.MULTI.Equals(cnode.Token))
            {
                return NavigationAction.MATCH;
            }
            if (topic.IsEmpty())
            {
                return NavigationAction.STOP;
            }
            Token token = topic.HeadToken();
            if (!(Token.SINGLE.Equals(cnode.Token) || cnode.Token.Equals(token) || ROOT.Equals(cnode.Token)))
            {
                return NavigationAction.STOP;
            }
            return NavigationAction.GODEEP;
        }
        public MQConsumer[] Match(Topic topic)
        {
            return Match(topic, this.root);
        }

        private MQConsumer[] Match(Topic topic, CNode node)
        {
            NavigationAction action = Evaluate(topic, node);
            if (action == NavigationAction.MATCH)
            {
                MQConsumer[] copyArray = new MQConsumer[node.Customers.Values.Count];
                node.Customers.Values.CopyTo(copyArray, 0);
                return copyArray;
            }
            if (action == NavigationAction.STOP)
            {
                return new MQConsumer[0];
            }
            if (node is TNode)
            {
                return new MQConsumer[0];
            }
            Topic remainingTopic = (ROOT.Equals(node.Token)) ? topic : topic.ExceptHeadToken();
            List<MQConsumer> tcustomers = new List<MQConsumer>();
            if (remainingTopic.IsEmpty())
            {
                foreach (MQConsumer cn in node.Customers.Values)
                {
                    tcustomers.Add(cn);
                }
            }
            foreach (CNode subnode in node.AllChildren())
            {
                tcustomers.AddRange(Match(remainingTopic, subnode));
            }
            return tcustomers.ToArray();
        }

        public void AddToTree(MQConsumer newCustomer)
        {
            Insert(newCustomer.TopicFilter, this.root, newCustomer);
        }

        private void Insert(Topic topic, CNode node, MQConsumer newcustomer)
        {
            Token token = topic.HeadToken();
            if (!topic.IsEmpty() && node.AnyChildrenMatch(token))
            {
                Topic remainingTopic = topic.ExceptHeadToken();
                CNode nextInode = node.ChildOf(token);
                Insert(remainingTopic, nextInode, newcustomer);
            }
            else
            {
                if (topic.IsEmpty())
                {
                    node.AddCustomer(newcustomer);
                }
                else
                {
                    node.Add(CreatePathRec(topic, newcustomer));
                }
            }
        }


        private CNode CreatePathRec(Topic topic, MQConsumer newCustomer)
        {
            Topic remainingTopic = topic.ExceptHeadToken();
            if (!remainingTopic.IsEmpty())
            {
                CNode childnode = CreatePathRec(remainingTopic, newCustomer);
                CNode cnode = new CNode();
                cnode.Token = topic.HeadToken();
                cnode.Add(childnode);
                return cnode;
            }
            else
            {
                return CreateLeafNodes(topic.HeadToken(), newCustomer);
            }
        }

        private CNode CreateLeafNodes(Token token, MQConsumer newCustomer)
        {
            CNode newLeafCnode = new CNode();
            newLeafCnode.Token = token;
            newLeafCnode.AddCustomer(newCustomer);
            return newLeafCnode;
        }

        public void RemoveFromTree(Topic topic, string clientID)
        {
            Remove(clientID, topic, this.root, NO_PARENT);
        }

        private void Remove(string clientId, Topic topic, CNode node, CNode parent)
        {
            Token token = topic.HeadToken();
            if (!topic.IsEmpty() && (node.AnyChildrenMatch(token)))
            {
                Topic remainingTopic = topic.ExceptHeadToken();
                CNode nextnode = node.ChildOf(token);
                Remove(clientId, remainingTopic, nextnode, node);
            }
            else
            {
                if (node is TNode)
                {
                    return;
                }
                if (node.ContainsOnly(clientId) && topic.IsEmpty() && (node.AllChildren().Count == 0))
                {
                    node.RemoveCustomerFor(clientId);
                    if (node == this.root) return;
                    if (node.Customers.Count == 0)
                    {
                        parent.Remove(node);
                    }
                    return;
                }
                else if (node.Contains(clientId) && topic.IsEmpty())
                {
                    node.RemoveCustomerFor(clientId);
                    return;
                }
                else
                {
                    return;
                }
            }
        }
 
    }
}
