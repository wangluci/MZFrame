using System;
using System.Reflection;
using System.Threading.Tasks;
using TemplateAction.Label;
namespace TemplateAction.Core
{
    /// <summary>
    /// 基础控制器,所有控制器应该继承这个
    /// </summary>
    public abstract class TABaseController : IController
    {
        protected ITAAction mAction;
        protected ITAContext Context { get { return mAction.Context; } }
        protected ITARequest Request { get { return mAction.Context.Request; } }
        protected ITAResponse Response { get { return mAction.Context.Response; } }
        public virtual void Init(ITAAction handle)
        {
            mAction = handle;
        }
        protected void Define(string key)
        {
            mAction.AddGlobal(key, string.Empty);
        }
        /// <summary>
        /// 设置全局变量
        /// </summary>
        /// <param name="key"></param>
        /// <param name="val"></param>
        protected void SetGlobal(string key, object val)
        {
            mAction.AddGlobal(key, val);
        }
        /// <summary>
        /// 同步重定向
        /// </summary>
        /// <param name="controller"></param>
        /// <param name="action"></param>
        /// <param name="ns"></param>
        /// <returns></returns>
        protected virtual IResult Redirect(string controller, string action, string ns = "")
        {
            if (string.IsNullOrEmpty(ns))
            {
                ns = mAction.NameSpace;
            }
            TAActionBuilder builder = mAction.Context.Application.CreateTARequestHandleBuilder(mAction.Context, ns, controller, action);
            return builder.Start();
        }
        /// <summary>
        /// 异步重定向
        /// </summary>
        /// <param name="controller"></param>
        /// <param name="action"></param>
        /// <param name="ns"></param>
        /// <returns></returns>
        protected virtual Task RedirectAsync(string controller, string action, string ns = "")
        {
            if (string.IsNullOrEmpty(ns))
            {
                ns = mAction.NameSpace;
            }
            TAActionBuilder builder = mAction.Context.Application.CreateTARequestHandleBuilder(mAction.Context, ns, controller, action);
            return builder.StartAsync();
        }
        /// <summary>
        /// 返回默认视图
        /// </summary>
        /// <returns></returns>
        protected virtual ViewResult View()
        {
            return View(mAction.Controller, mAction.Action);
        }
        protected StreamResult Stream(string filename, byte[] data)
        {
            return new StreamResult(mAction, filename, data);
        }
        /// <summary>
        /// 返回指定视图
        /// </summary>
        /// <param name="moduleName"></param>
        /// <param name="moduleNode"></param>
        /// <returns></returns>
        protected ViewResult View(string moduleName, string moduleNode)
        {
            return new ViewResult(mAction, moduleName, moduleNode);
        }
        protected virtual ViewResult View(string moduleNode)
        {
            return new ViewResult(mAction, mAction.Controller, moduleNode);
        }
        protected PngResult Png(byte[] data)
        {
            return new PngResult(mAction, data);
        }
        protected GifResult Gif(byte[] data)
        {
            return new GifResult(mAction, data);
        }
        protected FileResult File(string path)
        {
            return new FileResult(Context, path);
        }
        protected AjaxResult Err(string mess)
        {
            return Err(-1, mess);
        }
        /// <summary>
        /// 错误时调用
        /// </summary>
        /// <param name="error">错误代码<=-1</param>
        /// <param name="mess"></param>
        /// <returns></returns>
        protected AjaxResult Err(int error, string mess)
        {
            return new AjaxResult(Context, error, mess, null);
        }
        protected AjaxResult Success(string mess, string jsondata)
        {
            return new AjaxResult(Context, 0, mess, jsondata);
        }
        protected AjaxResult Success(int code, string jsondata)
        {
            return new AjaxResult(Context, code, string.Empty, jsondata);
        }
        protected AjaxResult Success()
        {
            return Success(null);
        }
        protected AjaxResult Success(string jsondata)
        {
            return Success(string.Empty, jsondata);
        }
        protected TextResult Content(string content)
        {
            return new TextResult(mAction, content);
        }

        /// <summary>
        /// 控制器异常
        /// </summary>
        /// <param name="ex">异常类</param>
        /// <returns></returns>
        public virtual IResult Exception(Exception ex)
        {
            return new ExceptionResult(Context, ex);
        }
        /// <summary>
        /// 路由执行
        /// </summary>
        /// <returns></returns>
        public virtual object CallAction(MethodInfo method, object[] parameters)
        {
            return method.Invoke(this, parameters);
        }
        public ITAAction RequestHandle
        {
            get { return mAction; }
        }
    }
}
