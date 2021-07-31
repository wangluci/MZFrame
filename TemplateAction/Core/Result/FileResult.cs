using System;
using System.IO;
using TemplateAction.Common;
using TemplateAction.Label;

namespace TemplateAction.Core
{
    /// <summary>
    /// 文件输出
    /// </summary>
    public class FileResult : IResult
    {
        private string mPath;
        private ITAContext mContext;
        public string Path
        {
            get { return mPath; }
        }

        public FileResult(ITAContext context, string path)
        {
            mContext = context;
            mPath = path;
        }

        public void Output()
        {
            try
            {
                string ext = System.IO.Path.GetExtension(mPath);
                mContext.Response.ContentType = FileContentType.GetMimeType(ext);
                using (FileStream fsRead = new FileStream(mPath, FileMode.OpenOrCreate))
                {
                    byte[] heByte = new byte[fsRead.Length];
                    fsRead.Read(heByte, 0, heByte.Length);
                    mContext.Response.BinaryWrite(heByte);
                }
            }
            catch {
                mContext.Response.Write("文件异常");
            }
        }
    }
}
