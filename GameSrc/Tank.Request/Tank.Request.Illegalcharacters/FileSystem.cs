using System.Collections;
using System.IO;
using System.Text;

namespace Tank.Request.Illegalcharacters
{
    public class FileSystem
    {
        public ArrayList contentList = new ArrayList();
        private FileSystemWatcher fileWatcher = new FileSystemWatcher();
        private string filePath = string.Empty;
        private string fileDirectory = string.Empty;
        private string fileType = string.Empty;

        public FileSystem(string Path, string Directory, string Type)
        {
            this.initContent(Path);
            this.initFileWatcher(Directory, Type);
        }

        private void initContent(string Path)
        {
            if (!File.Exists(Path))
                return;
            this.filePath = Path;
            StreamReader streamReader = new StreamReader(Path, Encoding.GetEncoding("GB2312"));
            string str = "";
            if (this.contentList.Count > 0)
                this.contentList.Clear();
            while (str != null)
            {
                str = streamReader.ReadLine();
                if (!string.IsNullOrEmpty(str))
                    this.contentList.Add((object)str);
            }
            if (str != null)
                return;
            streamReader.Close();
        }

        private void initFileWatcher(string directory, string type)
        {
            if (!Directory.Exists(directory))
                return;
            this.fileDirectory = directory;
            this.fileType = type;
            this.fileWatcher.Path = directory;
            this.fileWatcher.Filter = type;
            this.fileWatcher.NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastWrite | NotifyFilters.LastAccess;
            this.fileWatcher.EnableRaisingEvents = true;
            this.fileWatcher.Changed += new FileSystemEventHandler(this.OnChanged);
            this.fileWatcher.Renamed += new RenamedEventHandler(FileSystem.OnRenamed);
        }

        public bool checkIllegalChar(string strRegName)
        {
            bool flag = false;
            if (!string.IsNullOrEmpty(strRegName))
                flag = this.checkChar(strRegName);
            return flag;
        }

        private bool checkChar(string strRegName)
        {
            bool flag = false;
            foreach (string content in this.contentList)
            {
                if (!content.StartsWith("GM"))
                {
                    foreach (char ch in content)
                    {
                        if (strRegName.Contains(ch.ToString()))
                        {
                            flag = true;
                            break;
                        }
                    }
                    if (flag)
                        break;
                }
                else
                {
                    string str1 = content;
                    char[] chArray = new char[1] { '|' };
                    foreach (string str2 in str1.Split(chArray))
                    {
                        if (strRegName.Contains(str2))
                        {
                            flag = true;
                            break;
                        }
                    }
                    if (flag)
                        break;
                }
            }
            return flag;
        }

        private void OnChanged(object source, FileSystemEventArgs e)
        {
            this.UpdataContent();
        }

        private void UpdataContent()
        {
            this.initContent(this.filePath);
        }

        private static void OnRenamed(object source, RenamedEventArgs e)
        {
        }
    }
}