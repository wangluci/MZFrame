using System;
using System.IO;
using ICSharpCode.SharpZipLib.Zip;
namespace MyAccess.Core
{
    public class ZipHelper
    {
        public static bool UnZip(string sourceFile, string destFile)
        {
            ZipInputStream s = new ZipInputStream(File.OpenRead(sourceFile));
            try
            {
                ZipEntry theEntry;
                while ((theEntry = s.GetNextEntry()) != null)
                {
                    string fileName = Path.GetFileName(theEntry.Name);
                    string directoryName = destFile + "\\" + Path.GetDirectoryName(theEntry.Name);
                    //生成解压目录
                    Directory.CreateDirectory(directoryName);

                    if (fileName != String.Empty)
                    {
                        //解压文件到指定的目录
                        FileStream streamWriter = File.Create(directoryName + "\\" + fileName);

                        int size = 2048;
                        byte[] data = new byte[2048];
                        while (true)
                        {
                            size = s.Read(data, 0, data.Length);
                            if (size > 0)
                            {
                                streamWriter.Write(data, 0, size);
                            }
                            else
                            {
                                break;
                            }
                        }

                        streamWriter.Close();
                    }
                }
                s.Close();
            }
            catch
            {
                return false;
            }
            return true;
        }


        public static bool ZipFile(string FileToZip, string ZipedFile, int CompressionLevel, int BlockSize, string password)
        {
            try
            {
                //如果文件没有找到，则报错
                if (!System.IO.File.Exists(FileToZip))
                {
                    throw new System.IO.FileNotFoundException("指定的文件" + FileToZip + "未找到，压缩中止！");
                }

                System.IO.FileStream StreamToZip = new System.IO.FileStream(FileToZip, System.IO.FileMode.Open, System.IO.FileAccess.Read);
                System.IO.FileStream ZipFile = System.IO.File.Create(ZipedFile);
                ZipOutputStream ZipStream = new ZipOutputStream(ZipFile);
                ZipEntry ZipEntry = new ZipEntry("ZippedFile");
                ZipStream.PutNextEntry(ZipEntry);
                ZipStream.SetLevel(CompressionLevel);
                byte[] buffer = new byte[BlockSize];
                System.Int32 size = StreamToZip.Read(buffer, 0, buffer.Length);
                ZipStream.Write(buffer, 0, size);

                while (size < StreamToZip.Length)
                {
                    int sizeRead = StreamToZip.Read(buffer, 0, buffer.Length);
                    ZipStream.Write(buffer, 0, sizeRead);
                    size += sizeRead;
                }

                ZipStream.Finish();
                ZipStream.Close();
                StreamToZip.Close();
                return true;
            }
            catch 
            {
                return false;
            }
        }
        public static bool ZipFile(string[] files, string zipFilePath)
        {
            try
            {
                string[] strArray = files;
                using (ZipOutputStream stream = new ZipOutputStream(File.Create(zipFilePath)))
                {
                    stream.SetLevel(9);
                    byte[] buffer = new byte[0x1000];
                    foreach (string str in strArray)
                    {
                        ZipEntry entry = new ZipEntry(Path.GetFileName(str));
                        entry.DateTime = DateTime.Now;
                        stream.PutNextEntry(entry);
                        using (FileStream stream2 = File.OpenRead(str))
                        {
                            int num;
                            do
                            {
                                num = stream2.Read(buffer, 0, buffer.Length);
                                stream.Write(buffer, 0, num);
                            }
                            while (num > 0);
                        }
                    }
                    stream.Finish();
                    stream.Close();
                }
            }
            catch
            {
                return false;
            }
            return true;
        }
    }
}
