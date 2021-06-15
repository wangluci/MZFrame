using System;
using TemplateAction.Common;
using TemplateAction.Label;

namespace TemplateAction.Core
{
    public class ViewResult : IResult
    {
        protected IRequestHandle mHandle;
        protected string mHtml;
        public override string ToString()
        {
            return mHtml;
        }
        public ViewResult(IRequestHandle handle, string html)
        {
            mHandle = handle;
            mHtml = html;
        }
        public ViewResult(IRequestHandle handle)
        {
            mHandle = handle;
            mHtml = ModuleToString(handle.Controller, handle.Action);
        }
        public ViewResult(IRequestHandle handle, string module, string node)
        {
            mHandle = handle;
            mHtml = ModuleToString(module, node);
        }

        /// <summary>
        /// 生成View
        /// </summary>
        /// <returns></returns>
        public string ModuleToString(string module, string node)
        {
            string zPath = "";
            TAApplication tdata = mHandle.Context.Application;
            if (module.Equals("root"))
            {
                zPath = "/" + node + TAUtility.FILE_EXT;
            }
            else
            {
                zPath = "/" + mHandle.NameSpace + "/" + module + "/" + node + TAUtility.FILE_EXT;
            }

            zPath = mHandle.Context.MapPath(zPath);
            TemplateDocument indexTemp = TemplateApp.Instance.LoadRawPage(zPath);
            return indexTemp.MakeHtml(mHandle.TemplateContext);
        }
        public void Output()
        {
            mHandle.Context.Response.ContentType = "text/html";
            mHandle.Context.Response.Write(mHtml);
        }
    }
}
