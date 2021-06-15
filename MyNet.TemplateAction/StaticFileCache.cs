using System;
using System.IO;
using TemplateAction.Cache;
using MyNet.Common;
namespace MyNet.TemplateAction
{
    public class StaticFileCache
    {
        private static StaticFileCache _instance = new StaticFileCache();
        public static StaticFileCache Instance
        {
            get { return _instance; }
        }
        private CachePool _cachepool = new CachePool();
        private FileDependencyWatcher _watcher;
        public StaticFileCache()
        {
            _watcher = new FileDependencyWatcher(AppDomain.CurrentDomain.BaseDirectory);
        }
        public byte[] GetGZipStaticFile(string path, int maxcache)
        {
            byte[] filedata = _cachepool.Get(path) as byte[];
            if (filedata == null)
            {
                if (File.Exists(path))
                {
                    FileStream fs = new FileStream(path, FileMode.Open);//初始化文件流
                    filedata = new byte[fs.Length];//初始化字节数组
                    fs.Read(filedata, 0, filedata.Length);//读取流中数据到字节数组中
                    fs.Close();
                    byte[] gzipbytes = Compression.GZipCompress(filedata);
                    if (gzipbytes.Length <= maxcache)
                    {
                        FileDependency fdep = _watcher.CreateFileDependency(path);
                        if (fdep != null)
                        {
                            _cachepool.Insert(path, gzipbytes, fdep);
                        }
                    }
                    return gzipbytes;
                }
            }
            return filedata;
        }
    }
}
