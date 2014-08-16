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
using System.Windows.Interop;
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
            Height = Screen.PrimaryScreen.Bounds.Height;
            Width = Screen.PrimaryScreen.Bounds.Width;
            vGamePadBase.Width = Width;
        }

        public void DefaultDown(object sender, vGamePadEventArgs e)
        {
            // 反転
            ChangeButtonStatus(e.ui, Colors.White, Colors.Black);
            dic.vButtonDic[e.ui.Uid].Id = e.id;

            // 移動可能なコントロール
            if (dic.vButtonDic[e.ui.Uid].Moving)
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
            if (dic.vButtonDic[e.ui.Uid].Moving)
            {
                if (dic.vButtonDic[e.ui.Uid].Top != double.MaxValue)
                {
                    e.ui.ClearValue(Canvas.BottomProperty);
                    e.ui.SetValue(Canvas.TopProperty, dic.vButtonDic[e.ui.Uid].Top);
                }
                if (dic.vButtonDic[e.ui.Uid].Left != double.MaxValue)
                {
                    e.ui.ClearValue(Canvas.RightProperty);
                    e.ui.SetValue(Canvas.LeftProperty, dic.vButtonDic[e.ui.Uid].Left);
                }
                if (dic.vButtonDic[e.ui.Uid].Bottom != double.MaxValue)
                {
                    e.ui.ClearValue(Canvas.TopProperty);
                    e.ui.SetValue(Canvas.BottomProperty, dic.vButtonDic[e.ui.Uid].Bottom);
                }
                if (dic.vButtonDic[e.ui.Uid].Right != double.MaxValue)
                {
                    e.ui.ClearValue(Canvas.LeftProperty);
                    e.ui.SetValue(Canvas.RightProperty, dic.vButtonDic[e.ui.Uid].Right);
                }
            }
        }

        public void DefaultMove(object sender, vGamePadEventArgs e)
        {
            Point pos = dic.vButtonDic[e.ui.Uid].vGetPosition(e.point);
            double width = (double)e.ui.GetValue(WidthProperty);
            double height = (double)e.ui.GetValue(HeightProperty);
            e.ui.ClearValue(Canvas.RightProperty);
            e.ui.ClearValue(Canvas.BottomProperty);
            e.ui.SetValue(Canvas.LeftProperty, pos.X - width / 2);
            e.ui.SetValue(Canvas.TopProperty, pos.Y - height / 2);
        }
        
        public void AnalogStickDown(object sender, vGamePadEventArgs e)
        {
            devCon.MoveStick(dic.vButtonDic[e.ui.Uid].Index, dic.vButtonDic[e.ui.Uid].vGetAxisX(e.point.X), dic.vButtonDic[e.ui.Uid].vGetAxisY(e.point.Y));
            DefaultDown(sender, e);
        }

        public void AnalogStickUp(object sender, vGamePadEventArgs e)
        {
            devCon.MoveStick(dic.vButtonDic[e.ui.Uid].Index, dic.vButtonDic[e.ui.Uid].vGetAxisX(double.MaxValue), dic.vButtonDic[e.ui.Uid].vGetAxisY(double.MaxValue));
            DefaultUp(sender, e);
        }

        public void AnalogStickMove(object sender, vGamePadEventArgs e)
        {
            Point pos = dic.vButtonDic[e.ui.Uid].vGetPosition(e.point);
            devCon.MoveStick(dic.vButtonDic[e.ui.Uid].Index, dic.vButtonDic[e.ui.Uid].vGetAxisX(pos.X), dic.vButtonDic[e.ui.Uid].vGetAxisY(pos.Y));
            DefaultMove(sender, e);
        }

        public void CrossDown(object sender, vGamePadEventArgs e)
        {
            devCon.PushCross(dic.vButtonDic[e.ui.Uid].Index);
            DefaultDown(sender, e);
        }

        public void CrossUp(object sender, vGamePadEventArgs e)
        {
            devCon.FreeCross(dic.vButtonDic[e.ui.Uid].Index);
            DefaultUp(sender, e);
        }

        public void ButtonDown(object sender, vGamePadEventArgs e)
        {
            devCon.PushButton(dic.vButtonDic[e.ui.Uid].Index);
            DefaultDown(sender, e);
        }

        public void ButtonUp(object sender, vGamePadEventArgs e)
        {
            devCon.FreeButton(dic.vButtonDic[e.ui.Uid].Index);
            DefaultUp(sender, e);
        }

        public void BarrageDown(object sender, vGamePadEventArgs e)
        {
            // デフォルト動作
            DefaultDown(sender, e);

            // vJoyボタンダウン
            devCon.PushButton(dic.vButtonDic[e.ui.Uid].Index);

            // タイマーオブジェクトの有無を確認
            if (!vButtonTimerDic.ContainsKey(e.ui.Uid))
            {
                vButtonTimerDic.Add(e.ui.Uid, new BarrageTimer(e.ui));
            }
            // 対象のボタンのタイマー状態で動作を切り分け
            switch (dic.vButtonDic[e.ui.Uid].Barrage)
            {
                case vButton.BarrageState.TimerOn:
                    vButtonTimerDic[e.ui.Uid].Tick -= new EventHandler(BarrageTimerEvent);
                    dic.vButtonDic[e.ui.Uid].Barrage = vButton.BarrageState.TimerOff;
                    vButtonTimerDic[e.ui.Uid].Stop();
                    break;
                case vButton.BarrageState.None:
                case vButton.BarrageState.TimerOff:
                    // 2秒間のタイマーセット
                    vButtonTimerDic[e.ui.Uid].Interval = new TimeSpan(0, 0, 2);
                    vButtonTimerDic[e.ui.Uid].Tick += new EventHandler(BarrageTimerEvent);
                    dic.vButtonDic[e.ui.Uid].Barrage = vButton.BarrageState.WaitTimeout;
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
            devCon.FreeButton(dic.vButtonDic[e.ui.Uid].Index);

            // タイマーオブジェクトの有無を確認(※そもそもBarrageDownが先にくるが...)
            if (!vButtonTimerDic.ContainsKey(e.ui.Uid))
            {
                vButtonTimerDic.Add(e.ui.Uid, new BarrageTimer(e.ui));
            }
            // 対象のボタンタイマー状態で動作を切り分け
            switch (dic.vButtonDic[e.ui.Uid].Barrage)
            {
                case vButton.BarrageState.None:
                case vButton.BarrageState.TimerOff:
                    ChangeButtonStatus(e.ui, Colors.Black, Colors.White);
                    break;
                case vButton.BarrageState.WaitTimeout:
                    vButtonTimerDic[e.ui.Uid].Tick -= new EventHandler(BarrageTimerEvent);
                    dic.vButtonDic[e.ui.Uid].Barrage = vButton.BarrageState.TimerOff;
                    vButtonTimerDic[e.ui.Uid].Stop();
                    ChangeButtonStatus(e.ui, Colors.Black, Colors.White);
                    break;
            }
        }

        private void BarrageTimerEvent(object sender, EventArgs e)
        {
            Debug.WriteLine("BarrageTimerEvent:{0}",((BarrageTimer)sender).Ui.Uid);

            if (dic.vButtonDic[((BarrageTimer)sender).Ui.Uid].Barrage == vButton.BarrageState.WaitTimeout)
            {
                // タイマーステータス TimerOn
                dic.vButtonDic[((BarrageTimer)sender).Ui.Uid].Barrage = vButton.BarrageState.TimerOn;
                // インターバル 0.1秒
                ((BarrageTimer)sender).Interval = new TimeSpan(0, 0, 0, 0, 100);
                // 画面ボタン赤に
                ChangeButtonStatus(((BarrageTimer)sender).Ui, Colors.Red, Colors.White);
                // 音を鳴らす
            }
            // vJoyボタンダウン
            devCon.PushButton(dic.vButtonDic[((BarrageTimer)sender).Ui.Uid].Index);
            // vJoyボタンアップ
            devCon.FreeButton(dic.vButtonDic[((BarrageTimer)sender).Ui.Uid].Index);
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
            Close();
        }

        public void HomeDown(object sender, vGamePadEventArgs e)
        {
            // 反転
            ChangeButtonStatus(e.ui, Colors.White, Colors.Black);
            dic.vButtonDic[e.ui.Uid].Id = e.id;
            dic.vButtonDic[e.ui.Uid].Range = Height;
            vGamePadBase.Background = new SolidColorBrush(Colors.White);
            // 移動可能なコントロール
            if (dic.vButtonDic[e.ui.Uid].Moving)
            {
                // この座標を中心点にする
                // 中心点の計算
                double height = (double)e.ui.GetValue(HeightProperty);
                e.ui.SetValue(Canvas.TopProperty, e.point.Y - height / 2);
            }
        }

        public void HomeMove(object sender, vGamePadEventArgs e)
        {
            Point pos = dic.vButtonDic[e.ui.Uid].vGetPosition(e.point);
            double height = (double)e.ui.GetValue(HeightProperty);
            if (pos.Y < height / 2 + 8)
                pos.Y = height / 2 + 8;
            else if (pos.Y > (Height - vGamePadBase.Height + height / 2 + 8))
                pos.Y = Height - vGamePadBase.Height + height / 2 + 8;
            e.ui.ClearValue(Canvas.BottomProperty);
            e.ui.SetValue(Canvas.TopProperty, pos.Y - height / 2);
            vGamePadBase.SetValue(Canvas.TopProperty, pos.Y - height / 2 - 8);
        }

        public void HomeUp(object sender, vGamePadEventArgs e)
        {
            this.vGamePadBase.Background = new SolidColorBrush(Colors.Black);
            ChangeButtonStatus(e.ui, Colors.Black, Colors.White);
            if (dic.vButtonDic[e.ui.Uid].Moving)
            {
                Point pos = dic.vButtonDic[e.ui.Uid].vGetPosition(e.point);
                double height = (double)e.ui.GetValue(HeightProperty);
                if (pos.Y < height / 2 + 8)
                    pos.Y = height / 2 + 8;
                else if (pos.Y > (Height - vGamePadBase.Height + height / 2 + 8))
                    pos.Y = Height - vGamePadBase.Height + height / 2 + 8;

                // 自分の座標以外を全部移動させる
                double range = pos.Y - height / 2 - dic.vButtonDic[e.ui.Uid].Top;

                foreach (UIElement ui in vGamePadCanvas.Children)
                {
                    foreach (string key in dic.vButtonDic.Keys)
                    {
                        if (ui.Uid == "Home")
                            continue;
                        if (ui.Uid == key)
                        {
                            if (dic.vButtonDic[key].Top != double.MaxValue)
                            {
                                dic.vButtonDic[key].Top = dic.vButtonDic[key].Top + range;
                                ui.ClearValue(Canvas.BottomProperty);
                                ui.SetValue(Canvas.TopProperty, dic.vButtonDic[key].Top);
                            }
                            if (dic.vButtonDic[key].Bottom != double.MaxValue)
                            {
                                dic.vButtonDic[key].Bottom = dic.vButtonDic[key].Bottom + range;
                                ui.ClearValue(Canvas.TopProperty);
                                ui.SetValue(Canvas.BottomProperty, dic.vButtonDic[key].Bottom);
                            }
                        }
                    }
                }
                vGamePadBase.SetValue(Canvas.TopProperty, pos.Y - height / 2 - 8);
                dic.vButtonDic[e.ui.Uid].Top = pos.Y - height / 2;
            }
        }
    }
}