using MyAccess.WordSegment;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace MZWordSeg
{
    class Program
    {
        public static void Segment(string source, string result)
        {
            MyAccess.WordSegment.Segment.Init();
            MyAccess.WordSegment.Segment segment = new MyAccess.WordSegment.Segment();

            using (var reader = new StreamReader(source))
            {
                using (var writer = new StreamWriter(result,false,Encoding.UTF8))
                {
                    bool isend = false;
                    string preline = "";
                    while (true)
                    {
                        var line = reader.ReadLine();
                        if (line == null)
                        {
                            isend = true;
                        }
                        else
                        {
                            if (line.IndexOf('\t') <= 0)
                            {
                                preline += line;
                                continue;
                            }

                        }
                        var parts = preline.Split('\t', StringSplitOptions.RemoveEmptyEntries);
                        preline = line;
                        if (parts.Length != 2)
                        {
                            continue;
                        }
                        ICollection<WordInfo> words = segment.DoSegment(parts[1]);
                        StringBuilder segments = new StringBuilder();
                        foreach (WordInfo w in words)
                        {
                            segments.AppendFormat("{0} ", w.Word);
                        }
                        string tmpsegcode = segments.ToString().Trim();
                        writer.WriteLine("{0}\t{1}", parts[0], tmpsegcode);

                        if (isend)
                        {
                            break;
                        }
                    }
                }
            }
        }
        static void Main(string[] args)
        {
            Segment(@"E:\tmpcode.txt", @"E:\xxcode.txt");
        }
    }
}
