using System;
using TemplateAction.Label;

namespace TemplateAction.Core
{
    public class StreamResult : IResult
    {
        private ITAContext _context;
        private byte[] mdata;
        private string mfilename;


        public StreamResult(ITAContext context, string filename, byte[] data)
        {
            _context = context;
            mfilename = filename;
            mdata = data;
        }

        public void Output()
        {
            _context.Response.ContentType = "application/octet-stream";
            _context.Response.AppendHeader("Content-Disposition", "attachment;filename=" + mfilename);
            if (mdata != null)
            {
                _context.Response.BinaryWrite(mdata);
            }
        }
    }
}
