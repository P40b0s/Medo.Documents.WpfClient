using NLog;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Threading;
using System.Windows;

namespace Medo.Client.Updater
{
    public class Update : INotifyPropertyChanged
    {
        readonly Logger logger = LogManager.GetCurrentClassLogger();
        public Model.UpdaterModel XmlCurrentVesion = new Model.UpdaterModel();
        public Model.UpdaterModel XmlRelaseVersion = new Model.UpdaterModel();
        private string BaseDir;
        private string RootDir;
        private bool IsClientLaunch = false;
        #region Notify
        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName]string propertyName = null)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion
        AppDomain currentDomain = AppDomain.CurrentDomain;
        private List<FileInfo> assemblyList = new List<FileInfo>();
        public Update()
        {
            StarupArgs();
            WaitClientExit(); 
        }
        
        private void Initialize()
        {
            BaseDir = currentDomain.BaseDirectory.Replace("\\Updater\\", "");
            RootDir = new FileInfo(XmlRelaseVersion.UpdatePath.Remove(XmlRelaseVersion.UpdatePath.Length - 1, 1)).Name;
            Deserilize();
            UpdateDlls(XmlCurrentVesion.Version, XmlRelaseVersion.Version);
            InfoText = "Обновление...";
        }
        private async void WaitClientExit()
        {
            if(IsClientLaunch)
            {
                InfoText = "Ожидание завершения процесса Medo.Client.exe...";
                var client = Process.GetProcessesByName("Medo.Client").FirstOrDefault();
                if (client!= null)
                {
                    await Task.Factory.StartNew(() =>
                    {
                        while (!client.HasExited)
                        {
                            Thread.Sleep(100);
                        }
                        Initialize();
                    });
                }
                else
                {
                    Initialize();
                }
            }
            else
            {
                Initialize();
            }
          
        }

        private void StarupArgs()
        {
            string[] args = Environment.GetCommandLineArgs();
            foreach (string arg in args)
            {
                if (arg.Contains("-client"))
                {
                    IsClientLaunch = true;
                }
            }
        }

        void Deserilize()
        {
            try
            {
                System.Xml.Serialization.XmlSerializer ser = new System.Xml.Serialization.XmlSerializer(typeof(Model.UpdaterModel));

                using (TextReader reader = new StreamReader(Path.Combine(XmlRelaseVersion.UpdatePath, "version.xml")))
                {
                    XmlRelaseVersion = (Model.UpdaterModel)ser.Deserialize(reader);
                }
                using (TextReader reader = new StreamReader(Path.Combine(BaseDir, "version.xml")))
                {
                    XmlCurrentVesion = (Model.UpdaterModel)ser.Deserialize(reader);
                }
            }
            catch (System.Exception ex)
            {
                logger.Fatal(ex);
                //UpdateDlls(null, XmlRelaseVersion.Version);
            }
        }

        private async void UpdateDlls(string currver, string relver)
        {
            try
            {
                Process client = new Process();
                client.StartInfo.FileName = Path.Combine(BaseDir, "Medo.Client.exe");
                client.StartInfo.WorkingDirectory = BaseDir;
                if (string.IsNullOrEmpty(currver) || relaseVersionIsNewestThanCurrentVersion(currver, relver))
                {
                    if (await UpdateDllsAsync())
                    {
                        client.StartInfo.Arguments = "-updateok" + relver;
                    }
                    else
                    {
                        client.StartInfo.Arguments = "-updatefalse";
                    }
                }              
                client.Start();
                await Application.Current.Dispatcher.BeginInvoke(new Action(() => { App.Current.Shutdown(); }));
            }
            catch (System.Exception ex)
            {
                logger.Fatal(ex);
                await Application.Current.Dispatcher.BeginInvoke(new Action(() => { App.Current.Shutdown(); }));
            }

        }


        #region ProgressBarValues
        public double MaximumFiles
        {
            get
            {
                return this._MaximumFiles;
            }
            set
            {
                if (this.MaximumFiles != value)
                {
                    this._MaximumFiles = value;
                    OnPropertyChanged();
                }
            }
        }
        private double _MaximumFiles { get; set; }

        public double CurrentUpdateFile
        {
            get
            {
                return this._CurrentUpdateFile;
            }
            set
            {
                if (this.CurrentUpdateFile != value)
                {
                    this._CurrentUpdateFile = value;
                    OnPropertyChanged();
                }
            }
        }
        private double _CurrentUpdateFile { get; set; }
        #endregion

        #region Textblock values
        public string UpdateFile
        {
            get
            {
                return this._UpdateFile;
            }
            set
            {
                if (this.UpdateFile != value)
                {
                    this._UpdateFile = value;
                    OnPropertyChanged();
                }
            }
        }
        private string _UpdateFile { get; set; }

        public string InfoText
        {
            get
            {
                return this._InfoText;
            }
            set
            {
                if (this.InfoText != value)
                {
                    this._InfoText = value;
                    OnPropertyChanged();
                }
            }
        }
        private string _InfoText { get; set; }
        #endregion

        private bool relaseVersionIsNewestThanCurrentVersion(string curversion, string relversion)
        {
            try
            {
                if (!string.IsNullOrEmpty(curversion) && !string.IsNullOrEmpty(relversion))
                {

                    double rvdouble = double.Parse(relversion.Replace(".", ""));
                    double cvdouble = double.Parse(curversion.Replace(".", ""));
                    if (rvdouble > cvdouble)
                        return true;
                    else
                        return false;
                }
                else
                    return false;
            }
            catch (System.Exception ex)
            {
                logger.Fatal(ex);
                return false;
            }
        }

        public async Task<bool> UpdateDllsAsync()
        {
            return await Task<bool>.Factory.StartNew(() =>
            {
                try
                {
                    MaximumFiles = XmlRelaseVersion.Modules.Count;
                    string[] subdirs = XmlRelaseVersion.Modules.Select(s => s.Subdir).Where(s => !string.IsNullOrEmpty(s)).Distinct().ToArray();
                    foreach (string sdir in subdirs)
                    {
                        Directory.CreateDirectory(Path.Combine(BaseDir, sdir));
                    }
                    Directory.CreateDirectory(Path.Combine(BaseDir, "Logs"));
                    for (int i = 0; i < XmlRelaseVersion.Modules.Count; i++)
                    {

                        FileInfo file = new FileInfo(XmlRelaseVersion.Modules[i].Path);
                        CurrentUpdateFile = i;
                        UpdateFile = file.Name;

                        int index = file.FullName.IndexOf(RootDir);
                        if (index != -1)
                        {
                            //прибавляем к индексу 1 для удаления - \
                            string path = file.FullName.Remove(0, index + RootDir.Length + 1);
                            string[] p = path.Split('\\');
                            if (p.Count() > 1)
                            {
                                Copy(file.FullName, Path.Combine(BaseDir, path));
                            }
                            else
                            {
                                Copy(file.FullName, Path.Combine(BaseDir, file.Name));
                            }
                        }



                    }
                    //Copy(Path.Combine(XmlSourceVersion.UpdatePath, "version.xml"), Path.Combine(BaseDir, "version.xml"));
                    //CurrentUpdateFile++;
                    //UpdateFile = "version.xml";
                    return true;
                }
                catch (System.Exception ex)
                {
                    logger.Fatal(ex);
                    return false;
                }
            });
        }



        /// <summary>
        /// Побитовое копирование файлов
        /// </summary>
        /// <param name="sourceFilePath">Откуда копируем</param>
        /// <param name="destFilePath">Куда копируем</param>
        private void Copy(string sourceFilePath, string destFilePath)
        {
            try
            {
                byte[] buffer = new byte[1024 * 1024];
                using (FileStream source = new FileStream(sourceFilePath, FileMode.Open, FileAccess.Read))
                {
                    long fileLenght = source.Length;
                    using (FileStream dest = new FileStream(destFilePath, FileMode.Create, FileAccess.Write))
                    {
                        long totalBytes = 0;
                        int currentBlockSize = 0;
                        while ((currentBlockSize = source.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            totalBytes += currentBlockSize;
                            // CopyProgressByte = Math.Round(((double)totalBytes * 100 / fileLenght), 0);
                            dest.Write(buffer, 0, currentBlockSize);
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                logger.Fatal(ex);
                CurrentUpdateFile--;
            }

        }
    }
}
