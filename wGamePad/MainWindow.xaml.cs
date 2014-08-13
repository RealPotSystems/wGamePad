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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Interop;

namespace wGamePad
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        vButtonDictionay dic = null;

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
            // Sample
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            WindowInteropHelper helper = new WindowInteropHelper(this);
            SetWindowLong(helper.Handle, GWL_EXSTYLE, GetWindowLong(helper.Handle, GWL_EXSTYLE) | WS_EX_NOACTIVATE);
            HwndSource souce = HwndSource.FromHwnd(helper.Handle);
            souce.AddHook(WndProc);
        }

        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            switch (msg)
            {
                case WM_POINTERDOWN:
                    OnPointerDown(PointFromScreen(new System.Windows.Point(GET_X_LPARAM(lParam),GET_Y_LPARAM(lParam))),GET_POINTERID_WPARAM(wParam));
                    break;
                case WM_POINTERUP:
                    OnPointerUp(GET_POINTERID_WPARAM(wParam));
                    break;
                case WM_POINTERUPDATE:
                    // OnPointerUpdate(GET_POINTERID_WPARAM(wParam));
                    break;
                case WM_LBUTTONUP:
                    foreach (string key in dic.vButtonDic.Keys)
                    {
                        var button = dic.vButtonDic[key];
                        if (button.Id == 1)
                        {
                            button.Id = uint.MaxValue;
                            foreach (UIElement ui in vGamePadCanvas.Children)
                            {
                                if (ui.Uid == key)
                                {
                                    // 反転
                                    foreach (object child in ((Grid)ui).Children)
                                    {
                                        if (child is Ellipse)
                                        {
                                            ((Ellipse)child).Fill = new SolidColorBrush(Colors.White);
                                        }
                                        if (child is Label)
                                        {
                                            ((Label)child).Foreground = new SolidColorBrush(Colors.Black);
                                        }
                                    }
                                    break;
                                }
                            }
                            break;
                        }
                    }
                    break;

            }
            return IntPtr.Zero;
        }

        private void OnPointerDown(System.Windows.Point point, uint id)
        {
            foreach (string key in dic.vButtonDic.Keys)
            {
                var button = dic.vButtonDic[key];
                if (button.hitTest(point) == true)
                {
                    foreach (UIElement ui in vGamePadCanvas.Children)
                    {
                        if (ui.Uid == key)
                        {
                            // 反転
                            foreach (object child in ((Grid)ui).Children)
                            {
                                if (child is Ellipse)
                                {
                                    ((Ellipse)child).Fill = new SolidColorBrush(Colors.Black);
                                }
                                if (child is Label)
                                {
                                    ((Label)child).Foreground = new SolidColorBrush(Colors.White);
                                }
                            }

                            button.Id = id;
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
                var button = dic.vButtonDic[key];
                if (button.Id == id)
                {
                    button.Id = uint.MaxValue;
                }
            }
        }

        //private void vGamePadCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        //{
        //    Point pt = e.GetPosition((UIElement)sender);
        //    HitTestResult result = VisualTreeHelper.HitTest((UIElement)sender, pt);

        //    if (result != null)
        //    {
        //        // TODO:
        //    }
        //}


        //// ボタンタッチ
        //private void Button_TouchDown(object sender, TouchEventArgs e)
        //{
        //    // 反転
        //    foreach (object child in ((Grid)sender).Children)
        //    {
        //        if (child is Ellipse)
        //        {
        //            ((Ellipse)child).Fill = new SolidColorBrush(Colors.Black);
        //        }
        //        if (child is Label)
        //        {
        //            ((Label)child).Foreground = new SolidColorBrush(Colors.White);
        //        }
        //    }

        //    // タッチ音を鳴らす

        //    // 押下されたボタンを判定する

        //}

        //private void Button_TouchUp(object sender, TouchEventArgs e)
        //{
        //    // 戻し
        //    foreach (object child in ((Grid)sender).Children)
        //    {
        //        if (child is Ellipse)
        //        {
        //            ((Ellipse)child).Fill = new SolidColorBrush(Colors.White);
        //        }
        //        if (child is Label)
        //        {
        //            ((Label)child).Foreground = new SolidColorBrush(Colors.Black);
        //        }
        //    }
        //}
    }
}
