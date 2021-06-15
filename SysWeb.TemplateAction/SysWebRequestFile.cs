using System;
using System.IO;
using System.Web;
using TemplateAction.Core;

namespace SysWeb.TemplateAction
{
    public class SysWebRequestFile : IRequestFile
    {
        HttpPostedFile _postfile;
        public SysWebRequestFile(HttpPostedFile postfile)
        {
            _postfile = postfile;
        }
        public long ContentLength
        {
            get
            {
                return _postfile.ContentLength;
            }
        }

        public string FileName
        {
            get
            {
                return _postfile.FileName;
            }
        }

        public Stream OpenReadStream()
        {
            return _postfile.InputStream;
        }

        public void SaveAs(string filename)
        {
            _postfile.SaveAs(filename);
        }
    }
}
