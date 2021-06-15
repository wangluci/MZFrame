using System;
using System.IO;

namespace TemplateAction.Core
{
    public interface IRequestFile
    {
        long ContentLength { get; }
        string FileName { get; }
        Stream OpenReadStream();
        void SaveAs(string filename);
    }
}
