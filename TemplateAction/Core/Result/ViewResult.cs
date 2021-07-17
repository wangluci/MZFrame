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
            TASiteApplication tdata = mHandle.Context.Application;
            string zPath = "/" + mHandle.NameSpace + "/" + module + "/" + node + TAUtility.FILE_EXT;
            TemplateDocument indexTemp = TemplateApp.Instance.LoadViewPage(zPath);
            if (indexTemp == null)
            {
                return "视图不存在";
            }

            return indexTemp.MakeHtml(mHandle.TemplateContext);
        }
        public void Output()
        {
            mHandle.Context.Response.ContentType = "text/html";
            mHandle.Context.Response.Write(mHtml);
        }
    }
}
