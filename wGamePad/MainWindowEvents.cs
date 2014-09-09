//------------------------------------------------------------
//
//  仮想ゲームパッド vGamePad
//  © 2014,RealPotSystems(TAKUBON). All rights reserverd.
//
//  Workfile : MainWindowEvents.cs
//  Author   : TAKUYA MANABE(manataku@me.com)
//
//------------------------------------------------------------
using System;
using System.Windows;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Forms;
using System.Diagnostics;
using System.Windows.Threading;
using Microsoft.VisualBasic;

namespace vGamePad
{
    public partial class MainWindow : Window
    {
        internal class BarrageTimer : DispatcherTimer
        {
            public UIElement Ui { set; get; }
            public BarrageTimer(UIElement ui)
            {
                Ui = ui;
            }
        }

        internal Dictionary<string, BarrageTimer> vButtonTimerDic = new Dictionary<string, BarrageTimer>();

        private void SystemEvents_DisplaySettingsChanged(object sender, EventArgs e)
        {
            var dpi = ViewExtensions.GetDpiScaleFactor(this);
            var screen = System.Windows.Forms.Screen.PrimaryScreen;
            Top = 0;
            Left = 0;
            Width = (int)(screen.Bounds.Width / dpi.X);
            Height = (int)(screen.Bounds.Height / dpi.Y);
        }

        public void DefaultDown(object sender, vGamePadEventArgs e)
        {
            // 反転
            ChangeButtonStatus(e.ui, Colors.White, Colors.Black);
            // 音を鳴らす
            PlayButtonSound.Play();

            dic[e.ui.Uid].Id = e.id;

            // 移動可能なコントロール
            if (dic[e.ui.Uid].Moving)
            {
                // この座標を中心点にする
                // 中心点の計算
                double width = (double)e.ui.GetValue(WidthProperty);
                double height = (double)e.ui.GetValue(HeightProperty);
                e.ui.SetValue(Canvas.LeftProperty, e.point.X - width / 2);
                e.ui.SetValue(Canvas.TopProperty, e.point.Y - height / 2);
            }
        }

        public void DefaultUp(object sender, vGamePadEventArgs e)
        {
            ChangeButtonStatus(e.ui, Colors.Black, Colors.White);
            if (dic[e.ui.Uid].Moving)
            {
                if (dic[e.ui.Uid].Top != double.MaxValue)
                {
                    e.ui.ClearValue(Canvas.BottomProperty);
                    e.ui.SetValue(Canvas.TopProperty, dic[e.ui.Uid].Top);
                }
                if (dic[e.ui.Uid].Left != double.MaxValue)
                {
                    e.ui.ClearValue(Canvas.RightProperty);
                    e.ui.SetValue(Canvas.LeftProperty, dic[e.ui.Uid].Left);
                }
                if (dic[e.ui.Uid].Bottom != double.MaxValue)
                {
                    e.ui.ClearValue(Canvas.TopProperty);
                    e.ui.SetValue(Canvas.BottomProperty, dic[e.ui.Uid].Bottom);
                }
                if (dic[e.ui.Uid].Right != double.MaxValue)
                {
                    e.ui.ClearValue(Canvas.LeftProperty);
                    e.ui.SetValue(Canvas.RightProperty, dic[e.ui.Uid].Right);
                }
            }
        }

        public void DefaultMove(object sender, vGamePadEventArgs e)
        {
            Point pos = dic[e.ui.Uid].vGetPosition(e.point);
            double width = (double)e.ui.GetValue(WidthProperty);
            double height = (double)e.ui.GetValue(HeightProperty);
            e.ui.ClearValue(Canvas.RightProperty);
            e.ui.ClearValue(Canvas.BottomProperty);
            e.ui.SetValue(Canvas.LeftProperty, pos.X - width / 2);
            e.ui.SetValue(Canvas.TopProperty, pos.Y - height / 2);
        }
        
        public void AnalogStickDown(object sender, vGamePadEventArgs e)
        {
            devCon.MoveStick(dic[e.ui.Uid].Index, dic[e.ui.Uid].vGetAxisX(e.point.X), dic[e.ui.Uid].vGetAxisY(e.point.Y));
            DefaultDown(sender, e);
        }

        public void AnalogStickUp(object sender, vGamePadEventArgs e)
        {
            devCon.MoveStick(dic[e.ui.Uid].Index, dic[e.ui.Uid].vGetAxisX(double.MaxValue), dic[e.ui.Uid].vGetAxisY(double.MaxValue));
            DefaultUp(sender, e);
        }

