using Captura.Models;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using Ninject;
using Ninject.Modules;

namespace Captura
{
    public static class ServiceProvider
    {
        static string _settingsDir;

        public static string SettingsDir
        {
            get
            {
                if (_settingsDir == null)
                    _settingsDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Captura");

                if (!Directory.Exists(_settingsDir))
                    Directory.CreateDirectory(_settingsDir);

                return _settingsDir;
            }
            set
            {
                _settingsDir = value;

                if (!Directory.Exists(_settingsDir))
                    Directory.CreateDirectory(_settingsDir);
            }
        }

        static IKernel Kernel { get; } = new StandardKernel(new CoreModule());

        public static void LoadModule(INinjectModule Module)
        {
            Kernel.Load(Module);
        }

        public static T Get<T>() => Kernel.Get<T>();
        
        public static void LaunchFile(ProcessStartInfo StartInfo)
        {
            try { Process.Start(StartInfo.FileName); }
            catch (Win32Exception e) when (e.NativeErrorCode == 2)
            {
                MessageProvider.ShowError($"Could not find file: {StartInfo.FileName}");
            }
            catch (Exception e)
            {
                MessageProvider.ShowError($"Could not open file: {StartInfo.FileName}\n\n\n{e}");
            }
        }

        public static IMessageProvider MessageProvider => Get<IMessageProvider>();
        
        public static bool FileExists(string FileName)
        {
            return File.Exists(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), FileName));
        }
    }
}
