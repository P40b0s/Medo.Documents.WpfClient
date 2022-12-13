using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medo.Modules.ClientUpdateModule
{
    class BGFilesCopy
    {
        readonly Logger logger = LogManager.GetCurrentClassLogger();
        public string localUpdatePath = "";

        public BGFilesCopy()
        {
            localUpdatePath = new Uri(this.GetType().Module.Assembly.CodeBase).LocalPath.ToLower().Replace("clientupdatemodule.dll", "") + "Updates";
        }

        public async Task<bool> BackgroundCopyAsync(IEnumerable<Client.Updater.Model.ModuleVersion> modules)
        {
            return await Task<bool>.Factory.StartNew(() =>
            {
                try
                {
                   
                    List<string> directories = modules.Select(s => s.Subdir).Where(s => s != null).ToList();
                    directories.Add("Logs");
                    foreach (string dir in directories)
                    {
                        Directory.CreateDirectory(Path.Combine(localUpdatePath, dir));
                    }
                    foreach (Client.Updater.Model.ModuleVersion module in modules)
                    {
                        string subdir = string.Empty;
                        if (module.Subdir != null)
                        {
                            subdir = module.Subdir;
                            FileInfo f = new FileInfo(module.Path);
                            Copy(f.FullName, Path.Combine(localUpdatePath, subdir, f.Name));
                        }
                        else
                        {
                            FileInfo f = new FileInfo(module.Path);
                            Copy(f.FullName, Path.Combine(localUpdatePath, f.Name));
                        }

                    }
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
            }

        }
    }
}