        public void AnalogStickMove(object sender, vGamePadEventArgs e)
        {
            Point pos = dic[e.ui.Uid].vGetPosition(e.point);
            devCon.MoveStick(dic[e.ui.Uid].Index, dic[e.ui.Uid].vGetAxisX(pos.X), dic[e.ui.Uid].vGetAxisY(pos.Y));
            DefaultMove(sender, e);
        }

        public void CrossDown(object sender, vGamePadEventArgs e)
        {
            devCon.PushCross(dic[e.ui.Uid].Index);
            DefaultDown(sender, e);
        }

        public void CrossUp(object sender, vGamePadEventArgs e)
        {
            devCon.FreeCross(dic[e.ui.Uid].Index);
            DefaultUp(sender, e);
        }

        public void ButtonDown(object sender, vGamePadEventArgs e)
        {
            devCon.PushButton(dic[e.ui.Uid].Index);
            DefaultDown(sender, e);
        }

        public void ButtonUp(object sender, vGamePadEventArgs e)
        {
            devCon.FreeButton(dic[e.ui.Uid].Index);
            DefaultUp(sender, e);
        }

        public void BarrageDown(object sender, vGamePadEventArgs e)
        {
            // デフォルト動作
            DefaultDown(sender, e);

            // vJoyボタンダウン
            devCon.PushButton(dic[e.ui.Uid].Index);

            // タイマーオブジェクトの有無を確認
            if (!vButtonTimerDic.ContainsKey(e.ui.Uid))
            {
                vButtonTimerDic.Add(e.ui.Uid, new BarrageTimer(e.ui));
            }
            // 対象のボタンのタイマー状態で動作を切り分け
            switch (dic[e.ui.Uid].Barrage)
            {
                case vButton.BarrageState.TimerOn:
                    vButtonTimerDic[e.ui.Uid].Tick -= new EventHandler(BarrageTimerEvent);
                    dic[e.ui.Uid].Barrage = vButton.BarrageState.TimerOff;
                    vButtonTimerDic[e.ui.Uid].Stop();
                    break;
                case vButton.BarrageState.None:
                case vButton.BarrageState.TimerOff:
                    // 2秒間のタイマーセット
                    vButtonTimerDic[e.ui.Uid].Interval = new TimeSpan(0, 0, 2);
                    vButtonTimerDic[e.ui.Uid].Tick += new EventHandler(BarrageTimerEvent);
                    dic[e.ui.Uid].Barrage = vButton.BarrageState.WaitTimeout;
                    vButtonTimerDic[e.ui.Uid].Start();
                    break;
                case vButton.BarrageState.WaitTimeout:
                    break;
            }
        }

        public void BarrageUp(object sender, vGamePadEventArgs e)
        {
            // デフォルト動作は不要
            // vJoyボタンアップ
            devCon.FreeButton(dic[e.ui.Uid].Index);

            // タイマーオブジェクトの有無を確認(※そもそもBarrageDownが先にくるが...)
            if (!vButtonTimerDic.ContainsKey(e.ui.Uid))
            {
                vButtonTimerDic.Add(e.ui.Uid, new BarrageTimer(e.ui));
            }
            // 対象のボタンタイマー状態で動作を切り分け
            switch (dic[e.ui.Uid].Barrage)
            {
                case vButton.BarrageState.None:
                case vButton.BarrageState.TimerOff:
                    ChangeButtonStatus(e.ui, Colors.Black, Colors.White);
                    break;
                case vButton.BarrageState.WaitTimeout:
                    vButtonTimerDic[e.ui.Uid].Tick -= new EventHandler(BarrageTimerEvent);
                    dic[e.ui.Uid].Barrage = vButton.BarrageState.TimerOff;
                    vButtonTimerDic[e.ui.Uid].Stop();
                    ChangeButtonStatus(e.ui, Colors.Black, Colors.White);
                    break;
            }
        }

        private void BarrageTimerEvent(object sender, EventArgs e)
        {
            Debug.WriteLine("BarrageTimerEvent:{0}",((BarrageTimer)sender).Ui.Uid);

            if (dic[((BarrageTimer)sender).Ui.Uid].Barrage == vButton.BarrageState.WaitTimeout)
            {
                // タイマーステータス TimerOn
                dic[((BarrageTimer)sender).Ui.Uid].Barrage = vButton.BarrageState.TimerOn;
                // インターバル 0.1秒
                ((BarrageTimer)sender).Interval = new TimeSpan(0, 0, 0, 0, 100);
                // 画面ボタン赤に
                ChangeButtonStatus(((BarrageTimer)sender).Ui, Colors.Red, Colors.White);
                // 音を２回鳴らす
                PlayButtonSound.Play(PlayButtonSound.PlayType.Sync);
                PlayButtonSound.Play();
            }
            // vJoyボタンダウン
            devCon.PushButton(dic[((BarrageTimer)sender).Ui.Uid].Index);
            // vJoyボタンアップ
            devCon.FreeButton(dic[((BarrageTimer)sender).Ui.Uid].Index);
        }

