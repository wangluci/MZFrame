using System;
using TemplateAction.Label;

namespace TemplateAction.Core
{
    public class StreamResult : IResult
    {
        private ITAAction mHandle;
        private byte[] mdata;
        private string mfilename;


        public StreamResult(ITAAction handle,string filename, byte[] data)
        {
            mHandle = handle;
            mfilename = filename;
            mdata = data;
        }

        public void Output()
        {
            mHandle.Context.Response.ContentType = "application/octet-stream";
            mHandle.Context.Response.AppendHeader("Content-Disposition", "attachment;filename=" + mfilename);
            if (mdata != null)
            {
                mHandle.Context.Response.BinaryWrite(mdata);
            }
        }
    }
}
