using System;
using System.IO;
using IWshRuntimeLibrary;
using File = System.IO.File;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace ShadowStartMenu.Menu
{
    public class UmbraMenuSource : IMenuSource
    {
        private readonly ILogger _logger;
        private readonly string _startMenuPath;
        private readonly List<IApp> _apps = new List<IApp>();
        
        public IEnumerable<IApp> Apps => _apps;

        public UmbraMenuSource(ILogger<UmbraMenuSource> logger)
        {
            _logger = logger;
            _startMenuPath = Environment.GetFolderPath(Environment.SpecialFolder.StartMenu);

            foreach (var file in AllFiles(_startMenuPath, "lnk"))
            {
                _apps.Add(new App
                {
                    Name = Path.GetFileNameWithoutExtension(file.Name),
                    Path = GetShortcutTargetFile(file.FullName),
                    ShortcutPath = file.FullName
                });
            }

            foreach (var app in Apps)
            {
                _logger.LogDebug($"Shortcut Detected: {app.Name}");
            }
        }

        public void Add(IApp app, ShortcutType type, params string[] subRoots)
        {
            _apps.Add(app);
            if (subRoots == null)
            {
                subRoots = Array.Empty<string>();
            }
            string path = Path.Combine(_startMenuPath, Path.Combine(subRoots), $"{app.Name}.lnk");
            CreateShortcut(path, app.Path);
            app.ShortcutPath = path;
        }

        public void Remove(IApp app)
        {
            _apps.Remove(app);
            if (File.Exists(app.ShortcutPath))
            {
                File.Delete(app.ShortcutPath!);
                app.ShortcutPath = null;
            }
        }

        private static void CreateShortcut(string path, string originalExe)
        {
            WshShell shell = new WshShell();
            string scLinkPath = Path.Combine(path);
            IWshShortcut shortcut = (IWshShortcut)shell.CreateShortcut(scLinkPath);
            shortcut.TargetPath = originalExe;
            shortcut.Save();
        }

        private static FileInfo[] AllFiles(string directory, params string[] extensions)
        {
            List<FileInfo> files = new List<FileInfo>();
            DirectoryInfo dir = new DirectoryInfo(directory);
            if (dir.Exists)
            {
                foreach (var extension in extensions)
                {
                    foreach (var file in dir.EnumerateFiles($"*.{extension}", SearchOption.AllDirectories))
                    {
                        files.Add(file);
                    }
                }
            }
            return files.ToArray();
        }

        private static string GetShortcutTargetFile(string shortcutFilename)
        {
            WshShell shell = new WshShell();
            IWshShortcut shortcut = (IWshShortcut)shell.CreateShortcut(shortcutFilename);
            return shortcut.TargetPath;
        }

        public class App : IApp
        {
            public string Name { get; init; } = null!;
            public string Path { get; init; } = null!;
            public string? ShortcutPath { get; set; }
        }
    }
}