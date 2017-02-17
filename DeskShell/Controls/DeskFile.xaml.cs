using DeskShell.Natives;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;

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

            Drop += DeskFile_Drop;
            MouseDoubleClick += DeskFile_MouseDoubleClick;
            MouseLeftButtonDown += DeskFile_MouseLeftButtonDown;
        }
        #endregion

        #region 이벤트
        private void DeskFile_Drop(object sender, DragEventArgs e)
        {
            var files = e.Data.GetData(DataFormats.FileDrop, false) as string[];

            for (int i = 0; i < files.Length; i++)
            {
                files[i] = $@"""{files[i]}""";
            }

            Process.Start(Target, string.Join(" ", files));
        }

        private void DeskFile_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Process.Start(Target);
        }

        private void DeskFile_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            border.Focus();
        }
        #endregion
    }
}
