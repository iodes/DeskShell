using System;
using System.Windows;
using System.Windows.Interop;

namespace DeskShell.Natives
{
    public static class ShellUtility
    {
        public static void SetShell(Window target)
        {
            if (target.Owner != null)
            {
                SetDesktop(target);

                target.Top = target.Owner.Top;
                target.Left = target.Owner.Left;
                target.Width = target.Owner.Width;
                target.Height = target.Owner.Height;
            }
        }

        public static void SetDesktop(Window target)
        {
            var handle = new WindowInteropHelper(target).EnsureHandle();

            var progman = WinAPI.FindWindow("Progman", null);
            var defView = WinAPI.FindWindowEx(progman, IntPtr.Zero, "SHELLDLL_DefView", null);
            var folderView = WinAPI.FindWindowEx(defView, IntPtr.Zero, "SysListView32", null);

            WinAPI.SetParent(handle, defView);
            WinAPI.ShowWindow(folderView, WinAPI.ShowWindowCommands.Hide);
            WinAPI.SetWindowLong(handle, WinAPI.GWL_EXSTYLE, WinAPI.GetWindowLong(handle, WinAPI.GWL_EXSTYLE) | WinAPI.WS_EX_NOACTIVATE);

            target.Top = 0;
            target.Left = 0;
            target.Width = SystemParameters.PrimaryScreenWidth;
            target.Height = SystemParameters.PrimaryScreenHeight;
        }

        public static void SetTranspernt(Window target, bool value)
        {
            var handle = new WindowInteropHelper(target).EnsureHandle();
            var extendedStyle = WinAPI.GetWindowLong(handle, WinAPI.GWL_EXSTYLE);

            if (value)
            {
                WinAPI.SetWindowLong(handle, WinAPI.GWL_EXSTYLE, extendedStyle | WinAPI.WS_EX_TRANSPARENT);
            }
            else
            {
                WinAPI.SetWindowLong(handle, WinAPI.GWL_EXSTYLE, extendedStyle & ~WinAPI.WS_EX_TRANSPARENT);
            }
        }
    }
}
