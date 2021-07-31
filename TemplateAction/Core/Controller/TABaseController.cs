using System;
using System.Reflection;
using TemplateAction.Label;
namespace TemplateAction.Core
{
    /// <summary>
    /// 基础控制器,所有控制器应该继承这个
    /// </summary>
    public abstract class TABaseController : IController
    {
        protected IRequestHandle mRequestHandle;
        protected ITAContext Context { get { return mRequestHandle.Context; } }
        protected ITARequest Request { get { return mRequestHandle.Context.Request; } }
        protected ITAResponse Response { get { return mRequestHandle.Context.Response; } }
        public virtual void Init(IRequestHandle handle)
        {
            mRequestHandle = handle;
        }
        protected void Define(string key)
        {
            mRequestHandle.AddGlobal(key, string.Empty);
        }
        /// <summary>
        /// 设置全局变量
        /// </summary>
        /// <param name="key"></param>
        /// <param name="val"></param>
        protected void SetGlobal(string key, object val)
        {
            mRequestHandle.AddGlobal(key, val);
        }

        protected virtual IResult Redirect(string controller, string action, string ns = "")
        {
            if (string.IsNullOrEmpty(ns))
            {
                ns = mRequestHandle.NameSpace;
            }
            TARequestHandleBuilder builder = mRequestHandle.Context.Application.CreateTARequestHandleBuilder(mRequestHandle.Context, ns, controller, action);
            return builder.BuildAndExcute();
        }
        /// <summary>
        /// 返回默认视图
        /// </summary>
        /// <returns></returns>
        protected virtual ViewResult View()
        {
            return View(mRequestHandle.Controller, mRequestHandle.Action);
        }
        protected StreamResult Stream(string filename, byte[] data)
        {
            return new StreamResult(mRequestHandle, filename, data);
        }
        /// <summary>
        /// 返回指定视图
        /// </summary>
        /// <param name="moduleName"></param>
        /// <param name="moduleNode"></param>
        /// <returns></returns>
        protected ViewResult View(string moduleName, string moduleNode)
        {
            return new ViewResult(mRequestHandle, moduleName, moduleNode);
        }
        protected virtual ViewResult View(string moduleNode)
        {
            return new ViewResult(mRequestHandle, mRequestHandle.Controller, moduleNode);
        }
        protected PngResult Png(byte[] data)
        {
            return new PngResult(mRequestHandle, data);
        }
        protected GifResult Gif(byte[] data)
        {
            return new GifResult(mRequestHandle, data);
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
            return new TextResult(mRequestHandle, content);
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
        public IRequestHandle RequestHandle
        {
            get { return mRequestHandle; }
        }
    }
}
