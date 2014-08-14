using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Diagnostics;

namespace wGamePad
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        private static vButtonDictionay dic = null;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Initialized(object sender, EventArgs e)
        {
            var screen = System.Windows.Forms.Screen.PrimaryScreen;
            this.Left = screen.Bounds.Left;
            this.Top = screen.Bounds.Top;
            this.Width = screen.Bounds.Width;
            this.Height = screen.Bounds.Height;

            // Sample
            dic = new vButtonDictionay();

            foreach (UIElement ui in vGamePadCanvas.Children)
            {
                foreach (string key in dic.vButtonDic.Keys)
                {
                    if (ui.Uid == key)
                    {
                        var button = dic.vButtonDic[key];
                        if (button.Top != double.MaxValue)
                        {
                            ui.SetValue(Canvas.TopProperty, button.Top);
                        }
                        if (button.Left != double.MaxValue)
                        {
                            ui.SetValue(Canvas.LeftProperty, button.Left);
                        }
                        if (button.Bottom != double.MaxValue)
                        {
                            ui.SetValue(Canvas.BottomProperty, button.Bottom);
                        }
                        if (button.Right != double.MaxValue)
                        {
                            ui.SetValue(Canvas.RightProperty, button.Right);
                        }
                        ui.SetValue(Canvas.VisibilityProperty, button.Visible);
                        button.Width = (double)ui.GetValue(WidthProperty);
                        button.Height = (double)ui.GetValue(HeightProperty);
                        break;
                    }
                }
            }
            dic.vButtonDic["Exit"].UpAction += new EventHandler(ExitGamePad);
            // Sample
        }

        // サンプル
        public void ExitGamePad(object sender, EventArgs e)
        {
            this.Close();
        }

        private void ChangeButtonStatus(UIElement ui, Color A, Color B)
        {
            // 反転
            foreach (object child in ((Grid)ui).Children)
            {
                if (child is Ellipse)
                {
                    ((Ellipse)child).Fill = new SolidColorBrush(A);
                }
                if (child is Label)
                {
                    ((Label)child).Foreground = new SolidColorBrush(B);
                }
            }

        }

        private void OnPointerDown(Point point, uint id)
        {
            foreach (string key in dic.vButtonDic.Keys)
            {
                if (dic.vButtonDic[key].hitTest(point))
                {
                    foreach (UIElement ui in vGamePadCanvas.Children)
                    {
                        if (ui.Uid == key)
                        {
                            // 反転
                            ChangeButtonStatus(ui, Colors.Black, Colors.White);
                            dic.vButtonDic[key].Id = id;

                            // 移動可能なコントロール
                            if (dic.vButtonDic[key].Moving)
                            {
                                // この座標を中心点にする
                                // 中心点の計算
                                double width = (double)ui.GetValue(WidthProperty);
                                double height = (double)ui.GetValue(HeightProperty);
                                ui.SetValue(Canvas.LeftProperty, point.X - width / 2);
                                ui.SetValue(Canvas.TopProperty, point.Y - height / 2);
                            }

                            // イベントハンドラが登録されている場合はDownイベントを実行する
                            dic.vButtonDic[key].ExecEvent(vButton.ExecType.Down);
                            break;
                        }
                    }
                    break;
                }
            }

        }

        private void OnPointerUp(uint id)
        {
            foreach (string key in dic.vButtonDic.Keys)
            {
                if (dic.vButtonDic[key].Id == id)
                {
                    dic.vButtonDic[key].Id = uint.MaxValue;
                    foreach(UIElement ui in vGamePadCanvas.Children)
                    {
                        if (ui.Uid == key)
                        {
                            ChangeButtonStatus(ui, Colors.White, Colors.Black);
                            if (dic.vButtonDic[key].Moving)
                            {
                                if (dic.vButtonDic[key].Top != double.MaxValue)
                                {
                                    ui.ClearValue(Canvas.BottomProperty);
                                    ui.SetValue(Canvas.TopProperty, dic.vButtonDic[key].Top);
                                }
                                if (dic.vButtonDic[key].Left != double.MaxValue)
                                {
                                    ui.ClearValue(Canvas.RightProperty);
                                    ui.SetValue(Canvas.LeftProperty, dic.vButtonDic[key].Left);
                                }
                                if (dic.vButtonDic[key].Bottom != double.MaxValue)
                                {
                                    ui.ClearValue(Canvas.TopProperty);
                                    ui.SetValue(Canvas.BottomProperty, dic.vButtonDic[key].Bottom);
                                }
                                if (dic.vButtonDic[key].Right != double.MaxValue)
                                {
                                    ui.ClearValue(Canvas.LeftProperty);
                                    ui.SetValue(Canvas.RightProperty, dic.vButtonDic[key].Right);
                                }
                            }
                            dic.vButtonDic[key].ExecEvent(vButton.ExecType.Up);
                            break;
                        }
                    }
                    break;
                }
            }
        }

        private void OnPointerMove(Point point, uint id)
        {
            foreach (string key in dic.vButtonDic.Keys)
            {
                if (dic.vButtonDic[key].Id == id && dic.vButtonDic[key].Moving)
                {
                    foreach (UIElement ui in vGamePadCanvas.Children)
                    {
                        if (ui.Uid == key)
                        {
                            Point pos = dic.vButtonDic[key].GetPosition(point);
                            double width = (double)ui.GetValue(WidthProperty);
                            double height = (double)ui.GetValue(HeightProperty);
                            ui.SetValue(Canvas.LeftProperty, pos.X - width / 2);
                            ui.SetValue(Canvas.TopProperty, pos.Y - height / 2);
                            dic.vButtonDic[key].ExecEvent(vButton.ExecType.Move);
                            break;
                        }
                    }
                    break;
                }
            }
        }

        private void vGamePadCanvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            vGamePadCanvas.CaptureMouse();
            Debug.WriteLine("vGamePadCanvas_MouseDown X:{0} Y:{1}", e.MouseDevice.GetPosition(this).X, e.MouseDevice.GetPosition(this).Y);
            OnPointerDown(e.MouseDevice.GetPosition((UIElement)sender), 1);
        }

        private void vGamePadCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                Debug.WriteLine("vGamePadCanvas_MouseMove X:{0} Y:{1}", e.MouseDevice.GetPosition(this).X, e.MouseDevice.GetPosition(this).Y);
                OnPointerMove(e.MouseDevice.GetPosition((UIElement)sender), 1);
            }
        }

        private void vGamePadCanvas_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Debug.WriteLine("vGamePadCanvas_MouseUp X:{0} Y:{1}", e.MouseDevice.GetPosition(this).X, e.MouseDevice.GetPosition(this).Y);
            OnPointerUp(1);
            vGamePadCanvas.ReleaseMouseCapture();
        }

        private void vGamePadCanvas_TouchDown(object sender, TouchEventArgs e)
        {
            ((Canvas)sender).CaptureTouch(e.TouchDevice);
            OnPointerDown(e.GetTouchPoint((UIElement)sender).Position, (uint)e.GetTouchPoint(this).TouchDevice.Id);
        }

        private void vGamePadCanvas_TouchUp(object sender, TouchEventArgs e)
        {
            OnPointerUp((uint)e.GetTouchPoint((UIElement)sender).TouchDevice.Id);
            ((Canvas)sender).ReleaseTouchCapture(e.TouchDevice);
        }

        private void vGamePadCanvas_TouchMove(object sender, TouchEventArgs e)
        {
            OnPointerMove(e.GetTouchPoint(this).Position, (uint)e.GetTouchPoint(this).TouchDevice.Id);
        }
    }
}
