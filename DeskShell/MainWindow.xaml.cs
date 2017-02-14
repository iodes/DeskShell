using DeskShell.Controls;
using DeskShell.Natives;
using Gma.System.MouseKeyHook;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace DeskShell
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        #region 객체
        private FileSystemWatcher watcher;
        private IKeyboardMouseEvents globalHook;
        #endregion

        #region 생성자
        public MainWindow()
        {
            RenderOptions.SetBitmapScalingMode(this, BitmapScalingMode.HighQuality);
            InitializeComponent();
            InitializeView();

            watcher = new FileSystemWatcher
            {
                Path = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory),
                NotifyFilter = NotifyFilters.FileName | NotifyFilters.DirectoryName,
                EnableRaisingEvents = true
            };
            watcher.Created += Watcher_Event;
            watcher.Deleted += Watcher_Event;

            globalHook = Hook.GlobalEvents();
            globalHook.MouseDragStartedExt += GlobalHook_MouseDragStartedExt;
            globalHook.MouseDragFinishedExt += GlobalHook_MouseDragFinishedExt;
        }
        #endregion

        #region 이벤트
        private void Watcher_Event(object sender, FileSystemEventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                deskPanel.Children.Clear();
                CreateFiles(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory));
                CreateFiles(Environment.GetFolderPath(Environment.SpecialFolder.CommonDesktopDirectory));
            });
        }

        private void GlobalHook_MouseDragStartedExt(object sender, MouseEventExtArgs e)
        {
            ShellUtility.SetTranspernt(this, true);
        }

        private void GlobalHook_MouseDragFinishedExt(object sender, MouseEventExtArgs e)
        {
            var delayTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(10)
            };

            delayTimer.Tick += (tS, tE) =>
            {
                ShellUtility.SetTranspernt(this, false);
                delayTimer.Stop();
            };

            delayTimer.Start();
        }
        #endregion

        #region 내부 함수
        private void InitializeView()
        {
            Top = 0;
            Left = 0;
            Width = SystemParameters.PrimaryScreenWidth;
            Height = SystemParameters.PrimaryScreenHeight;

            ShellUtility.SetDesktop(this);
            CreateFiles(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory));
            CreateFiles(Environment.GetFolderPath(Environment.SpecialFolder.CommonDesktopDirectory));
        }

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
