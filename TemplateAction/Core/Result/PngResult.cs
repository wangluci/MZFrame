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
        private ITAContext _context;
        public byte[] Data
        {
            get { return mData; }
        }

        public PngResult(ITAContext context, byte[] pngdata)
        {
            _context = context;
            mData = pngdata;
        }

        public void Output()
        {
            _context.Response.ContentType = "image/png";
            if (mData != null)
            {
                _context.Response.BinaryWrite(mData);
            }
        }
    }
}
