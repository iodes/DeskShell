using DeskShell.Natives;
using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace DeskShell.Controls
{
    /// <summary>
    /// DeskFile.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class DeskFile : UserControl
    {
        #region 속성
        public string Target
        {
            get
            {
                return _Target;
            }
            set
            {
                _Target = value;

                textTitle.Text = Path.GetFileNameWithoutExtension(Target);
                ImageIcon.Source = IconUtility.GetIcon(Target, IconUtility.IconSize.ExtraLarge);
            }
        }
        private string _Target;
        #endregion

        #region 생성자
        public DeskFile()
        {
            InitializeComponent();
            MouseDoubleClick += DeskFile_MouseDoubleClick;
        }
        #endregion

        #region 이벤트
        private void DeskFile_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Process.Start(Target);
        }
        #endregion
    }
}
