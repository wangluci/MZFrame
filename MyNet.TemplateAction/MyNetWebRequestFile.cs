using MyNet.Middleware.Http;
using System;
using System.IO;
using TemplateAction.Core;

namespace MyNet.TemplateAction
{
    public class MyNetWebRequestFile : IRequestFile
    {
        HttpFile _postfile;
        public MyNetWebRequestFile(HttpFile postfile)
        {
            _postfile = postfile;
        }
        public long ContentLength
        {
            get
            {
                return _postfile.Stream.Length;
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
            return new MemoryStream(_postfile.Stream);
        }

        public void SaveAs(string filename)
        {
            _postfile.SaveAs(filename);
        }
    }
}
