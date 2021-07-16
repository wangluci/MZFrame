using System;
using System.Collections.Generic;

namespace TemplateAction.Core
{
    public abstract class Node
    {
        protected string mDescript = "";
        public string Descript
        {
            get { return mDescript; }
        }
 
        /// <summary>
        /// 节点键
        /// </summary>
        protected string mKey = "";
        public string Key
        {
            get { return mKey; }
        }


        protected Dictionary<string, Node> mChildrens = new Dictionary<string, Node>();
        public Dictionary<string, Node> Childrens
        {
            get { return mChildrens; }
        }

        public Node GetChildNode(string key)
        {
            Node rtVal = null;
            if (mChildrens.TryGetValue(key.ToLower(), out rtVal))
            {
                return rtVal;
            }
            return null;
        }
        public void AddChildNode(string key, Node n)
        {
            mChildrens[key.ToLower()] = n;
        }
    }
}
