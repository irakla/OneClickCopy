using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace OneClickCopy
{
    public class SettingFileEntry
    {
        private const string SettingFileFolderName = @"\setting";
        private const string SettingFileName = "setting.txt";

        private string nowExecutedDirectory;
        private string settingFilePath;
        private readonly Window userWindow;
        private WindowSettings nowWindowSettings;

        public SettingFileEntry(Window userWindow, string nowExecutedDirectory)
        {
            this.userWindow = userWindow;
            this.nowExecutedDirectory = nowExecutedDirectory;
            SettingFilePath = nowExecutedDirectory;

            try
            {
                InitializeSettingFileContents();
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
                    "파일 경로 : " + SettingFilePath);

                throw e;
            }
        }

        public string SettingFileDirectoryPath
        {
            get => SettingFileDirectoryPath;
            private set => SettingFileDirectoryPath = value + SettingFileFolderName;
        }
        //if it set by exe file's path, it will be Setting File's path with the file's path.
        public string SettingFilePath
        {
            get => settingFilePath;
            private set => settingFilePath = value + SettingFileFolderName + @"\" + SettingFileName;
        }

        private void InitializeSettingFileContents()
        {
            string oneLine;
            System.IO.StreamReader file = new System.IO.StreamReader(settingFilePath);
            while ((oneLine = file.ReadLine()) != null)
            {
                Console.WriteLine(oneLine); //for test
            }

            file.Close();
        }

        private void MakeNewSettingFile()
        {
            try
            {
                MakeSettingFileDirectory();
                MakeNewSettingFile();
            }
            catch (IOException e)
            {
                //TODO : 새 환경설정파일 만드는 도중 io예외발생 처리
            }
        }

        private void MakeSettingFileDirectory()
        {
            DirectoryInfo settingFileDI = new DirectoryInfo(SettingFileDirectoryPath);

            if (settingFileDI.Exists == false)
                settingFileDI.Create();
        }

        private void MakeSettingFile()
        {

        }
    }
}
