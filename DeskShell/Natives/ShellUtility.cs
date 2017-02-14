using System;
using System.Windows;
using System.Windows.Interop;

namespace DeskShell.Natives
{
    public static class ShellUtility
    {
        private static int originalStyle;

        public static void SetDesktop(Window target)
        {
            var handle = new WindowInteropHelper(target).EnsureHandle();

            var progman = WinAPI.FindWindow("Progman", null);
            var defView = WinAPI.FindWindowEx(progman, IntPtr.Zero, "SHELLDLL_DefView", null);
            var folderView = WinAPI.FindWindowEx(defView, IntPtr.Zero, "SysListView32", null);

            WinAPI.SetParent(handle, defView);
            WinAPI.ShowWindow(folderView, WinAPI.ShowWindowCommands.Hide);

            WinAPI.SetWindowLong(handle, WinAPI.GWL_EXSTYLE, WinAPI.GetWindowLong(handle, WinAPI.GWL_EXSTYLE) | WinAPI.WS_EX_NOACTIVATE);
        }

        public static void SetTranspernt(Window target, bool value)
        {
            var handle = new WindowInteropHelper(target).EnsureHandle();

            if (originalStyle == 0)
            {
                originalStyle = WinAPI.GetWindowLong(handle, WinAPI.GWL_EXSTYLE);
            }

            if (value)
            {
                WinAPI.SetWindowLong(handle, WinAPI.GWL_EXSTYLE, originalStyle | WinAPI.WS_EX_TRANSPARENT);
            }
            else
            {
                WinAPI.SetWindowLong(handle, WinAPI.GWL_EXSTYLE, originalStyle);
            }
        }
    }
}
