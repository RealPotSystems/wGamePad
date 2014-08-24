using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using Microsoft.Win32;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Xml;
using DeviceControl;

namespace vGamePad
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        private static vButtonDictionary dic = null;
        public static JoyStick devCon = new JoyStick();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Initialized(object sender, EventArgs e)
        {
            var screen = System.Windows.Forms.Screen.PrimaryScreen;
            Left = screen.Bounds.Left;
            Top = screen.Bounds.Top;
            Width = screen.Bounds.Width;
            Height = screen.Bounds.Height;

            // ここにこのロジックを置くべきか？
            dic = vLayoutControl.LoadLayout(0);

            double top = double.MaxValue;
            double bottom = double.MinValue;

            foreach (UIElement ui in vGamePadCanvas.Children)
            {
                foreach (string key in dic.Keys)
                {
                    if (ui.Uid == key)
                    {
                        var button = dic[key];

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
                            if ((Height - button.Bottom) > bottom)
                                bottom = Height - button.Bottom;
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
            vGamePadBase.Height = bottom - top + App.GRID * 2;
            vGamePadBase.SetValue(Canvas.TopProperty, top - App.GRID);

            // 各イベントハンドラの登録
            SystemEvents.DisplaySettingsChanged += new EventHandler(SystemEvents_DisplaySettingsChanged);

            dic["AnalogStick0"].DownAction += new EventHandler<vGamePadEventArgs>(AnalogStickDown);
            dic["AnalogStick0"].MoveAction += new EventHandler<vGamePadEventArgs>(AnalogStickMove);
            dic["AnalogStick0"].UpAction += new EventHandler<vGamePadEventArgs>(AnalogStickUp);

            dic["AnalogStick1"].DownAction += new EventHandler<vGamePadEventArgs>(AnalogStickDown);
            dic["AnalogStick1"].MoveAction += new EventHandler<vGamePadEventArgs>(AnalogStickMove);
            dic["AnalogStick1"].UpAction += new EventHandler<vGamePadEventArgs>(AnalogStickUp);

            dic["Button01"].DownAction += new EventHandler<vGamePadEventArgs>(BarrageDown);
            dic["Button01"].UpAction += new EventHandler<vGamePadEventArgs>(BarrageUp);

            dic["Button02"].DownAction += new EventHandler<vGamePadEventArgs>(BarrageDown);
            dic["Button02"].UpAction += new EventHandler<vGamePadEventArgs>(BarrageUp);

            dic["Button03"].DownAction += new EventHandler<vGamePadEventArgs>(BarrageDown);
            dic["Button03"].UpAction += new EventHandler<vGamePadEventArgs>(BarrageUp);

            dic["Button04"].DownAction += new EventHandler<vGamePadEventArgs>(BarrageDown);
            dic["Button04"].UpAction += new EventHandler<vGamePadEventArgs>(BarrageUp);

            dic["Button05"].DownAction += new EventHandler<vGamePadEventArgs>(ButtonDown);
            dic["Button05"].UpAction += new EventHandler<vGamePadEventArgs>(ButtonUp);

            dic["Button06"].DownAction += new EventHandler<vGamePadEventArgs>(ButtonDown);
            dic["Button06"].UpAction += new EventHandler<vGamePadEventArgs>(ButtonUp);

            dic["Button07"].DownAction += new EventHandler<vGamePadEventArgs>(ButtonDown);
            dic["Button07"].UpAction += new EventHandler<vGamePadEventArgs>(ButtonUp);

            dic["Button08"].DownAction += new EventHandler<vGamePadEventArgs>(ButtonDown);
            dic["Button08"].UpAction += new EventHandler<vGamePadEventArgs>(ButtonUp);

            dic["Button09"].DownAction += new EventHandler<vGamePadEventArgs>(ButtonDown);
            dic["Button09"].UpAction += new EventHandler<vGamePadEventArgs>(ButtonUp);

            dic["Button10"].DownAction += new EventHandler<vGamePadEventArgs>(ButtonDown);
            dic["Button10"].UpAction += new EventHandler<vGamePadEventArgs>(ButtonUp);

            dic["Button11"].DownAction += new EventHandler<vGamePadEventArgs>(ButtonDown);
            dic["Button11"].UpAction += new EventHandler<vGamePadEventArgs>(ButtonUp);

            dic["Button12"].DownAction += new EventHandler<vGamePadEventArgs>(ButtonDown);
            dic["Button12"].UpAction += new EventHandler<vGamePadEventArgs>(ButtonUp);

            dic["Button_UP"].DownAction += new EventHandler<vGamePadEventArgs>(CrossDown);
            dic["Button_UP"].UpAction += new EventHandler<vGamePadEventArgs>(CrossUp);

            dic["Button_DOWN"].DownAction += new EventHandler<vGamePadEventArgs>(CrossDown);
            dic["Button_DOWN"].UpAction += new EventHandler<vGamePadEventArgs>(CrossUp);

            dic["Button_LEFT"].DownAction += new EventHandler<vGamePadEventArgs>(CrossDown);
            dic["Button_LEFT"].UpAction += new EventHandler<vGamePadEventArgs>(CrossUp);

            dic["Button_RIGHT"].DownAction += new EventHandler<vGamePadEventArgs>(CrossDown);
            dic["Button_RIGHT"].UpAction += new EventHandler<vGamePadEventArgs>(CrossUp);

            dic["Keyboard"].DownAction += new EventHandler<vGamePadEventArgs>(DefaultDown);
            dic["Keyboard"].UpAction += new EventHandler<vGamePadEventArgs>(KeyboardUp);

            dic["Crop"].DownAction += new EventHandler<vGamePadEventArgs>(DefaultDown);
            dic["Crop"].UpAction += new EventHandler<vGamePadEventArgs>(CropUp);

            dic["Config"].DownAction += new EventHandler<vGamePadEventArgs>(DefaultDown);
            dic["Config"].UpAction += new EventHandler<vGamePadEventArgs>(ConfigUp);

            dic["Exit"].DownAction += new EventHandler<vGamePadEventArgs>(DefaultDown);
            dic["Exit"].UpAction += new EventHandler<vGamePadEventArgs>(ExitUp);

            dic["Home"].DownAction += new EventHandler<vGamePadEventArgs>(HomeDown);
            dic["Home"].MoveAction += new EventHandler<vGamePadEventArgs>(HomeMove);
            dic["Home"].UpAction += new EventHandler<vGamePadEventArgs>(HomeUp);
            // Sample

#if DEBUG
            // デバッグ用イベントハンドラは以下の３つにしておく
            // MouseDown="vGamePadCanvas_MouseDown"
            // MouseMove="vGamePadCanvas_MouseMove"
            // MouseUp="vGamePadCanvas_MouseUp"
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
            foreach (string key in dic.Keys)
            {
                if (dic[key].vHitTest(point))
                {
                    foreach (UIElement ui in vGamePadCanvas.Children)
                    {
                        if (ui.Uid == key)
                        {
                            // イベントハンドラが登録されている場合はDownイベントを実行する
                            dic[key].ActionEvent(vButton.ActionType.Down, ui, point, id);
                            break;
                        }
                    }
                    break;
                }
            }
        }

        private void OnPointerUp(Point point, int id)
        {
            foreach (string key in dic.Keys)
            {
                if (dic[key].Id == id)
                {
                    dic[key].Id = int.MinValue;
                    foreach (UIElement ui in vGamePadCanvas.Children)
                    {
                        if (ui.Uid == key)
                        {
                            dic[key].ActionEvent(vButton.ActionType.Up, ui, point);
                            break;
                        }
                    }
                    break;
                }
            }
        }

        private void OnPointerMove(Point point, int id)
        {
            foreach (string key in dic.Keys)
            {
                if (dic[key].Id == id && dic[key].Moving)
                {
                    foreach (UIElement ui in vGamePadCanvas.Children)
                    {
                        if (ui.Uid == key)
                        {
                            dic[key].ActionEvent(vButton.ActionType.Move, ui, point);
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
