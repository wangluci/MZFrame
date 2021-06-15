using System;
using System.Collections.Generic;
using System.Reflection;
using TemplateAction.Label;

namespace TemplateAction.Core
{
    public class TARequestHandle : AbstractTemplateContext, IRequestHandle
    {
        protected const string CONTROLLER_PRE = "CONTR_STORE_";
        protected string mNameSpace;
        public string NameSpace
        {
            get { return mNameSpace; }
        }
        protected string mController;
        public string Controller
        {
            get { return mController; }
        }
        protected string mAction;
        public string Action
        {
            get { return mAction; }
        }
        protected ITAContext mContext;
        public ITAContext Context
        {
            get
            {
                return mContext;
            }
        }
        protected ITemplateContext mTemplateContext;
        public ITemplateContext TemplateContext { get { return mTemplateContext; } }

        [ThreadStatic]
        private static ITAContext _current;
        public static ITAContext Current
        {
            get { return _current; }
        }
        private ITAObjectCollection _extparams;
        public ITAObjectCollection ExtParams
        {
            get { return _extparams; }
        }

        public TARequestHandle(ITAContext context, string ns, string controller, string action, ITAObjectCollection ext)
        {
            mNameSpace = ns;
            mController = controller;
            mAction = action;
            _current = context;
            mContext = context;
            mTemplateContext = this;
            _extparams = ext;
        }
        public void AddGlobal(string key, object value)
        {
            PushGlobal(key, value);
        }
        public T Global<T>(string key, T def)
        {
            return GetGlobal(key, def);
        }
        public bool IsDefine(string key)
        {
            return mGlobalReplace.ContainsKey(key);
        }

        public object Excute(Type controllerType, ActionNode action)
        {
            if (controllerType == null) return null;
            if (action == null) return null;
            //创建控制器
            string tmpcontrollkey = CONTROLLER_PRE + controllerType.FullName;
            IController c = mContext.Items[tmpcontrollkey] as IController;
            if (c == null)
            {
                c = mContext.Application.CreateInstance(controllerType) as IController;
                mContext.Items[tmpcontrollkey] = c;
            }
            if (c == null)
            {
                return null;
            }
            try
            {
                //绑定控制器
                c.Init(this);
                if (!action.JudgeHttpMethod(mContext.Request.HttpMethod)) return null;
                MethodInfo method = action.Method;
                ParameterInfo[] pinfos = method.GetParameters();
                List<object> paramlist = new List<object>();
                //开始遍历参数进行验证
                ITAObjectCollection gc = null;
                if (Context.Request.Form == null)
                {
                    gc = Context.Request.Query;
                }
                else
                {
                    gc = new TAGroupCollection(Context.Request.Query, Context.Request.Form);
                }
                if (!Equals(_extparams, null))
                {
                    gc = new TAGroupCollection(gc, _extparams);
                }

                for (int i = 0; i < pinfos.Length; i++)
                {
                    //创建参数映射
                    object mapobj = MappingFactory.Mapping(gc, pinfos[i]);
                    if (Equals(mapobj, null))
                    {
                        return null;
                    }
                    paramlist.Add(mapobj);
                }

                return c.CallAction(method, paramlist.ToArray());
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                {
                    return c.Exception(ex.InnerException);
                }
                else
                {
                    return c.Exception(ex);
                }
            }
        }
        public override string Include(string src)
        {
            string zPath = string.Empty;
            if (src.StartsWith("/"))
            {
                zPath = src;
            }
            else
            {
                zPath = "/" + mNameSpace + "/" + mController + "/" + src;
            }
            zPath = this.MapPath(zPath);
            TemplateDocument indexTemp = TemplateApp.Instance.LoadRawPage(zPath);
            return indexTemp.MakeHtml(this);
        }
        public string MapPath(string path)
        {
            return mContext.MapPath(path);
        }
    }
}
