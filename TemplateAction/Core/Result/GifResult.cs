using System;
using TemplateAction.Label;

namespace TemplateAction.Core
{
    public class GifResult : IResult
    {
        private byte[] mData;
        private ITAContext _context;
        public byte[] Data
        {
            get { return mData; }
        }

        public GifResult(ITAContext context, byte[] pngdata)
        {
            _context = context;
            mData = pngdata;
        }

        public void Output()
        {
            _context.Response.ContentType = "image/gif";
            if (mData != null)
            {
                _context.Response.BinaryWrite(mData);
            }
        }
    }
}
