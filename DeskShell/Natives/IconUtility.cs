using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;

namespace DeskShell.Natives
{
    public static class IconUtility
    {
        #region 상수
        private const uint SHGFI_ICON = 0x100;
        private const uint SHGFI_LARGEICON = 0x0;
        private const uint SHGFI_SMALLICON = 0x1;
        private const uint SHGFI_USEFILEATTRIBUTES = 0x10;
        private const uint SHGFI_ICONLOCATION = 0x1000;
        private const uint SHGFI_SYSICONINDEX = 0x4000;
        private const uint SHIL_JUMBO = 0x4;
        private const uint SHIL_EXTRALARGE = 0x2;
        private const uint ILD_TRANSPARENT = 0x1;
        private const uint ILD_IMAGE = 0x20;
        private const uint FILE_ATTRIBUTE_NORMAL = 0x80;
        #endregion

        #region 열거형
        public enum IconSize
        {
            Large = 0x0,
            Small = 0x1,
            ExtraLarge = 0x2,
            Jumbo = 0x4
        }
        #endregion

        #region 구조체
        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int left, top, right, bottom;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            int x;
            int y;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct IMAGELISTDRAWPARAMS
        {
            public int cbSize;
            public IntPtr himl;
            public int i;
            public IntPtr hdcDst;
            public int x;
            public int y;
            public int cx;
            public int cy;
            public int xBitmap;
            public int yBitmap;
            public int rgbBk;
            public int rgbFg;
            public int fStyle;
            public int dwRop;
            public int fState;
            public int Frame;
            public int crEffect;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct IMAGEINFO
        {
            public IntPtr hbmImage;
            public IntPtr hbmMask;
            public int Unused1;
            public int Unused2;
            public RECT rcImage;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct SHFILEINFOW
        {
            public IntPtr hIcon;
            public int iIcon;
            public uint dwAttributes;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            public string szDisplayName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
            public string szTypeName;
        }
        #endregion

        #region 외부 함수
        [DllImport("shell32.dll", EntryPoint = "#727")]
        public static extern int SHGetImageList(int iImageList, ref Guid riid, ref IImageList ppv);

        [DllImport("shell32.dll", EntryPoint = "SHGetFileInfoW", CallingConvention = CallingConvention.StdCall)]
        private static extern int SHGetFileInfoW([MarshalAs(UnmanagedType.LPWStr)]
        string pszPath, uint dwFileAttributes, ref SHFILEINFOW psfi, int cbFileInfo, uint uFlags);

        [DllImport("shell32.dll", EntryPoint = "SHGetFileInfoW", CallingConvention = CallingConvention.StdCall)]
        private static extern int SHGetFileInfoW(IntPtr pszPath, uint dwFileAttributes, ref SHFILEINFOW psfi, int cbFileInfo, uint uFlags);

        [DllImport("user32.dll", EntryPoint = "DestroyIcon")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool DestroyIcon(IntPtr hIcon);
        #endregion

        #region 사용자 함수
        public static BitmapSource GetIcon(string path, IconSize size)
        {
            BitmapSource result = null;

            var shinfo = new SHFILEINFOW();
            var flags = SHGFI_SYSICONINDEX;

            if (SHGetFileInfoW(path, FILE_ATTRIBUTE_NORMAL, ref shinfo, Marshal.SizeOf(shinfo), flags) == 0)
            {
                throw new FileNotFoundException();
            }

            IImageList iml = null;
            var guid = new Guid("46EB5926-582E-4017-9FDF-E8998DAA0950");
            SHGetImageList((int)size, ref guid, ref iml);

            if (iml != null)
            {
                var hIcon = IntPtr.Zero;
                iml.GetIcon(shinfo.iIcon, (int)ILD_IMAGE, ref hIcon);

                var icon = Icon.FromHandle(hIcon);

                if (icon.ToBitmap().PixelFormat != PixelFormat.Format32bppArgb)
                {
                    icon.Dispose();
                    DestroyIcon(hIcon);
                    iml.GetIcon(shinfo.iIcon, (int)ILD_TRANSPARENT, ref hIcon);
                }

                result = Imaging.CreateBitmapSourceFromHIcon(icon.Handle, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                result.Freeze();

                icon.Dispose();
                DestroyIcon(hIcon);
            }

            return result;
        }
        #endregion

        #region 인터페이스
        [ComImport()]
        [Guid("46EB5926-582E-4017-9FDF-E8998DAA0950")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IImageList
        {
            [PreserveSig]
            int Add(
            IntPtr hbmImage,
            IntPtr hbmMask,
            ref int pi);

            [PreserveSig]
            int ReplaceIcon(
            int i,
            IntPtr hicon,
            ref int pi);

            [PreserveSig]
            int SetOverlayImage(
            int iImage,
            int iOverlay);

            [PreserveSig]
            int Replace(
            int i,
            IntPtr hbmImage,
            IntPtr hbmMask);

            [PreserveSig]
            int AddMasked(
            IntPtr hbmImage,
            int crMask,
            ref int pi);

            [PreserveSig]
            int Draw(
            ref IMAGELISTDRAWPARAMS pimldp);

            [PreserveSig]
            int Remove(
            int i);

            [PreserveSig]
            int GetIcon(
            int i,
            int flags,
            ref IntPtr picon);

            [PreserveSig]
            int GetImageInfo(
            int i,
            ref IMAGEINFO pImageInfo);

            [PreserveSig]
            int Copy(
            int iDst,
            IImageList punkSrc,
            int iSrc,
            int uFlags);

            [PreserveSig]
            int Merge(
            int i1,
            IImageList punk2,
            int i2,
            int dx,
            int dy,
            ref Guid riid,
            ref IntPtr ppv);

            [PreserveSig]
            int Clone(
            ref Guid riid,
            ref IntPtr ppv);

            [PreserveSig]
            int GetImageRect(
            int i,
            ref RECT prc);

            [PreserveSig]
            int GetIconSize(
            ref int cx,
            ref int cy);

            [PreserveSig]
            int SetIconSize(
            int cx,
            int cy);

            [PreserveSig]
            int GetImageCount(
            ref int pi);

            [PreserveSig]
            int SetImageCount(
            int uNewCount);

            [PreserveSig]
            int SetBkColor(
            int clrBk,
            ref int pclr);

            [PreserveSig]
            int GetBkColor(
            ref int pclr);

            [PreserveSig]
            int BeginDrag(
            int iTrack,
            int dxHotspot,
            int dyHotspot);

            [PreserveSig]
            int EndDrag();

            [PreserveSig]
            int DragEnter(
            IntPtr hwndLock,
            int x,
            int y);

            [PreserveSig]
            int DragLeave(
            IntPtr hwndLock);

            [PreserveSig]
            int DragMove(
            int x,
            int y);

            [PreserveSig]
            int SetDragCursorImage(
            ref IImageList punk,
            int iDrag,
            int dxHotspot,
            int dyHotspot);

            [PreserveSig]
            int DragShowNolock(
            int fShow);

            [PreserveSig]
            int GetDragImage(
            ref POINT ppt,
            ref POINT pptHotspot,
            ref Guid riid,
            ref IntPtr ppv);

            [PreserveSig]
            int GetItemFlags(
            int i,
            ref int dwFlags);

            [PreserveSig]
            int GetOverlayImage(
            int iOverlay,
            ref int piIndex);
        };
        #endregion
    }
}
