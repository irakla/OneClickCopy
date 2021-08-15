using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml;

namespace OneClickCopy
{
    public class SettingsFileEntry
    {
        private const string FileNameForWindowSettings = "settings.config";
        private readonly string folderNameForSettings;

        private string nowExecutedDirectory = Environment.CurrentDirectory;
        private readonly Window userWindowForErrorMessageBox;

        public SettingsFileEntry(Window userWindow)
        {
            this.userWindowForErrorMessageBox = userWindow;

            string productName =
                Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyProductAttribute>().Product;
            folderNameForSettings = productName + "Setting";

            try
            {
                InitializeWindowSettingContents();
            }
            catch (IOException e)
            {
                string errorDetail = "?";
                switch (e)
                {
                    case DirectoryNotFoundException _:
                        errorDetail = "디렉토리를 찾을 수 없음.";
                        break;
                    case FileNotFoundException _:
                        errorDetail = "파일을 찾을 수 없음.";
                        break;
                    case FileLoadException _:
                        errorDetail = "파일 열기 실패.";
                        break;
                    case PathTooLongException _:
                        errorDetail = "잘못된 파일 경로(Too long)";
                        break;
                }

                MessageBox.Show(userWindow,
                    "환경설정 파일 열기 실패\n" +
                    "원인 : " + errorDetail + '\n' +
                    "파일 경로 : " + FilePathForWindowSettings);

                throw e;
            }
        }

        public string DirectoryPathForSettings
        { get => Path.Combine(nowExecutedDirectory, folderNameForSettings); }

        public string FilePathForWindowSettings
        { get => Path.Combine(DirectoryPathForSettings, FileNameForWindowSettings);    }

        public Configuration WindowSettingFromFile { get; private set; } = null;

        private void InitializeWindowSettingContents()
        {
            if (!Directory.Exists(DirectoryPathForSettings))
            {
                //!!!This work isn't considering Windows Authorization.
                Directory.CreateDirectory(DirectoryPathForSettings);
                MakeNewSettingFile();
            }
            else if (!File.Exists(FilePathForWindowSettings))
            {
                //!!!This work isn't considering Windows Authorization.
                MakeNewSettingFile();
            }

            ExeConfigurationFileMap settingsFileMap = new ExeConfigurationFileMap();
            settingsFileMap.ExeConfigFilename = FilePathForWindowSettings;
            WindowSettingFromFile = ConfigurationManager.OpenMappedExeConfiguration(settingsFileMap, ConfigurationUserLevel.None, false);
        }

        private void MakeNewSettingFile()
        {
            XmlDocument initialSettingsXml = new XmlDocument();
            initialSettingsXml.AppendChild(initialSettingsXml.CreateXmlDeclaration("1.0", "utf-8", null));
            

            XmlNode root = initialSettingsXml.CreateElement("configuration");
            initialSettingsXml.AppendChild(root);

            using (FileStream settingsFileStream = new FileStream(FilePathForWindowSettings, FileMode.CreateNew))
            {
                //!!!This work isn't considering Windows Authorization.
                initialSettingsXml.Save(settingsFileStream);
            }
        }
    }
}
