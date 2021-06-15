using System;
using TemplateAction.Label;

namespace TemplateAction.Core
{
    /// <summary>
    /// Png图片结果
    /// </summary>
    public class PngResult : IResult
    {
        private byte[] mData;
        protected IRequestHandle mHandle;
        public byte[] Data
        {
            get { return mData; }
        }

        public PngResult(IRequestHandle handle, byte[] pngdata)
        {
            mHandle = handle;
            mData = pngdata;
        }

        public void Output()
        {
            mHandle.Context.Response.ContentType = "image/png";
            if (mData != null)
            {
                mHandle.Context.Response.BinaryWrite(mData);
            }
        }
    }
}
