using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace OneClickCopy
{
    public class MainWindowSettingsController
    {
        private const string SettingKeyLeftOnScreen = "LeftOnScreen";
        private const string SettingKeyTopOnScreen = "TopOnScreen";
        private const string SettingKeyTopmostPinState = "TopmostPinState";
        private const string SettingKeyCanBeTransparent = "CanBeTransparent";
        private const string SettingKeyOpacityAtMouseLeaving = "OpacityAtMouseLeaving";

        private double defaultLeftOnScreen = 100;
        private double defaultTopOnScreen = 100;
        private bool defaultTopmostPinState;
        private bool defaultCanBeTransparent;
        private double defaultOpacityAtMouseLeaving;

        private Configuration targetWindowSettings = null;
        private MainWindow targetWindow;
        private bool isActivatedBatchSave = false;

        public MainWindowSettingsController(MainWindow targetWindow, Configuration initialSettings)
        {
            this.targetWindow = targetWindow;
            this.targetWindowSettings = initialSettings;

            defaultTopmostPinState = targetWindow.TopmostButtonIsPinned;
            defaultCanBeTransparent = targetWindow.CanBeTransparent;
            defaultOpacityAtMouseLeaving = targetWindow.OpacityAtMouseLeaving;
        }

        //Must be considered if you want to do with WindowSettings. because the Settings can be null
        public bool IsLoadedWindowSettings
        {
            get => targetWindowSettings != null;
        }

        public Configuration TargetWindowSettings 
        { get => targetWindowSettings; set => targetWindowSettings = value; }

        public double DefaultLeftOnScreen { get => defaultLeftOnScreen; set => defaultLeftOnScreen = value; }
        public double DefaultTopOnScreen { get => defaultTopOnScreen; set => defaultTopOnScreen = value; }
        

        private KeyValueConfigurationCollection NowSettingsCollection
        {
            get
            {
                if (!IsLoadedWindowSettings)
                    return null;
                else
                    return targetWindowSettings.AppSettings.Settings;
            }
        }

        public void MoveLeftOnScreen(double newLeftOnScreen)
        {
            targetWindow.Left = newLeftOnScreen;

            if(IsLoadedWindowSettings)
                NowSettingsCollection[SettingKeyLeftOnScreen].Value = newLeftOnScreen.ToString();

            if (!isActivatedBatchSave)
                targetWindowSettings.Save(ConfigurationSaveMode.Modified);
        }

        public void MoveTopOnScreen(double newTopOnScreen)
        {
            targetWindow.Top = newTopOnScreen;

            if (IsLoadedWindowSettings)
                NowSettingsCollection[SettingKeyTopOnScreen].Value = newTopOnScreen.ToString();

            if (!isActivatedBatchSave)
                targetWindowSettings.Save(ConfigurationSaveMode.Modified);
        }

        public void SetTopmostState(bool stateIsPinned)
        {
            targetWindow.TopmostButtonIsPinned = stateIsPinned;

            if (IsLoadedWindowSettings)
                NowSettingsCollection[SettingKeyTopmostPinState].Value = stateIsPinned.ToString();

            if (!isActivatedBatchSave)
                targetWindowSettings.Save(ConfigurationSaveMode.Modified);
        }

        public void SetCanBeTransparent(bool stateCanBeTransParent)
        {
            targetWindow.CanBeTransparent = stateCanBeTransParent;

            if (IsLoadedWindowSettings)
                NowSettingsCollection[SettingKeyCanBeTransparent].Value = stateCanBeTransParent.ToString();

            if (!isActivatedBatchSave)
                targetWindowSettings.Save(ConfigurationSaveMode.Modified);
        }

        public void SetOpacityAtMouseLeaving(double newOpacity)
        {
            targetWindow.OpacityAtMouseLeaving = newOpacity;

            if (IsLoadedWindowSettings)
                NowSettingsCollection[SettingKeyOpacityAtMouseLeaving].Value = newOpacity.ToString();

            if (!isActivatedBatchSave)
                targetWindowSettings.Save(ConfigurationSaveMode.Modified);
        }

        public void ApplyAllCurrentSettings()
        {
            double? settingLeftOnScreen = GetNullableDoubleSetting(SettingKeyLeftOnScreen, defaultLeftOnScreen);
            if (settingLeftOnScreen != null)
                MoveLeftOnScreen((double)settingLeftOnScreen);

            double? settingTopOnScreen = GetNullableDoubleSetting(SettingKeyTopOnScreen, defaultTopOnScreen);
            if (settingTopOnScreen != null)
                MoveTopOnScreen((double)settingTopOnScreen);

            bool? settingTopmostState = GetNullableBoolSetting(SettingKeyTopmostPinState, defaultTopmostPinState);
            if (settingTopmostState != null)
                SetTopmostState((bool)settingTopmostState);

            bool? settingCanBeTransparent = GetNullableBoolSetting(SettingKeyCanBeTransparent, defaultCanBeTransparent);
            if (settingCanBeTransparent != null)
                SetCanBeTransparent((bool)settingCanBeTransparent);

            double? settingOpacityAtMouseLeaving = GetNullableDoubleSetting(SettingKeyOpacityAtMouseLeaving, defaultOpacityAtMouseLeaving);
            if (settingOpacityAtMouseLeaving != null)
                SetOpacityAtMouseLeaving((double)settingOpacityAtMouseLeaving);
        }

        private double? GetNullableDoubleSetting(string settingKey, double? defaultValue = null)
        {
            if (!IsLoadedWindowSettings)
                return null;

            string settingValueString = NowSettingsCollection[settingKey]?.Value;
            bool isNotFoundFromTheCollection = settingValueString == null;
            if(isNotFoundFromTheCollection)
            {
                if (defaultValue != null)
                    AddNewWindowSetting(settingKey, defaultValue.ToString());

                return defaultValue;
            }

            return Convert.ToDouble(settingValueString);
        }

        private bool? GetNullableBoolSetting(string settingKey, bool? defaultValue = null)
        {
            if (!IsLoadedWindowSettings)
                return null;

            string settingValueString = NowSettingsCollection[settingKey]?.Value;
            bool isNotFoundFromTheCollection = settingValueString == null;
            if (isNotFoundFromTheCollection)
            {
                if (defaultValue != null)
                    AddNewWindowSetting(settingKey, defaultValue.ToString());

                return defaultValue;
            }

            return Convert.ToBoolean(settingValueString);
        }

        private void AddNewWindowSetting(string settingKey, string settingValue)
        {
            if (IsLoadedWindowSettings)
                NowSettingsCollection.Add(settingKey, settingValue);

            if (!isActivatedBatchSave)
                targetWindowSettings.Save(ConfigurationSaveMode.Modified);
        }
    }
}
