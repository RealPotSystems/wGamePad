﻿using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using Microsoft.Win32;
using System.Diagnostics;

namespace vGamePad
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
            this.vGamePadBase.Width = screen.Bounds.Width;
            this.vGamePadBase.Visibility = System.Windows.Visibility.Visible;
            this.vGamePadBase.Height = bottom - top + 16;
            this.vGamePadBase.SetValue(Canvas.TopProperty, top - 8);

            // 各イベントハンドラの登録
            SystemEvents.DisplaySettingsChanged += new EventHandler(SystemEvents_DisplaySettingsChanged);

            dic.vButtonDic["AnalogStick0"].DownAction += new EventHandler<vGamePadEventArgs>(DefaultDown);
            dic.vButtonDic["AnalogStick0"].MoveAction += new EventHandler<vGamePadEventArgs>(DefaultMove);
            dic.vButtonDic["AnalogStick0"].UpAction += new EventHandler<vGamePadEventArgs>(DefaultUp);

            dic.vButtonDic["AnalogStick1"].DownAction += new EventHandler<vGamePadEventArgs>(DefaultDown);
            dic.vButtonDic["AnalogStick1"].MoveAction += new EventHandler<vGamePadEventArgs>(DefaultMove);
            dic.vButtonDic["AnalogStick1"].UpAction += new EventHandler<vGamePadEventArgs>(DefaultUp);

            dic.vButtonDic["Button01"].DownAction += new EventHandler<vGamePadEventArgs>(BarrageDown);
            dic.vButtonDic["Button01"].MoveAction += new EventHandler<vGamePadEventArgs>(DefaultMove);
            dic.vButtonDic["Button01"].UpAction += new EventHandler<vGamePadEventArgs>(BarrageUp);

            dic.vButtonDic["Button02"].DownAction += new EventHandler<vGamePadEventArgs>(BarrageDown);
            dic.vButtonDic["Button02"].MoveAction += new EventHandler<vGamePadEventArgs>(DefaultMove);
            dic.vButtonDic["Button02"].UpAction += new EventHandler<vGamePadEventArgs>(BarrageUp);

            dic.vButtonDic["Button03"].DownAction += new EventHandler<vGamePadEventArgs>(BarrageDown);
            dic.vButtonDic["Button03"].MoveAction += new EventHandler<vGamePadEventArgs>(DefaultMove);
            dic.vButtonDic["Button03"].UpAction += new EventHandler<vGamePadEventArgs>(BarrageUp);

            dic.vButtonDic["Button04"].DownAction += new EventHandler<vGamePadEventArgs>(BarrageDown);
            dic.vButtonDic["Button04"].MoveAction += new EventHandler<vGamePadEventArgs>(DefaultMove);
            dic.vButtonDic["Button04"].UpAction += new EventHandler<vGamePadEventArgs>(BarrageUp);

            dic.vButtonDic["Button05"].DownAction += new EventHandler<vGamePadEventArgs>(DefaultDown);
            dic.vButtonDic["Button05"].MoveAction += new EventHandler<vGamePadEventArgs>(DefaultMove);
            dic.vButtonDic["Button05"].UpAction += new EventHandler<vGamePadEventArgs>(DefaultUp);

            dic.vButtonDic["Button06"].DownAction += new EventHandler<vGamePadEventArgs>(DefaultDown);
            dic.vButtonDic["Button06"].MoveAction += new EventHandler<vGamePadEventArgs>(DefaultMove);
            dic.vButtonDic["Button06"].UpAction += new EventHandler<vGamePadEventArgs>(DefaultUp);

            dic.vButtonDic["Button07"].DownAction += new EventHandler<vGamePadEventArgs>(DefaultDown);
            dic.vButtonDic["Button07"].MoveAction += new EventHandler<vGamePadEventArgs>(DefaultMove);
            dic.vButtonDic["Button07"].UpAction += new EventHandler<vGamePadEventArgs>(DefaultUp);

            dic.vButtonDic["Button08"].DownAction += new EventHandler<vGamePadEventArgs>(DefaultDown);
            dic.vButtonDic["Button08"].MoveAction += new EventHandler<vGamePadEventArgs>(DefaultMove);
            dic.vButtonDic["Button08"].UpAction += new EventHandler<vGamePadEventArgs>(DefaultUp);

            dic.vButtonDic["Button09"].DownAction += new EventHandler<vGamePadEventArgs>(DefaultDown);
            dic.vButtonDic["Button09"].MoveAction += new EventHandler<vGamePadEventArgs>(DefaultMove);
            dic.vButtonDic["Button09"].UpAction += new EventHandler<vGamePadEventArgs>(DefaultUp);

            dic.vButtonDic["Button10"].DownAction += new EventHandler<vGamePadEventArgs>(DefaultDown);
            dic.vButtonDic["Button10"].MoveAction += new EventHandler<vGamePadEventArgs>(DefaultMove);
            dic.vButtonDic["Button10"].UpAction += new EventHandler<vGamePadEventArgs>(DefaultUp);

            dic.vButtonDic["Button11"].DownAction += new EventHandler<vGamePadEventArgs>(DefaultDown);
            dic.vButtonDic["Button11"].MoveAction += new EventHandler<vGamePadEventArgs>(DefaultMove);
            dic.vButtonDic["Button11"].UpAction += new EventHandler<vGamePadEventArgs>(DefaultUp);

            dic.vButtonDic["Button12"].DownAction += new EventHandler<vGamePadEventArgs>(DefaultDown);
            dic.vButtonDic["Button12"].MoveAction += new EventHandler<vGamePadEventArgs>(DefaultMove);
            dic.vButtonDic["Button12"].UpAction += new EventHandler<vGamePadEventArgs>(DefaultUp);

            dic.vButtonDic["Button_UP"].DownAction += new EventHandler<vGamePadEventArgs>(DefaultDown);
            dic.vButtonDic["Button_UP"].MoveAction += new EventHandler<vGamePadEventArgs>(DefaultMove);
            dic.vButtonDic["Button_UP"].UpAction += new EventHandler<vGamePadEventArgs>(DefaultUp);

            dic.vButtonDic["Button_DOWN"].DownAction += new EventHandler<vGamePadEventArgs>(DefaultDown);
            dic.vButtonDic["Button_DOWN"].MoveAction += new EventHandler<vGamePadEventArgs>(DefaultMove);
            dic.vButtonDic["Button_DOWN"].UpAction += new EventHandler<vGamePadEventArgs>(DefaultUp);

            dic.vButtonDic["Button_LEFT"].DownAction += new EventHandler<vGamePadEventArgs>(DefaultDown);
            dic.vButtonDic["Button_LEFT"].MoveAction += new EventHandler<vGamePadEventArgs>(DefaultMove);
            dic.vButtonDic["Button_LEFT"].UpAction += new EventHandler<vGamePadEventArgs>(DefaultUp);

            dic.vButtonDic["Button_RIGHT"].DownAction += new EventHandler<vGamePadEventArgs>(DefaultDown);
            dic.vButtonDic["Button_RIGHT"].MoveAction += new EventHandler<vGamePadEventArgs>(DefaultMove);
            dic.vButtonDic["Button_RIGHT"].UpAction += new EventHandler<vGamePadEventArgs>(DefaultUp);

            dic.vButtonDic["Keyboard"].DownAction += new EventHandler<vGamePadEventArgs>(DefaultDown);
            dic.vButtonDic["Keyboard"].MoveAction += new EventHandler<vGamePadEventArgs>(DefaultMove);
            dic.vButtonDic["Keyboard"].UpAction += new EventHandler<vGamePadEventArgs>(KeyboardUp);

            dic.vButtonDic["Crop"].DownAction += new EventHandler<vGamePadEventArgs>(DefaultDown);
            dic.vButtonDic["Crop"].MoveAction += new EventHandler<vGamePadEventArgs>(DefaultMove);
            dic.vButtonDic["Crop"].UpAction += new EventHandler<vGamePadEventArgs>(DefaultUp);

            dic.vButtonDic["Config"].DownAction += new EventHandler<vGamePadEventArgs>(DefaultDown);
            dic.vButtonDic["Config"].MoveAction += new EventHandler<vGamePadEventArgs>(DefaultMove);
            dic.vButtonDic["Config"].UpAction += new EventHandler<vGamePadEventArgs>(DefaultUp);

            dic.vButtonDic["Exit"].DownAction += new EventHandler<vGamePadEventArgs>(DefaultDown);
            dic.vButtonDic["Exit"].MoveAction += new EventHandler<vGamePadEventArgs>(DefaultMove);
            dic.vButtonDic["Exit"].UpAction += new EventHandler<vGamePadEventArgs>(ExitUp);

            dic.vButtonDic["Home"].DownAction += new EventHandler<vGamePadEventArgs>(HomeDown);
            dic.vButtonDic["Home"].MoveAction += new EventHandler<vGamePadEventArgs>(HomeMove);
            dic.vButtonDic["Home"].UpAction += new EventHandler<vGamePadEventArgs>(HomeUp);
            // Sample
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
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
