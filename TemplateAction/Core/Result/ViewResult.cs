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
            string realpath = mHandle.Context.MapPath(zPath);
            TemplateDocument indexTemp = TemplateApp.Instance.LoadRawPage(realpath);
            if (indexTemp == null)
            {
                //判断是否从模块中取视图文件
                if (tdata.ReadAssetsFromPlugin)
                {
                    indexTemp = tdata.FindPluginView(zPath);
                }
                if (indexTemp == null)
                {
                    return "视图不存在";
                }
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
