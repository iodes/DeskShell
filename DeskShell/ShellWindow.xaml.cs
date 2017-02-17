using DeskShell.Controls;
using DeskShell.Natives;
using System;
using System.IO;
using System.Windows;

namespace DeskShell
{
    /// <summary>
    /// ShellWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ShellWindow : Window
    {
        #region 객체
        private FileSystemWatcher watcher;
        #endregion

        #region 생성자
        public ShellWindow()
        {
            InitializeComponent();

            watcher = new FileSystemWatcher
            {
                Path = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory),
                NotifyFilter = NotifyFilters.FileName | NotifyFilters.DirectoryName,
                EnableRaisingEvents = true
            };
            watcher.Created += Watcher_Event;
            watcher.Deleted += Watcher_Event;

            Loaded += ShellWindow_Loaded;
        }
        #endregion

        #region 이벤트
        private void ShellWindow_Loaded(object sender, RoutedEventArgs e)
        {
            ShellUtility.SetShell(this);

            CreateFiles(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory));
            CreateFiles(Environment.GetFolderPath(Environment.SpecialFolder.CommonDesktopDirectory));
        }

        private void Watcher_Event(object sender, FileSystemEventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                deskPanel.Children.Clear();
                CreateFiles(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory));
                CreateFiles(Environment.GetFolderPath(Environment.SpecialFolder.CommonDesktopDirectory));
            });
        }
        #endregion

        #region 내부 함수
        private void CreateFiles(string path)
        {
            var files = new DirectoryInfo(path).GetFiles();

            foreach (var file in files)
            {
                if (!file.Attributes.HasFlag(FileAttributes.Hidden))
                {
                    deskPanel.Children.Add(new DeskFile
                    {
                        Width = 80,
                        Margin = new Thickness(0, 0, 0, 10),
                        Target = file.FullName
                    });
                }
            }
        }
        #endregion
    }
}
