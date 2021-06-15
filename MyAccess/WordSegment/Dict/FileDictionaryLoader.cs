using System;

namespace MyAccess.WordSegment.Dict
{
    class FileDictionaryLoader : DictionaryLoader
    {
        private DateTime _MainDictLastTime;
        public FileDictionaryLoader(string dictDir) : base(dictDir)
        {
            _MainDictLastTime = GetLastTime("Dict.dct");
        }


        private bool MainDictChanged()
        {
            try
            {
                return _MainDictLastTime != GetLastTime("Dict.dct");
            }
            catch
            {
                return false;
            }
        }
        protected override void MonitorMainDict()
        {
            if (MainDictChanged())
            {
                try
                {
                    DictionaryLoader.Lock.Enter(WordSegment.Framework.Lock.Mode.Mutex);
                    Segment._WordDictionary.Load(_DictionaryDir + "Dict.dct");
                    _MainDictLastTime = GetLastTime("Dict.dct");
                }
                finally
                {
                    DictionaryLoader.Lock.Leave();
                }
            }
        }
    }
}
