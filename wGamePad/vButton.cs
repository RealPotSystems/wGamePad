//------------------------------------------------------------
//
//  仮想ゲームパッド vGamePad
//  © 2014,RealPotSystems(TAKUBON). All rights reserverd.
//
//  Workfile : vButton.cs
//  Author   : TAKUYA MANABE(manataku@me.com)
//
//------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Runtime.Serialization;
using System.Xml;
using System.IO;

namespace vGamePad
{
    public class vGamePadEventArgs : EventArgs
    {
        public UIElement ui { set; get; }
        public Point point { set; get; }
        public int id { set; get; }
        public vGamePadEventArgs(UIElement u, Point p, int i = -1)
        {
            ui = u;
            point = p;
            id = i;
        }
    }

    /// <summary>
    /// 仮想ゲームパッド上に表示する仮想ボタンクラス
    /// </summary>
    [DataContract]
    public class vButton
    {
        /// <summary>
        /// 仮想ボタンをタッチ時のアクション
        /// </summary>
        public enum ActionType
        {
            Down,
            Up,
            Move
        };
        /// <summary>
        /// 連射状態
        /// </summary>
        public enum BarrageState
        {
            TimerOff,
            WaitTimeout,
            TimerOn,
            None,
        }
        /// <summary>
        /// 仮想ボタン：ダウン時のイベントデリゲート
        /// </summary>
        public event EventHandler<vGamePadEventArgs> DownAction;
        /// <summary>
        /// 仮想ボタン：ダウン時のイベントハンドラ
        /// </summary>
        /// <param name="e">仮想ゲームパッドボタンイベント</param>
        protected virtual void OnDownAction(vGamePadEventArgs e)
        {
            if (DownAction != null)
            {
                DownAction(this, e);
            }
        }
        /// <summary>
        /// 仮想ボタン：アップ時のイベントデリゲート
        /// </summary>
        public event EventHandler<vGamePadEventArgs> UpAction;
        /// <summary>
        /// 仮想ボタン：アップ時のイベントハンドラ
        /// </summary>
        /// <param name="e">仮想ゲームパッドボタンイベント</param>
        protected virtual void OnUpAction(vGamePadEventArgs e)
        {
            if (UpAction != null)
            {
                UpAction(this, e);
            }
        }
        /// <summary>
        /// 仮想ボタン：移動時のイベントデリゲート
        /// </summary>
        public event EventHandler<vGamePadEventArgs> MoveAction;
        /// <summary>
        /// 仮想ボタン：移動時のイベントハンドラ
        /// </summary>
        /// <param name="e">仮想ゲームパッドボタンイベント</param>
        protected virtual void OnMoveAction(vGamePadEventArgs e)
        {
            if (MoveAction != null)
            {
                MoveAction(this, e);
            }
        }
        /// <summary>
        /// 仮想ボタン：イベントトリガー
        /// </summary>
        /// <param name="type">仮想ボタンの状態</param>
        public void ActionEvent(ActionType type, UIElement ui, Point point, int id = -1)
        {
            switch (type)
            {
                case ActionType.Down:
                    OnDownAction(new vGamePadEventArgs(ui,point,id));
                    break;
                case ActionType.Up:
                    OnUpAction(new vGamePadEventArgs(ui,point));
                    break;
                case ActionType.Move:
                    OnMoveAction(new vGamePadEventArgs(ui,point));
                    break;
            }
        }
        /// <summary>
        /// 仮想ボタンのインデックス
        /// </summary>
        /// <remarks>
        /// ゲームパッドのボタン番号
        /// 十字キーは0:上 1:右 2:下 3:左
        /// それ以外は不定
        /// </remarks>
        [DataMember]
        public uint Index { set; get; }
        /// <summary>
        /// 仮想ボタンのユニークID
        /// </summary>
        /// <remarks>
        /// タッチスクリーンにタッチした際、システムが割り当てる固有のID
        /// int.MinValueを指定することで管理対象外扱いになる
        /// </remarks>
        [DataMember]
        public int Id { set; get; }
        /// <summary>
        /// 仮想ボタンの画面上からの座標
        /// </summary>
        /// <remarks>
        /// Bottomを指定する場合、MaxValueを設定する
        /// </remarks>
        [DataMember]
        public double Top { set; get; }
        /// <summary>
        /// 仮想ボタンの画面下からの座標
        /// </summary>
        /// <remarks>
        /// Topを指定する場合、MaxValueを設定する
        /// </remarks>
        [DataMember]
        public double Bottom { set; get; }
        /// <summary>
        /// 仮想ボタンの画面左から座標
        /// </summary>
        /// <remarks>
        /// Rightを指定する場合、MaxValueを設定する
        /// </remarks>
        [DataMember]
        public double Left { set; get; }
        /// <summary>
        /// 仮想ボタンの画面左から座標
        /// </summary>
        /// <remarks>
        /// Leftを指定する場合、MaxValueを設定する
        /// </remarks>
        [DataMember]
        public double Right { set; get; }
        /// <summary>
        /// 仮想ボタンの表示状態
        /// </summary>
        [DataMember]
        public Visibility Visible { set; get; }
        /// <summary>
        /// 仮想ボタンの幅
        /// </summary>
        [DataMember]
        public double Width { set; get; }
        /// <summary>
        /// 仮想ボタンの高さ
        /// </summary>
        [DataMember]
        public double Height { set; get; }
        // 移動可能
        [DataMember]
        public bool Moving { set; get; }
        // 移動可能距離(中心点からの高さ、幅)
        [DataMember]
        public double Range { set; get; }
        /// <summary>
        /// 管理対象ボタン
        /// </summary>
        [DataMember]
        public bool Fixed { set; get; }

