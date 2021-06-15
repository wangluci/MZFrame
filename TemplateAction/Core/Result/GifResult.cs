using System;
using TemplateAction.Label;

namespace TemplateAction.Core
{
    public class GifResult : IResult
    {
        private byte[] mData;
        protected IRequestHandle mHandle;
        public byte[] Data
        {
            get { return mData; }
        }

        public GifResult(IRequestHandle handle, byte[] pngdata)
        {
            mHandle = handle;
            mData = pngdata;
        }

        public void Output()
        {
            mHandle.Context.Response.ContentType = "image/gif";
            if (mData != null)
            {
                mHandle.Context.Response.BinaryWrite(mData);
            }
        }
    }
}
