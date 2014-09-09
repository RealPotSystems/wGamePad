using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace vGamePad
{
    /// <summary>
    /// LayoutWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class LayoutWindow : Window
    {
        private Grid buttonState = null;
        private Grid topLeft = null;
        private Grid topRight = null;
        private Grid bottomLeft = null;
        private Grid bottomRight = null;

        private vButtonDictionary layoutDic = null;
        public LayoutWindow()
        {
            InitializeComponent();

            // 現在のレイアウトをいったん保存しておく
            vLayoutControl.SaveLayout(9, MainWindow.dic);
            // そのレイアウトをローカルに取り込む
            layoutDic = vLayoutControl.LoadLayout(9);
        }

        private Grid createButton(string uid, System.Windows.VerticalAlignment v, System.Windows.HorizontalAlignment h , Color c)
        {
            var button = new Grid();
            button.Name = uid;
            button.Uid = uid;
            button.Width = 32;
            button.Height = 32;
            button.VerticalAlignment = v;
            button.HorizontalAlignment = h;
            button.Opacity = 0.75;
            button.Background = new SolidColorBrush(c);
            return button;
        }

        private Point getScreenRange(Point pt)
        {
            int x = (int)(pt.X / App.GRID) * App.GRID;
            int y = (int)(pt.Y / App.GRID) * App.GRID;
            if (y < 16)
                y = 16;
            if (y > Height - 16)
                y = (int)Height - 16;
            return new Point(x, y);
        }

        private void TopLeftDown(Grid grid, UIElement source)
        {
            var gridtop = (double)grid.GetValue(Canvas.TopProperty);
            var gridleft = (double)grid.GetValue(Canvas.LeftProperty);
            
            // 横
            Line1.X1 = 32;
            Line1.Y1 = gridtop + 32;
            Line1.X2 = gridleft + 32;
            Line1.Y2 = gridtop + 32;

            // 縦
            Line2.X1 = gridleft + 32;
            Line2.Y1 = 48;
            Line2.X2 = gridleft + 32;
            Line2.Y2 = gridtop + 32;

            Line1.Visibility = System.Windows.Visibility.Visible;
            Line2.Visibility = System.Windows.Visibility.Visible;

            topLeft.Background = new SolidColorBrush(Colors.Red);
            topRight.Background = new SolidColorBrush(Colors.White);
            bottomLeft.Background = new SolidColorBrush(Colors.White);
            bottomRight.Background = new SolidColorBrush(Colors.White);

            // top + left を起点に変更する
            var button = layoutDic[grid.Uid];
            button.Top = gridtop + 32;
            button.Bottom = double.MaxValue;
            button.Left = gridleft + 32;
            button.Right = double.MaxValue;
        }

        private void TopLeftMove(Grid grid, MouseEventArgs e)
        {
            var _point = getScreenRange(e.GetPosition(LayoutAreaCanvas));

            if (_point.Y > Height - grid.Height - 16)
                _point.Y = Height - grid.Height - 16;
            if (_point.X > Width - grid.Width)
                _point.X = Width - grid.Width;

            grid.SetValue(Canvas.TopProperty, (double)_point.Y);
            grid.SetValue(Canvas.LeftProperty, (double)_point.X);

            // 横
            Line1.X1 = 32;
            Line1.Y1 = _point.Y + 32;
            Line1.X2 = _point.X + 32;
            Line1.Y2 = _point.Y + 32;

            Line2.X1 = _point.X + 32;
            Line2.Y1 = 48;
            Line2.X2 = _point.X + 32;
            Line2.Y2 = _point.Y + 32;
        }

        private void TopLeftUp(Grid grid, UIElement source)
        {
            Line1.Visibility = System.Windows.Visibility.Collapsed;
            Line2.Visibility = System.Windows.Visibility.Collapsed;
            buttonState.Children.RemoveRange(1, 4);
            buttonState = null;

            // 移動先の設定
        }

        private void TopRightDown(Grid grid, UIElement source)
        {
            var gridtop = (double)grid.GetValue(Canvas.TopProperty);
            var gridleft = (double)grid.GetValue(Canvas.LeftProperty);

            // 横
            Line1.X1 = Width - 32;
            Line1.Y1 = gridtop + 32;
            Line1.X2 = gridleft + grid.Width - 32;
            Line1.Y2 = gridtop + 32;

            // 縦
            Line2.X1 = gridleft + grid.Width - 32;
            Line2.Y1 = 48;
            Line2.X2 = gridleft + grid.Width - 32;
            Line2.Y2 = gridtop + 32;

            Line1.Visibility = System.Windows.Visibility.Visible;
            Line2.Visibility = System.Windows.Visibility.Visible;

            topLeft.Background = new SolidColorBrush(Colors.White);
            topRight.Background = new SolidColorBrush(Colors.Red);
            bottomLeft.Background = new SolidColorBrush(Colors.White);
            bottomRight.Background = new SolidColorBrush(Colors.White);

            // top + right を起点に変更する
            var button = layoutDic[grid.Uid];
            button.Top = gridtop + 32;
            button.Bottom = double.MaxValue;
            button.Left = double.MaxValue;
            button.Right = Width - grid.Width - 32;
        }

        private void TopRightMove(Grid grid, MouseEventArgs e)
        {
            var _point = getScreenRange(e.GetPosition(LayoutAreaCanvas));
            grid.SetValue(Canvas.TopProperty, (double)_point.Y);
            grid.SetValue(Canvas.LeftProperty, (double)_point.X - grid.Width + 32 );

            // 横
            Line1.X1 = Width - 32;
            Line1.Y1 = _point.Y + 32;
            Line1.X2 = _point.X;
            Line1.Y2 = _point.Y + 32;

            // 縦
            Line2.X1 = _point.X;
            Line2.Y1 = 48;
            Line2.X2 = _point.X;
            Line2.Y2 = _point.Y + 32;
        }

        private void TopRightUp(Grid grid, UIElement source)
        {
            Line1.Visibility = System.Windows.Visibility.Collapsed;
            Line2.Visibility = System.Windows.Visibility.Collapsed;
            buttonState.Children.RemoveRange(1, 4);
            buttonState = null;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var dpi = ViewExtensions.GetDpiScaleFactor(this);
            var screen = System.Windows.Forms.Screen.PrimaryScreen;
            Top = 0;
            Left = 0;
            Width = (int)(screen.Bounds.Width / dpi.X);
            Height = (int)(screen.Bounds.Height / dpi.Y);

            ScaleTransform scaleTransform = new ScaleTransform();

            GridLineAreaCanvas.Background = new SolidColorBrush(Colors.Black);
            for (int i = 0; i < GridLineAreaCanvas.ActualWidth; i += App.GRID)
            {
                Path path = new Path()
                {
                    Data = new LineGeometry(new Point(i, 0), new Point(i, GridLineAreaCanvas.ActualHeight)),
                    Stroke = Brushes.White,
                    StrokeThickness = .5
                };

                path.Data.Transform = scaleTransform;

                GridLineAreaCanvas.Children.Add(path);
            }

            // 横線
            for (int i = 0; i < GridLineAreaCanvas.ActualHeight; i += App.GRID)
            {
                Path path = new Path()
                {
                    Data = new LineGeometry(new Point(0, i), new Point(GridLineAreaCanvas.ActualWidth, i)),
                    Stroke = Brushes.White,
                    StrokeThickness = .5
                };

                path.Data.Transform = scaleTransform;

                GridLineAreaCanvas.Children.Add(path);
            }


            // 現在のレイアウトを取り込み初期値とする
            if (layoutDic != null)
            {
                foreach (string key in MainWindow.dic.Keys)
                {
                    var button = layoutDic[key];

                    if (button.Fixed == true)
                        continue;
                    if (button.Top == double.MaxValue && button.Bottom == double.MaxValue)
                        continue;
                    var searchKey = key;
                    // 基準の取得
                    foreach (UIElement ui in LayoutAreaCanvas.Children)
                    {
                        if ( ui.Uid == searchKey )
                        {
                            System.Diagnostics.Debug.WriteLine(ui.Uid);
                            if (button.Top != double.MaxValue)
                                ui.SetValue(Canvas.TopProperty, button.Top - 32);
                            if (button.Bottom != double.MaxValue)
                                ui.SetValue(Canvas.TopProperty, Height - button.Height - button.Bottom - 32);
                            if (button.Left != double.MaxValue)
                                ui.SetValue(Canvas.LeftProperty, button.Left - 32);
                            if (button.Right != double.MaxValue)
                                ui.SetValue(Canvas.LeftProperty, Width - button.Width - button.Right - 32);
                            ui.Visibility = System.Windows.Visibility.Visible;
                            break;
                        }
                    }
                }
            }
        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var grid = (Grid)sender;
            var postion = (UIElement)e.Source;
            switch (postion.Uid)
            {
                case "TopLeft":
                    TopLeftDown(grid, postion);
                    postion.CaptureMouse();
                    break;
                case "TopRight":
                    TopRightDown(grid, postion);
                    postion.CaptureMouse();
                    break;
                case "BottomLeft":
                    break;
                case "BottomRight":
                    break;
                default:
                    // すべてのボタンで同じロジックを採用する
                    if (buttonState != null)
                    {
                        buttonState.Children.RemoveRange(1, 4);
                        buttonState.SetValue(Canvas.ZIndexProperty, 0);
                    }

                    // 周りの４つのブロックを作成
                    // 始点によって色を変える
                    var SelectedButton = layoutDic[grid.Uid];
                    Color tlc = (SelectedButton.Top != double.MaxValue && SelectedButton.Left != double.MaxValue) ? Colors.Red : Colors.White;
                    Color trc = (SelectedButton.Top != double.MaxValue && SelectedButton.Right != double.MaxValue) ? Colors.Red : Colors.White;
                    Color blc = (SelectedButton.Bottom != double.MaxValue && SelectedButton.Left != double.MaxValue) ? Colors.Red : Colors.White;
                    Color brc = (SelectedButton.Bottom != double.MaxValue && SelectedButton.Right != double.MaxValue) ? Colors.Red : Colors.White;

                    topLeft = createButton("TopLeft", System.Windows.VerticalAlignment.Top, System.Windows.HorizontalAlignment.Left, tlc);
                    topRight = createButton("TopRight", System.Windows.VerticalAlignment.Top, System.Windows.HorizontalAlignment.Right, trc);
                    bottomLeft = createButton("BottomLeft", System.Windows.VerticalAlignment.Bottom, System.Windows.HorizontalAlignment.Left, blc);
                    bottomRight = createButton("BottomRight", System.Windows.VerticalAlignment.Bottom, System.Windows.HorizontalAlignment.Right, brc);
                    grid.Children.Add(topLeft);
                    grid.Children.Add(topRight);
                    grid.Children.Add(bottomLeft);
                    grid.Children.Add(bottomRight);
                    grid.SetValue(Canvas.ZIndexProperty, 100);
                    buttonState = grid;
                    break;
            }
            e.Handled = true;
        }

        private void Grid_MouseUp(object sender, MouseButtonEventArgs e)
        {
            var grid = (Grid)sender;
            var gridtop = (double)grid.GetValue(Canvas.TopProperty);
            var gridleft = (double)grid.GetValue(Canvas.LeftProperty);

            var postion = (UIElement)e.Source;
            switch (postion.Uid)
            {
                case "TopLeft":
                    TopLeftUp(grid, postion);
                    postion.ReleaseMouseCapture();
                    break;
                case "TopRight":
                    TopRightUp(grid, postion);
                    postion.ReleaseMouseCapture();
                    break;
                case "BottomLeft":
                    break;
                case "BottomRight":
                    break;
                default:
                    break;
            }
            e.Handled = true;
        }

        private void Grid_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton != MouseButtonState.Pressed)
                return;

            var grid = (Grid)sender;
            var postion = (UIElement)e.Source;
            switch (postion.Uid)
            {
                case "TopLeft":
                    TopLeftMove(grid, e);
                    break;
                case "TopRight":
                    TopRightMove(grid, e);
                    break;
                case "BottomLeft":
                    break;
                case "BottomRight":
                    break;
                default:
                    break;
            }
            e.Handled = true;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