        // 連射機能
        [DataMember]
        public BarrageState Barrage { set; get; }

        /// <summary>
        /// vGamePadのメインウィンドウ
        /// </summary>
        private static readonly Window mainwindow = Application.Current.MainWindow;
        /// <summary>
        /// 仮想ボタンコンストラクタ
        /// </summary>
        public vButton()
        {
            // ボタン未押下状態
            Id = int.MinValue;

            // 各座標はMaxValueで統一
            Top = double.MaxValue;
            Bottom = double.MaxValue;
            Left = double.MaxValue;
            Right = double.MaxValue;

            // 初期値はCollapsedにしておく
            Visible = Visibility.Collapsed;

            // 幅、高さはMaxValueで統一
            Width = double.MaxValue;
            Height = double.MaxValue;

            // 移動不可
            Moving = false;
            Range = 32.0;

            // 連射状態
            Barrage = BarrageState.None;

            // 再配置可能
            Fixed = false;
        }

        public bool vHitTest(Point now)
        {
            // 保存されている座標情報から判断する
            // 幅から基準となるradiusを決める
            double radian = Width / 2;

            double x;
            double y;
            // 中心座標を計算する
            if (Left != double.MaxValue)
                x = Left + Width / 2;
            else
                x = mainwindow.Width - Right - Width / 2;

            if (Top != double.MaxValue)
                y = Top + Height / 2;
            else
                y = mainwindow.Height - Bottom - Height / 2;

            if ((x - now.X) * (x - now.X) + (y - now.Y) * (y - now.Y) <= (radian * radian))
            {
                return true;

            }
            return false;
        }

        public Point vGetPosition(Point pos)
        {
            // ボタンの中心点を求める
            double x;
            double y;
            // 中心座標を計算する
            if (Left != double.MaxValue)
                x = Left + Width / 2;
            else
                x = mainwindow.Width - Right - Width / 2;

            if (Top != double.MaxValue)
                y = Top + Height / 2;
            else
                y = mainwindow.Height - Bottom - Height / 2;

            if ((x - Range) >= pos.X)
            {
                pos.X = x - Range;
            }
            else if ((x + Range) <= pos.X)
            {
                pos.X = x + Range;
            }
            if ((y - Range) >= pos.Y)
            {
                pos.Y = y - Range;
            }
            else if ((y + Range) <= pos.Y)
            {
                pos.Y = y + Range;
            }
            return pos;
        }

        public long vGetAxisY(double NowY)
        {
            double y;
            // 中心座標を計算する
            if (Top != double.MaxValue)
                y = Top + Height / 2;
            else
                y = mainwindow.Height - Bottom - Height / 2;
            if (NowY == double.MaxValue)
                return 50;
            return (long)((y + Range - NowY) * 100 / (Range * 2));
        }

