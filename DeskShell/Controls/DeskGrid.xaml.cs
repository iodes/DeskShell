using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace DeskShell.Controls
{
    /// <summary>
    /// DeskGrid.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class DeskGrid : UserControl
    {
        #region 객체
        private int lastRow;
        private int lastCol;
        #endregion

        #region 속성
        public double ItemWidth
        {
            get
            {
                return _ItemWidth;
            }
            set
            {
                _ItemWidth = value;
                UpdateGrid(GridType.Row, true);
            }
        }
        private double _ItemWidth;

        public double ItemHeight
        {
            get
            {
                return _ItemHeight;
            }
            set
            {
                _ItemHeight = value;
                UpdateGrid(GridType.Column, true);
            }
        }
        private double _ItemHeight;

        public bool Guideline
        {
            get
            {
                return _Guideline;
            }
            set
            {
                _Guideline = true;
            }
        }
        private bool _Guideline = false;
        #endregion

        #region 열거형
        private enum GridType
        {
            All,
            Row,
            Column
        }
        #endregion

        #region 생성자
        public DeskGrid()
        {
            InitializeComponent();
            Loaded += DeskGrid_Loaded;
        }
        #endregion

        #region 이벤트
        protected override void OnRender(DrawingContext dc)
        {
            if (Guideline)
            {
                double leftOffset = 0;
                double topOffset = 0;

                Pen pen = new Pen(Brushes.Black, 3);
                pen.Freeze();

                foreach (var row in gridPanel.RowDefinitions)
                {
                    dc.DrawLine(pen, new Point(0, topOffset), new Point(ActualWidth, topOffset));
                    topOffset += row.ActualHeight;
                }
                dc.DrawLine(pen, new Point(0, topOffset), new Point(ActualWidth, topOffset));

                foreach (var column in gridPanel.ColumnDefinitions)
                {
                    dc.DrawLine(pen, new Point(leftOffset, 0), new Point(leftOffset, ActualHeight));
                    leftOffset += column.ActualWidth;
                }
                dc.DrawLine(pen, new Point(leftOffset, 0), new Point(leftOffset, ActualHeight));
            }

            base.OnRender(dc);
        }

        private void DeskGrid_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateGrid(GridType.All);
        }
        #endregion

        #region 내부 함수
        private void UpdateGrid(GridType type, bool sizeUpdate = false)
        {
            var actionRow = new Action(() =>
            {
                gridPanel.RowDefinitions.Clear();
                for (double i = 0; i < ActualHeight / ItemHeight; i++)
                {
                    gridPanel.RowDefinitions.Add(new RowDefinition
                    {
                        Height = new GridLength(ItemHeight)
                    });
                }
            });

            var actionCol = new Action(() =>
            {
                gridPanel.ColumnDefinitions.Clear();
                for (double i = 0; i < ActualWidth / ItemWidth; i++)
                {
                    gridPanel.ColumnDefinitions.Add(new ColumnDefinition
                    {
                        Width = new GridLength(ItemWidth)
                    });
                }
            });

            switch (type)
            {
                case GridType.All:
                    actionRow();
                    actionCol();
                    break;

                case GridType.Row:
                    actionRow();
                    break;

                case GridType.Column:
                    actionCol();
                    break;
            }

            if (sizeUpdate)
            {
                foreach (Control control in gridPanel.Children)
                {
                    control.Width = ItemWidth;
                    control.Height = ItemHeight;
                }
            }
        }
        #endregion

        #region 사용자 함수
        public void Add(Control element)
        {
            UpdateGrid(GridType.All);

            element.Width = ItemWidth;
            element.Height = ItemHeight;
            gridPanel.Children.Add(element);

            Grid.SetRow(element, lastRow);
            Grid.SetColumn(element, lastCol);

            Console.WriteLine($"{lastRow} : {lastCol} : {gridPanel.RowDefinitions.Count}");

            if (lastRow >= gridPanel.RowDefinitions.Count)
            {
                lastRow = 0;
                lastCol++;
            }
            else
            {
                lastRow++;
            }
        }

        public void Clear()
        {
            lastRow = 0;
            lastCol = 0;
            gridPanel.Children.Clear();
        }
        #endregion
    }
}
