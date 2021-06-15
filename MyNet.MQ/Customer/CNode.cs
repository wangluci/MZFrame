
using System;
using System.Collections.Generic;

namespace MyNet.MQ.Customer
{
    public class CNode
    {
        protected Token _token;
        protected List<CNode> _children;
        protected Dictionary<MQConsumer, MQConsumer> _customers;
        public Dictionary<MQConsumer, MQConsumer> Customers
        {
            get { return _customers; }
        }

        public Token Token
        {
            get { return _token; }
            set { _token = value; }
        }
        public CNode()
        {
            this._children = new List<CNode>();
            this._customers = new Dictionary<MQConsumer, MQConsumer>();
        }
        private CNode(Token token, List<CNode> children, Dictionary<MQConsumer, MQConsumer> customers)
        {
            this._token = token;
            this._customers = new Dictionary<MQConsumer, MQConsumer>(customers);
            this._children = new List<CNode>(children);
        }

        public virtual bool AnyChildrenMatch(Token token)
        {
            foreach (CNode child in _children)
            {
                if (child.EqualsToken(token))
                {
                    return true;
                }
            }
            return false;
        }

        public List<CNode> AllChildren()
        {
            return this._children;
        }

        public CNode ChildOf(Token token)
        {
            foreach (CNode child in _children)
            {
                if (child.EqualsToken(token))
                {
                    return child;
                }
            }
            return null;
        }

        private bool EqualsToken(Token token)
        {
            return token != null && this._token != null && this._token.Equals(token);
        }
        public override int GetHashCode()
        {
            return _token.GetHashCode();
        }

        public CNode Copy()
        {
            return new CNode(this._token, this._children, this._customers);
        }

        public void Add(CNode newNode)
        {
            this._children.Add(newNode);
        }
        public void Remove(CNode node)
        {
            this._children.Remove(node);
        }

        public CNode AddCustomer(MQConsumer customer)
        {
            MQConsumer existing;
            if (_customers.TryGetValue(customer, out existing))
            {
                if (existing.Qos < customer.Qos)
                {
                    _customers.Remove(existing);
                    _customers.Add(customer, customer);
                }
            }
            else
            {
                this._customers.Add(customer, customer);
            }
            return this;
        }
        public bool ContainsOnly(string clientId)
        {
            foreach (MQConsumer sub in this._customers.Values)
            {
                if (!sub.SessionId.Equals(clientId))
                {
                    return false;
                }
            }
            return this._customers.Count > 0;
        }

        public bool Contains(string clientId)
        {
            foreach (MQConsumer sub in this._customers.Values)
            {
                if (sub.SessionId.Equals(clientId))
                {
                    return true;
                }
            }
            return false;
        }

        public void RemoveCustomerFor(string clientId)
        {
            List<MQConsumer> toRemove = new List<MQConsumer>();
            foreach (MQConsumer sub in this._customers.Values)
            {
                if (sub.SessionId.Equals(clientId))
                {
                    toRemove.Add(sub);
                }
            }
            foreach (MQConsumer mq in toRemove)
            {
                this._customers.Remove(mq);
            }
        }
    }
}
