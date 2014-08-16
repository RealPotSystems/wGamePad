using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using Microsoft.Win32;
using System.Diagnostics;
using DeviceControl;

namespace vGamePad
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        private static vButtonDictionay dic = null;
        private static JoyStick devCon = new JoyStick();

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

            double top = double.MaxValue;
            double bottom = double.MinValue;

            foreach (UIElement ui in vGamePadCanvas.Children)
            {
                foreach (string key in dic.vButtonDic.Keys)
                {
                    if (ui.Uid == key)
                    {
                        var button = dic.vButtonDic[key];

                        button.Width = (double)ui.GetValue(WidthProperty);
                        button.Height = (double)ui.GetValue(HeightProperty);

                        if (button.Top != double.MaxValue)
                        {
                            ui.SetValue(Canvas.TopProperty, button.Top);
                            if (button.Top < top)
                                top = button.Top;
                            if ((button.Top + button.Height) > bottom)
                                bottom = button.Top + button.Height;
                        }
                        if (button.Left != double.MaxValue)
                        {
                            ui.SetValue(Canvas.LeftProperty, button.Left);
                        }
                        if (button.Bottom != double.MaxValue)
                        {
                            ui.SetValue(Canvas.BottomProperty, button.Bottom);
                            if ((button.Height - button.Bottom) < top)
                                top = button.Height - button.Bottom;
                            if ((this.Height - button.Bottom) > bottom)
                                bottom = this.Height - button.Bottom;
                        }
                        if (button.Right != double.MaxValue)
                        {
                            ui.SetValue(Canvas.RightProperty, button.Right);
                        }
                        ui.SetValue(Canvas.VisibilityProperty, button.Visible);
                        break;
                    }
                }
            }
            // 一番上にあるボタンと一番下にあるボタンの下を求める
            vGamePadBase.Width = screen.Bounds.Width;
            vGamePadBase.Visibility = System.Windows.Visibility.Visible;
            vGamePadBase.Height = bottom - top + 16;
            vGamePadBase.SetValue(Canvas.TopProperty, top - 8);

            // 各イベントハンドラの登録
            SystemEvents.DisplaySettingsChanged += new EventHandler(SystemEvents_DisplaySettingsChanged);

            dic.vButtonDic["AnalogStick0"].DownAction += new EventHandler<vGamePadEventArgs>(AnalogStickDown);
            dic.vButtonDic["AnalogStick0"].MoveAction += new EventHandler<vGamePadEventArgs>(AnalogStickMove);
            dic.vButtonDic["AnalogStick0"].UpAction += new EventHandler<vGamePadEventArgs>(AnalogStickUp);

            dic.vButtonDic["AnalogStick1"].DownAction += new EventHandler<vGamePadEventArgs>(AnalogStickDown);
            dic.vButtonDic["AnalogStick1"].MoveAction += new EventHandler<vGamePadEventArgs>(AnalogStickMove);
            dic.vButtonDic["AnalogStick1"].UpAction += new EventHandler<vGamePadEventArgs>(AnalogStickUp);

            dic.vButtonDic["Button01"].DownAction += new EventHandler<vGamePadEventArgs>(BarrageDown);
            dic.vButtonDic["Button01"].UpAction += new EventHandler<vGamePadEventArgs>(BarrageUp);

            dic.vButtonDic["Button02"].DownAction += new EventHandler<vGamePadEventArgs>(BarrageDown);
            dic.vButtonDic["Button02"].UpAction += new EventHandler<vGamePadEventArgs>(BarrageUp);

            dic.vButtonDic["Button03"].DownAction += new EventHandler<vGamePadEventArgs>(BarrageDown);
            dic.vButtonDic["Button03"].UpAction += new EventHandler<vGamePadEventArgs>(BarrageUp);

            dic.vButtonDic["Button04"].DownAction += new EventHandler<vGamePadEventArgs>(BarrageDown);
            dic.vButtonDic["Button04"].UpAction += new EventHandler<vGamePadEventArgs>(BarrageUp);

            dic.vButtonDic["Button05"].DownAction += new EventHandler<vGamePadEventArgs>(ButtonDown);
            dic.vButtonDic["Button05"].UpAction += new EventHandler<vGamePadEventArgs>(ButtonUp);

            dic.vButtonDic["Button06"].DownAction += new EventHandler<vGamePadEventArgs>(ButtonDown);
            dic.vButtonDic["Button06"].UpAction += new EventHandler<vGamePadEventArgs>(ButtonUp);

            dic.vButtonDic["Button07"].DownAction += new EventHandler<vGamePadEventArgs>(ButtonDown);
            dic.vButtonDic["Button07"].UpAction += new EventHandler<vGamePadEventArgs>(ButtonUp);

            dic.vButtonDic["Button08"].DownAction += new EventHandler<vGamePadEventArgs>(ButtonDown);
            dic.vButtonDic["Button08"].UpAction += new EventHandler<vGamePadEventArgs>(ButtonUp);

            dic.vButtonDic["Button09"].DownAction += new EventHandler<vGamePadEventArgs>(ButtonDown);
            dic.vButtonDic["Button09"].UpAction += new EventHandler<vGamePadEventArgs>(ButtonUp);

            dic.vButtonDic["Button10"].DownAction += new EventHandler<vGamePadEventArgs>(ButtonDown);
            dic.vButtonDic["Button10"].UpAction += new EventHandler<vGamePadEventArgs>(ButtonUp);

            dic.vButtonDic["Button11"].DownAction += new EventHandler<vGamePadEventArgs>(ButtonDown);
            dic.vButtonDic["Button11"].UpAction += new EventHandler<vGamePadEventArgs>(ButtonUp);

            dic.vButtonDic["Button12"].DownAction += new EventHandler<vGamePadEventArgs>(ButtonDown);
            dic.vButtonDic["Button12"].UpAction += new EventHandler<vGamePadEventArgs>(ButtonUp);

            dic.vButtonDic["Button_UP"].DownAction += new EventHandler<vGamePadEventArgs>(CrossDown);
            dic.vButtonDic["Button_UP"].UpAction += new EventHandler<vGamePadEventArgs>(CrossUp);

            dic.vButtonDic["Button_DOWN"].DownAction += new EventHandler<vGamePadEventArgs>(CrossDown);
            dic.vButtonDic["Button_DOWN"].UpAction += new EventHandler<vGamePadEventArgs>(CrossUp);

            dic.vButtonDic["Button_LEFT"].DownAction += new EventHandler<vGamePadEventArgs>(CrossDown);
            dic.vButtonDic["Button_LEFT"].UpAction += new EventHandler<vGamePadEventArgs>(CrossUp);

            dic.vButtonDic["Button_RIGHT"].DownAction += new EventHandler<vGamePadEventArgs>(CrossDown);
            dic.vButtonDic["Button_RIGHT"].UpAction += new EventHandler<vGamePadEventArgs>(CrossUp);

            dic.vButtonDic["Keyboard"].DownAction += new EventHandler<vGamePadEventArgs>(DefaultDown);
            dic.vButtonDic["Keyboard"].UpAction += new EventHandler<vGamePadEventArgs>(KeyboardUp);

            dic.vButtonDic["Crop"].DownAction += new EventHandler<vGamePadEventArgs>(DefaultDown);
            dic.vButtonDic["Crop"].UpAction += new EventHandler<vGamePadEventArgs>(CropUp);

            dic.vButtonDic["Config"].DownAction += new EventHandler<vGamePadEventArgs>(DefaultDown);
            dic.vButtonDic["Config"].UpAction += new EventHandler<vGamePadEventArgs>(ConfigUp);

            dic.vButtonDic["Exit"].DownAction += new EventHandler<vGamePadEventArgs>(DefaultDown);
            dic.vButtonDic["Exit"].UpAction += new EventHandler<vGamePadEventArgs>(ExitUp);

            dic.vButtonDic["Home"].DownAction += new EventHandler<vGamePadEventArgs>(HomeDown);
            dic.vButtonDic["Home"].MoveAction += new EventHandler<vGamePadEventArgs>(HomeMove);
            dic.vButtonDic["Home"].UpAction += new EventHandler<vGamePadEventArgs>(HomeUp);
            // Sample

            // デバッグ用イベントハンドラ
            // MouseDown="vGamePadCanvas_MouseDown" MouseMove="vGamePadCanvas_MouseMove" MouseUp="vGamePadCanvas_MouseUp"
