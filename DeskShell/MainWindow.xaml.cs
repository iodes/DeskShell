using DeskShell.Natives;
using Gma.System.MouseKeyHook;
using System;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Threading;

namespace DeskShell
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        #region 객체
        private ShellWindow shell;
        private IKeyboardMouseEvents globalHook;
        #endregion

        #region 생성자
        public MainWindow()
        {
            InitializeComponent();
            RenderOptions.SetBitmapScalingMode(this, BitmapScalingMode.HighQuality);

            globalHook = Hook.GlobalEvents();
            globalHook.MouseDragStartedExt += GlobalHook_MouseDragStartedExt;
            globalHook.MouseDragFinishedExt += GlobalHook_MouseDragFinishedExt;

            Loaded += MainWindow_Loaded;
        }
        #endregion

        #region 이벤트
        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            (PresentationSource.FromVisual(this) as HwndSource).AddHook(WndProc);
        }

        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == WinAPI.WM_MOUSEACTIVATE)
            {
                handled = true;
                return new IntPtr(WinAPI.MA_NOACTIVATE);
            }
            else
            {
                return IntPtr.Zero;
            }
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            ShellUtility.SetDesktop(this);

            shell = new ShellWindow
            {
                Owner = this
            };

            shell.Show();
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
    }
}
