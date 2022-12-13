using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Medo.Client.Updater.XmlConfigurationUpdater
{
    static class Program
    {
        public static T getCustomAttribute<T>(this ICustomAttributeProvider assembly, bool inherit = false)
          where T : Assembly
        {
            return assembly.GetCustomAttributes(typeof(T), inherit)
                           .OfType<T>()
                           .FirstOrDefault();
        }

        public static string getSubDir(FileInfo file)
        {
            try
            {


                string subdir = string.Empty;
                List<char> sdir = new List<char>();
                int subdirchars = file.FullName.Length - UModel.UpdatePath.Length;
                for (int i = 0; i <= subdirchars; i++)
                {
                    sdir.Add(file.FullName[file.FullName.Length - subdirchars - 1 + i]);
                }
                if (sdir.Count > 0)
                {
                    sdir.RemoveAt(0);
                    var dir = new string(sdir.ToArray());
                    var index = dir.LastIndexOf('\\');
                    if (index > 0)
                    {
                        subdir = dir.Remove(index, dir.Length - index);
                    }
                    if (subdir.Length > 0)
                    {
                        return subdir;
                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    return null;
                }
            }
            catch (System.Exception ex)
            {
                logger.Fatal(ex);
                return null;
            }

        }
        readonly static Logger logger = LogManager.GetCurrentClassLogger();
        static Guid gVersion { get; set; }
        static AppDomain currentDomain = AppDomain.CurrentDomain;
        static Updater.Model.UpdaterModel UModel = new Updater.Model.UpdaterModel();
        static void Main(string[] args)
        {
            try
            {
                bool IsCritical = false;
                if (args.Count() > 0)
                {
                    if (args.Contains("-critical"))
                    {
                        IsCritical = true;
                        Console.WriteLine("Создание пакета критического обновления...");
                    }
                }
                else
                {
                    Console.WriteLine("Создание пакета регулярного обновления...");
                }
             
                Updater.Model.UpdaterModel UModel = new Updater.Model.UpdaterModel();
                List<Updater.Model.ModuleVersion> UList = new List<Updater.Model.ModuleVersion>();
                FileInfo[] dlls = new DirectoryInfo(UModel.UpdatePath).GetFiles("*.dll", SearchOption.AllDirectories);
                FileInfo xml = new FileInfo(Path.Combine(UModel.UpdatePath, "version.xml"));
                FileInfo exe = new FileInfo(Path.Combine(UModel.UpdatePath, "Medo.Client.exe"));
                FileInfo config = new FileInfo(Path.Combine(UModel.UpdatePath, "Medo.Client.exe.config"));
                FileInfo libmupdf = new FileInfo(Path.Combine(UModel.UpdatePath, "libmupdf.dll"));
                FileInfo nlogconfig = new FileInfo(Path.Combine(UModel.UpdatePath, "NLog.config"));
                FileInfo xmlVersion = new FileInfo(Path.Combine(UModel.UpdatePath, "version.xml"));

                foreach (FileInfo fi in dlls)
                {

                    try
                    {
                        Assembly ass = Assembly.Load(File.ReadAllBytes(fi.FullName));
                        Updater.Model.ModuleVersion UVersion = new Updater.Model.ModuleVersion();
                        UVersion.Name = ass.FullName.Split(',')[0].Replace(".resource", "") + ".dll";
                        UVersion.Path = fi.FullName;
                        UVersion.Version = ass.FullName.Split(',')[1].Replace("Version=", "").TrimStart(' ');
                        UVersion.Description = ass.GetCustomAttribute<AssemblyDescriptionAttribute>().Description;
                        UVersion.EditTime = fi.LastWriteTime;
                        UVersion.Subdir = getSubDir(fi);
                        UList.Add(UVersion);
                        Console.WriteLine(string.Format("Модуль {0} версии {1} добавлен в пакет обновления", UVersion.Name, UVersion.Version));
                    }
                    catch (BadImageFormatException bi)
                    {

                        Updater.Model.ModuleVersion badimage = new Updater.Model.ModuleVersion();
                        badimage.Name = fi.Name;
                        badimage.Path = fi.FullName;
                        badimage.Version = "0";
                        badimage.EditTime = fi.LastWriteTime;
                        badimage.Subdir = getSubDir(fi);
                        UList.Add(badimage);
                        Console.WriteLine(string.Format("Dll файл {0} добавлен в пакет обновления", badimage.Name));
                        logger.Info(bi);
                    }
                    catch (System.Exception ex)
                    {
                        logger.Fatal(ex);
                    }

                }
                Assembly exeAss = Assembly.Load(File.ReadAllBytes(exe.FullName));
                Updater.Model.ModuleVersion exeVersion = new Updater.Model.ModuleVersion();
                exeVersion.Name = exeAss.FullName.Split(',')[0].Replace(".resource", "") + ".exe";
                exeVersion.Path = exe.FullName;
                exeVersion.Version = exeAss.FullName.Split(',')[1].Replace("Version=", "").TrimStart(' ');
                exeVersion.Description = exeAss.GetCustomAttribute<AssemblyDescriptionAttribute>().Description;
                exeVersion.EditTime = exe.LastWriteTime;
                UList.Add(exeVersion);
                Console.WriteLine(string.Format("Файл {0} добавлен в пакет обновления", exeVersion.Name));

                Updater.Model.ModuleVersion configVersion = new Updater.Model.ModuleVersion();
                configVersion.Name = config.Name;
                configVersion.Path = config.FullName;
                configVersion.Version = "0";
                configVersion.EditTime = config.LastWriteTime;
                UList.Add(configVersion);
                Console.WriteLine(string.Format("Файл {0} добавлен в пакет обновления", configVersion.Name));

                Updater.Model.ModuleVersion nlog = new Updater.Model.ModuleVersion();
                nlog.Name = nlogconfig.Name;
                nlog.Path = nlogconfig.FullName;
                nlog.Description = "Конфиг файл логирования NLog.dll";
                nlog.EditTime = nlogconfig.LastWriteTime;
                nlog.Version = "0";
                UList.Add(nlog);
                Console.WriteLine(string.Format("Файл {0} добавлен в пакет обновления", nlog.Name));

                Updater.Model.ModuleVersion version = new Updater.Model.ModuleVersion();
                version.Name = xmlVersion.Name;
                version.Path = xmlVersion.FullName;
                version.Description = "Конфиг файл обновления системы";
                version.EditTime = xmlVersion.LastWriteTime;
                version.Version = "0";
                UList.Add(version);
                Console.WriteLine(string.Format("Файл {0} добавлен в пакет обновления", version.Name));

              
                try
                {
                    string[] history = File.ReadAllLines(Path.Combine(UModel.UpdatePath, "VersionHistory"), Encoding.Default);
                    foreach (string h in history)
                    {
                        UModel.VersionHistory += h + Environment.NewLine;
                    }
                    Console.WriteLine(string.Format("Файл {0} добавлен в пакет обновления", "VersionHistory"));
                }
                catch (System.Exception ex)
                {
                    logger.Fatal(ex);
                }
                UModel.IsCriticalUpdate = IsCritical;
                UModel.Modules = UList;
                UModel.UpdateTime = DateTime.Now;
                UModel.Version = exeVersion.Version;

                System.Xml.Serialization.XmlSerializer ser = new System.Xml.Serialization.XmlSerializer(typeof(Model.UpdaterModel));

                using (FileStream fs = new FileStream(Path.Combine(UModel.UpdatePath, "version.xml"), FileMode.Create))
                {
                    ser.Serialize(fs, UModel);
                }
                Console.WriteLine("Сериализация закончена. Нажмите любую кнопку для выхода...");
                Console.ReadKey();
            }
            catch (System.Exception ex)
            {
                logger.Fatal(ex);
            }
        }
    }
}
