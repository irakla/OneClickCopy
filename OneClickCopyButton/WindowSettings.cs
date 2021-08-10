using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OneClickCopyButton
{
    public class WindowSettings
    {
        private MainWindow targetWindow;
        private Dictionary<string, string> settings = new Dictionary<string, string>();

        //default
        public WindowSettings(MainWindow targetWindow)
        {
            this.targetWindow = targetWindow;
        }

        public Dictionary<string, string> NowSettingsInfo
        {
            get => settings;
        }

        public void AddSetting(string settingKeyName, string settingValue)
        {
            if (NowSettingsInfo.ContainsKey(settingKeyName))
                throw new SettingExistAlreadyException(settingKeyName, NowSettingsInfo[settingKeyName]);

            NowSettingsInfo.Add(settingKeyName, settingValue);
        }

        public void AddSetting(string settingKeyName, int settingValue) => AddSetting(settingKeyName, settingValue.ToString());
        public void AddSetting(string settingKeyName, double settingValue) => AddSetting(settingKeyName, settingValue.ToString());

        public void ChangeSetting(string settingKeyName, string settingNewValue)
        {
            if (NowSettingsInfo.ContainsKey(settingKeyName))
                throw new SettingIsNotExistException(settingKeyName);

            NowSettingsInfo[settingKeyName] = settingNewValue;
        }

        public class SettingExistAlreadyException : Exception
        {
            public SettingExistAlreadyException(string settingKeyName, string settingNowValue)
            {
                this.SettingKeyName = settingKeyName;
                this.SettingNowValue = settingNowValue;
            }

            public string SettingKeyName { get; set; }
            public string SettingNowValue { get; set; }
        }

        public class SettingIsNotExistException : Exception
        {
            public SettingIsNotExistException(string settingKeyname)
            {
                this.SettingKeyName = settingKeyname;
            }

            public string SettingKeyName { get; set; }
        }
    }
}
