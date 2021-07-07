using System;
using System.IO;
using System.Web;
using System.Web.Hosting;

namespace SysWeb.TemplateAction
{
    public class AssetsFile : VirtualFile
    {
        private byte[] _data;
        private string _path;
        public AssetsFile(string virtualPath, byte[] data) : base(virtualPath)
        {
            _path = VirtualPathUtility.ToAppRelative(virtualPath);
            _data = data;
        }
        public override Stream Open()
        {
           return new MemoryStream(_data);
        }
    }
}
