using System;
using System.Collections.Generic;
using System.Text;

namespace MyAccess.WordSegment.Setting
{
    class SettingLoader
    {
        private void Load(string fileName)
        {
            PanGuSettings.Load(fileName);
        }

        public SettingLoader(string fileName)
        {
            Load(fileName);
        }

        public SettingLoader()
        {
            string fileName = WordSegment.Framework.Path.GetAssemblyPath() + "PanGu.xml";
            Load(fileName);
        }
    }
}
