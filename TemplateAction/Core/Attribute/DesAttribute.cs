﻿using System;


namespace TemplateAction.Core
{
    /// <summary>
    /// 对控制器或动作的描述
    /// 不可重复使用，并且子类无法继承
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
    public class DesAttribute : Attribute
    {
        /// <summary>
        /// 描述
        /// </summary>
        private string mDes;

        public DesAttribute(string des)
        {
            mDes = des;
        }


        public string Des { get { return mDes; } }
 
    }
}
