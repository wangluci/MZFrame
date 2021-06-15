using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace MyAccess.WordSegment.Dict
{
    abstract class DictionaryLoader
    {
        public static Framework.Lock Lock = new WordSegment.Framework.Lock();
        protected string _DictionaryDir;
        private DateTime _ChsSingleLastTime;
        private DateTime _ChsName1LastTime;
        private DateTime _ChsName2LastTime;
        private DateTime _StopWordLastTime;
        private DateTime _SynonymLastTime;
        private DateTime _WildcardLastTime;

        protected Thread _Thread;

        protected DateTime GetLastTime(string fileName)
        {
            return System.IO.File.GetLastWriteTime(_DictionaryDir + fileName);
        }
        private bool ChsNameChanged()
        {
            try
            {
                return (_ChsSingleLastTime != GetLastTime(Dict.ChsName.ChsSingleNameFileName) ||
                    _ChsName1LastTime != GetLastTime(Dict.ChsName.ChsDoubleName1FileName) ||
                    _ChsName2LastTime != GetLastTime(Dict.ChsName.ChsDoubleName2FileName));
            }
            catch
            {
                return false;
            }
        }

        private bool StopWordChanged()
        {
            try
            {
                return _StopWordLastTime != GetLastTime("Stopword.txt");
            }
            catch
            {
                return false;
            }
        }

        private bool SynonymChanged()
        {
            try
            {
                return _SynonymLastTime != GetLastTime(Dict.Synonym.SynonymFileName);
            }
            catch
            {
                return false;
            }
        }

        private bool WildcardChanged()
        {
            try
            {
                return _WildcardLastTime != GetLastTime(Dict.Wildcard.WildcardFileName);
            }
            catch
            {
                return false;
            }

        }

        public DictionaryLoader(string dictDir)
        {
            _DictionaryDir = Framework.Path.AppendDivision(dictDir, '\\');

            _ChsSingleLastTime = GetLastTime(Dict.ChsName.ChsSingleNameFileName);
            _ChsName1LastTime = GetLastTime(Dict.ChsName.ChsDoubleName1FileName);
            _ChsName2LastTime = GetLastTime(Dict.ChsName.ChsDoubleName2FileName);
            _StopWordLastTime = GetLastTime("Stopword.txt");
            _SynonymLastTime = GetLastTime(Dict.Synonym.SynonymFileName);
            _WildcardLastTime = GetLastTime(Dict.Wildcard.WildcardFileName);

            _Thread = new Thread(MonitorDictionary);
            _Thread.IsBackground = true;
            _Thread.Start();
        }
        protected abstract void MonitorMainDict();
        private void MonitorDictionary()
        {
            while (true)
            {
                Thread.Sleep(30000);

                try
                {
                    MonitorMainDict();

                    if (ChsNameChanged())
                    {
                        try
                        {
                            DictionaryLoader.Lock.Enter(WordSegment.Framework.Lock.Mode.Mutex);

                            Segment._ChsName.LoadChsName(_DictionaryDir);
                            _ChsSingleLastTime = GetLastTime(Dict.ChsName.ChsSingleNameFileName);
                            _ChsName1LastTime = GetLastTime(Dict.ChsName.ChsDoubleName1FileName);
                            _ChsName2LastTime = GetLastTime(Dict.ChsName.ChsDoubleName2FileName);
                        }
                        finally
                        {
                            DictionaryLoader.Lock.Leave();
                        }
                    }

                    if (StopWordChanged())
                    {
                        try
                        {
                            DictionaryLoader.Lock.Enter(WordSegment.Framework.Lock.Mode.Mutex);

                            Segment._StopWord.LoadStopwordsDict(_DictionaryDir + "Stopword.txt");
                            _StopWordLastTime = GetLastTime("Stopword.txt");
                        }
                        finally
                        {
                            DictionaryLoader.Lock.Leave();
                        }
                    }

                    if (Segment._Synonym.Inited)
                    {
                        if (SynonymChanged())
                        {
                            try
                            {
                                DictionaryLoader.Lock.Enter(WordSegment.Framework.Lock.Mode.Mutex);

                                Segment._Synonym.Load(_DictionaryDir);
                                _SynonymLastTime = GetLastTime(Dict.Synonym.SynonymFileName);
                            }
                            finally
                            {
                                DictionaryLoader.Lock.Leave();
                            }
                        }
                    }

                    if (Segment._Wildcard.Inited)
                    {
                        if (WildcardChanged())
                        {
                            try
                            {
                                Segment._Wildcard.Load(_DictionaryDir);
                                _WildcardLastTime = GetLastTime(Dict.Wildcard.WildcardFileName);
                            }
                            finally
                            {
                            }
                        }
                    }

                }
                catch
                {
                }
            }
        }
    }
}
