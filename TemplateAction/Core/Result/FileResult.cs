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
        private IRequestHandle mHandle;
        public string Path
        {
            get { return mPath; }
        }

        public FileResult(IRequestHandle handle, string path)
        {
            mHandle = handle;
            mPath = path;
        }

        public void Output()
        {
            try
            {
                string ext = System.IO.Path.GetExtension(mPath);
                mHandle.Context.Response.ContentType = FileContentType.GetMimeType(ext);
                using (FileStream fsRead = new FileStream(mPath, FileMode.OpenOrCreate))
                {
                    byte[] heByte = new byte[fsRead.Length];
                    fsRead.Read(heByte, 0, heByte.Length);
                    mHandle.Context.Response.BinaryWrite(heByte);
                }
            }
            catch {
                mHandle.Context.Response.Write("文件异常");
            }
        }
    }
}