        // キーボードの起動
        public void KeyboardUp(object sender, vGamePadEventArgs e)
        {
            DefaultUp(sender, e);
            var screenKeyboard = Environment.ExpandEnvironmentVariables(Properties.Resources.TabTip);
            try
            {
                Process[] ps = Process.GetProcessesByName("DQXGame");
                if ( ps.Length == 1 )
                {
                    Interaction.AppActivate(ps[0].Id);
                }
                ps = Process.GetProcessesByName("DQXLauncher");
                if (ps.Length == 1)
                {
                    Interaction.AppActivate(ps[0].Id);
                }
                Process proc = Process.Start(screenKeyboard);
                proc.Close();
            }
            catch
            {
            }
        }

        // コンフィグ
        public void ConfigUp(object sender, vGamePadEventArgs e)
        {
            DefaultUp(sender, e);
            // 環境設定ダイアログの表示
            ConfigWindow conf = new ConfigWindow();
            Hide();
            conf.ShowDialog();
            SetConfig();
            Show();
        }

        // 切り取り
        public void CropUp(object sender, vGamePadEventArgs e)
        {
            DefaultUp(sender, e);
            // 画面の切り取り
        }

        // クローズ
        public void ExitUp(object sender, vGamePadEventArgs e)
        {
            DefaultUp(sender, e);
            // 確認ダイアログを表示する
            DialogWindow.DialogWindow dialog = new DialogWindow.DialogWindow(
                Properties.Resources.DialogTitle,
                Properties.Resources.ExitApplication,
                DialogWindow.DialogWindow.DialogStyle.OKCANCEL);
            Hide();
            bool? result = dialog.ShowDialog();
            if (result == true)
            {
                Close();
                return;
            }
            Show();
        }

        public void HomeDown(object sender, vGamePadEventArgs e)
        {
            // 反転
            ChangeButtonStatus(e.ui, Colors.White, Colors.Black);

            //　音
            PlayButtonSound.Play();

            dic[e.ui.Uid].Id = e.id;
            dic[e.ui.Uid].Range = Height;
            vGamePadBaseLeft.Background = new SolidColorBrush(Colors.White);
            vGamePadBaseRight.Background = new SolidColorBrush(Colors.White);
        }

        public void HomeMove(object sender, vGamePadEventArgs e)
        {
            double posY = ((int)(e.point.Y / App.GRID)) * App.GRID;
            if (posY > Height - (double)e.ui.GetValue(HeightProperty))
                posY = Height - (double)e.ui.GetValue(HeightProperty);
            e.ui.SetValue(Canvas.TopProperty, posY);
            
            vGamePadBaseLeft.SetValue(Canvas.TopProperty, posY);
            vGamePadBaseRight.SetValue(Canvas.TopProperty, posY);
        }

        public void HomeUp(object sender, vGamePadEventArgs e)
        {
            vGamePadBaseLeft.Background = new SolidColorBrush(Colors.Black);
            vGamePadBaseRight.Background = new SolidColorBrush(Colors.Black);
            ChangeButtonStatus(e.ui, Colors.Black, Colors.White);

            double posY = ((int)(e.point.Y / App.GRID)) * App.GRID;
            if (posY > Height - (double)e.ui.GetValue(HeightProperty))
                posY = Height - (double)e.ui.GetValue(HeightProperty);

            double distance = posY - dic[e.ui.Uid].Top;
            // Homeを基準に移動
            foreach (UIElement ui in vGamePadCanvas.Children)
            {
                foreach (string key in dic.Keys)
                {
                    var value = dic[key];
                    if (ui.Uid == key && value.Fixed == false && value.Visible == System.Windows.Visibility.Visible)
                    {
                        if (value.Top != double.MaxValue)
                        {
                            value.Top += distance;
                            ui.SetValue(Canvas.TopProperty, value.Top);
                        }
                        if (value.Bottom != double.MaxValue)
                        {
                            value.Bottom += distance;
                            ui.SetValue(Canvas.BottomProperty, value.Bottom);
                        }
                    }
                }
            }
            dic[e.ui.Uid].Top = posY;
        }
    }
}