#if DEBUG
            vGamePadCanvas.MouseDown += vGamePadCanvas_MouseDown;
            vGamePadCanvas.MouseMove += vGamePadCanvas_MouseMove;
            vGamePadCanvas.MouseUp += vGamePadCanvas_MouseUp;
#endif
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                devCon.Initialize();
                devCon.MoveStick(0, 50, 50);    // とりあえず中央に
                devCon.MoveStick(1, 50, 50);    // とりあえず中央に
            }
            catch(Exception ex)
            {
                // ダイアログメッセージを表示する
                vGamePad.DialogWindow.DialogWindow dialog = new vGamePad.DialogWindow.DialogWindow(
                    Properties.Resources.DialogTitle,
                    ex.Message);
                dialog.ShowDialog();
                Close();
            }
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

        private void OnPointerDown(Point point, int id)
        {
            foreach (string key in dic.vButtonDic.Keys)
            {
                if (dic.vButtonDic[key].vHitTest(point))
                {
                    foreach (UIElement ui in vGamePadCanvas.Children)
                    {
                        if (ui.Uid == key)
                        {
                            // イベントハンドラが登録されている場合はDownイベントを実行する
                            dic.vButtonDic[key].ActionEvent(vButton.ActionType.Down, ui, point, id);
                            break;
                        }
                    }
                    break;
                }
            }
        }

        private void OnPointerUp(Point point, int id)
        {
            foreach (string key in dic.vButtonDic.Keys)
            {
                if (dic.vButtonDic[key].Id == id)
                {
                    dic.vButtonDic[key].Id = int.MinValue;
                    foreach (UIElement ui in vGamePadCanvas.Children)
                    {
                        if (ui.Uid == key)
                        {
                            dic.vButtonDic[key].ActionEvent(vButton.ActionType.Up, ui, point);
                            break;
                        }
                    }
                    break;
                }
            }
        }

        private void OnPointerMove(Point point, int id)
        {
            foreach (string key in dic.vButtonDic.Keys)
            {
                if (dic.vButtonDic[key].Id == id && dic.vButtonDic[key].Moving)
                {
                    foreach (UIElement ui in vGamePadCanvas.Children)
                    {
                        if (ui.Uid == key)
                        {
                            dic.vButtonDic[key].ActionEvent(vButton.ActionType.Move, ui, point);
                            break;
                        }
                    }
                    break;
                }
            }
        }

#if DEBUG
        private void vGamePadCanvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            ((Canvas)sender).CaptureMouse();
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
            OnPointerUp(e.MouseDevice.GetPosition((UIElement)sender), 1);
            ((Canvas)sender).ReleaseMouseCapture();
        }
#endif
        private void vGamePadCanvas_TouchDown(object sender, TouchEventArgs e)
        {
            ((Canvas)sender).CaptureTouch(e.TouchDevice);
            OnPointerDown(e.GetTouchPoint((UIElement)sender).Position, e.GetTouchPoint(this).TouchDevice.Id);
        }

        private void vGamePadCanvas_TouchUp(object sender, TouchEventArgs e)
        {
            OnPointerUp(e.GetTouchPoint(this).Position, e.GetTouchPoint((UIElement)sender).TouchDevice.Id);
            ((Canvas)sender).ReleaseTouchCapture(e.TouchDevice);
        }

        private void vGamePadCanvas_TouchMove(object sender, TouchEventArgs e)
        {
            OnPointerMove(e.GetTouchPoint(this).Position, e.GetTouchPoint(this).TouchDevice.Id);
        }
    }
}
