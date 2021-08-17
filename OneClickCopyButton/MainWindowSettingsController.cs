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
        private const string SettingKeyWindowWidth = "WindowWidth";
        private const string SettingKeyWindowHeight = "WindowHeight";
        private const string SettingKeyTopmostPinState = "TopmostPinState";
        private const string SettingKeyCanBeTransparent = "CanBeTransparent";
        private const string SettingKeyOpacityAtMouseLeaving = "OpacityAtMouseLeaving";

        private double defaultLeftOnScreen = 100;
        private double defaultTopOnScreen = 100;
        private double defaultWindowWidth = 300;
        private double defaultWindowHeight = 300;
        private bool defaultTopmostPinState;
        private bool defaultCanBeTransparent;
        private double defaultOpacityAtMouseLeaving;

        private Configuration targetWindowSettings = null;
        private MainWindow targetWindow;

        public Configuration TargetWindowSettings
        { get => targetWindowSettings; set => targetWindowSettings = value; }

        public double DefaultLeftOnScreen { get => defaultLeftOnScreen; set => defaultLeftOnScreen = value; }
        public double DefaultTopOnScreen { get => defaultTopOnScreen; set => defaultTopOnScreen = value; }
        public double DefaultWindowWidth { get => defaultWindowWidth; set => defaultWindowWidth = value; }
        public double DefaultWindowHeight { get => defaultWindowHeight; set => defaultWindowHeight = value; }

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

            targetWindowSettings.Save(ConfigurationSaveMode.Modified);
        }

        public void MoveTopOnScreen(double newTopOnScreen)
        {
            targetWindow.Top = newTopOnScreen;

            if (IsLoadedWindowSettings)
                NowSettingsCollection[SettingKeyTopOnScreen].Value = newTopOnScreen.ToString();

            targetWindowSettings.Save(ConfigurationSaveMode.Modified);
        }

        public void SetNewPositionOnScreen(Point newPointOnScreen)
        {
            if (IsLoadedWindowSettings)
            {
                NowSettingsCollection[SettingKeyLeftOnScreen].Value = newPointOnScreen.X.ToString();
                NowSettingsCollection[SettingKeyTopOnScreen].Value = newPointOnScreen.Y.ToString();
            }

            targetWindowSettings.Save(ConfigurationSaveMode.Modified);
        }

        public void SetNewWindowSize(Size newWindowSize)
        {
            if (IsLoadedWindowSettings)
            {
                NowSettingsCollection[SettingKeyWindowWidth].Value = newWindowSize.Width.ToString();
                NowSettingsCollection[SettingKeyWindowHeight].Value = newWindowSize.Height.ToString();
            }

            targetWindowSettings.Save(ConfigurationSaveMode.Modified);
        }

        public void SetTopmostState(bool stateIsPinned)
        {
            if (IsLoadedWindowSettings)
                NowSettingsCollection[SettingKeyTopmostPinState].Value = stateIsPinned.ToString();

            targetWindowSettings.Save(ConfigurationSaveMode.Modified);
        }

        public void SetCanBeTransparent(bool stateCanBeTransParent)
        {
            if (IsLoadedWindowSettings)
                NowSettingsCollection[SettingKeyCanBeTransparent].Value = stateCanBeTransParent.ToString();

            targetWindowSettings.Save(ConfigurationSaveMode.Modified);
        }

        public void SetOpacityAtMouseLeaving(double newOpacity)
        {
            if (IsLoadedWindowSettings)
                NowSettingsCollection[SettingKeyOpacityAtMouseLeaving].Value = newOpacity.ToString();

            targetWindowSettings.Save(ConfigurationSaveMode.Modified);
        }

        public void ApplyAllCurrentSettings()
        {
            double? settingLeftOnScreen = GetNullableDoubleSetting(SettingKeyLeftOnScreen, defaultLeftOnScreen);
            double? settingTopOnScreen = GetNullableDoubleSetting(SettingKeyTopOnScreen, defaultTopOnScreen);
            if (settingLeftOnScreen != null && settingTopOnScreen != null)
                targetWindow.NowPositionOnScreen 
                    = new Point((double)settingLeftOnScreen, (double)settingTopOnScreen);

            double? settingWindowWidth = GetNullableDoubleSetting(SettingKeyWindowWidth, defaultWindowWidth);
            double? settingWindowHeight = GetNullableDoubleSetting(SettingKeyWindowHeight, defaultWindowHeight);
            if (settingWindowWidth != null && settingWindowHeight != null)
                targetWindow.NowWindowSize
                    = new Size((double)settingWindowWidth, (double)settingWindowHeight);

            bool? settingTopmostState = GetNullableBoolSetting(SettingKeyTopmostPinState, defaultTopmostPinState);
            if (settingTopmostState != null)
                targetWindow.TopmostButtonIsPinned = (bool)settingTopmostState;

            bool? settingCanBeTransparent = GetNullableBoolSetting(SettingKeyCanBeTransparent, defaultCanBeTransparent);
            if (settingCanBeTransparent != null)
                targetWindow.CanBeTransparent = (bool)settingCanBeTransparent;

            double? settingOpacityAtMouseLeaving = GetNullableDoubleSetting(SettingKeyOpacityAtMouseLeaving, defaultOpacityAtMouseLeaving);
            if (settingOpacityAtMouseLeaving != null)
                targetWindow.OpacityAtMouseLeaving = (double)settingOpacityAtMouseLeaving;
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

            targetWindowSettings.Save(ConfigurationSaveMode.Modified);
        }
    }
}
