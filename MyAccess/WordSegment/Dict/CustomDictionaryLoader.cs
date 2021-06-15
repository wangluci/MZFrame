using System;
namespace MyAccess.WordSegment.Dict
{
    /// <summary>
    /// 自定义字典加载
    /// </summary>
    class CustomDictionaryLoader : DictionaryLoader
    {
        private ICustomSource _source;
        public CustomDictionaryLoader(ICustomSource source, string dictDir) :base(dictDir)
        {
            _source = source;
        }

        protected override void MonitorMainDict()
        {
            if (_source.DictFromChanged())
            {
                try
                {
                    DictionaryLoader.Lock.Enter(WordSegment.Framework.Lock.Mode.Mutex);
                    Segment._WordDictionary.Load(_source.DictFrom());
                }
                finally
                {
                    DictionaryLoader.Lock.Leave();
                }
            }
        }
    }
}