        public long vGetAxisX(double NowX)
        {
            double x;
            // 中心座標を計算する
            if (Left != double.MaxValue)
                x = Left + Width / 2;
            else
                x = mainwindow.Width - Right - Width / 2;
            if (NowX == double.MaxValue)
                return 50;

            return (long)((x + Range - NowX) * 100 / (Range * 2));
        }
    }

    public class vButtonDictionary : Dictionary<string,vButton>
    {
        public void SetDefaultLayout()
        {
            Clear();
            Add("AnalogStick0", new vButton() { Index = 0, Top = 352.0, Left = 80.0, Visible = Visibility.Visible, Moving = true });
            Add("AnalogStick1", new vButton() { Index = 1, Top = 352.0, Right = 80.0, Visible = Visibility.Visible, Moving = true });
            Add("Button01", new vButton() { Index = 0, Top = 128.0, Right = 96.0, Visible = Visibility.Visible });
            Add("Button02", new vButton() { Index = 1, Top = 192.0, Right = 32.0, Visible = Visibility.Visible });
            Add("Button03", new vButton() { Index = 2, Top = 256.0, Right = 96.0, Visible = Visibility.Visible });
            Add("Button04", new vButton() { Index = 3, Top = 192.0, Right = 160.0, Visible = Visibility.Visible });
            Add("Button05", new vButton() { Index = 4, Top = 320.0, Right = 240.0, Visible = Visibility.Visible });
            Add("Button06", new vButton() { Index = 5, Top = 416.0, Right = 240.0, Visible = Visibility.Visible });
            Add("Button07", new vButton() { Index = 6, Top = 320.0, Left = 240.0, Visible = Visibility.Visible });
            Add("Button08", new vButton() { Index = 7, Top = 416.0, Left = 240.0, Visible = Visibility.Visible });
            Add("Button09", new vButton() { });
            Add("Button10", new vButton() { Index = 9, Top = 512.0, Left = 176.0, Visible = Visibility.Visible });
            Add("Button11", new vButton() { });
            Add("Button12", new vButton() { Index = 11, Top = 512.0, Right = 176.0, Visible = Visibility.Visible });
            Add("Button_UP", new vButton() { Index = 0, Top = 128.0, Left = 96.0, Visible = Visibility.Visible });
            Add("Button_DOWN", new vButton() { Index = 2, Top = 256.0, Left = 96.0, Visible = Visibility.Visible });
            Add("Button_LEFT", new vButton() { Index = 3, Top = 192.0, Left = 32.0, Visible = Visibility.Visible });
            Add("Button_RIGHT", new vButton() { Index = 1, Top = 192.0, Left = 160.0, Visible = Visibility.Visible });
            Add("Keyboard", new vButton() { Top = 512.0, Left = 48.0, Visible = Visibility.Visible });
            Add("Crop", new vButton() { Top = 512, Right = 48.0, Visible = Visibility.Visible });
            Add("Config", new vButton() { Top = 0.0, Left = 128.0, Visible = Visibility.Visible, Fixed = true });
            Add("Exit", new vButton() { Top = 0.0, Right = 128.0, Visible = Visibility.Visible, Fixed = true });
            Add("Home", new vButton() { Top = 16.0, Left = 16.0, Visible = Visibility.Visible, Moving = true, Fixed = true });
        }

    }

    public class vLayoutControl
    {
        public static vButtonDictionary LoadLayout(int n)
        {
            vButtonDictionary d;
            if ( n == 0 )
            {
                // デフォルト
                d = new vButtonDictionary();
                d.SetDefaultLayout();
                return d;
            }
            var serializer = new DataContractSerializer(typeof(vButtonDictionary));
            XmlReader xml = XmlReader.Create(@"D:\Users\manabe\Documents\DataStore\Test.xml");
            d = (vButtonDictionary)serializer.ReadObject(xml);
            xml.Close();
            return d;
        }

        public static void SaveLayout(int n, vButtonDictionary d)
        {
            var serializer = new DataContractSerializer(typeof(vButtonDictionary));
            using (var ms = new MemoryStream())
            {
                serializer.WriteObject(ms, d); // シリアライズ
                var xml = Encoding.UTF8.GetString(ms.ToArray(), 0, (int)ms.Length);
                Encoding utf8 = Encoding.GetEncoding("utf-8");
                StreamWriter writer = new StreamWriter(@"D:\Users\manabe\Documents\DataStore\Test.xml", true, utf8);
                writer.WriteLine(xml);
                writer.Close();
            }
        }
    }
}
