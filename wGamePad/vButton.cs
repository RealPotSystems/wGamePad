using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace wGamePad
{
    public class vButton
    {
        public enum ExecType
        {
            Down,
            Up,
            Move
        };
        // イベントハンドラを実装する
        public event EventHandler DownAction;
        public event EventHandler UpAction;
        public event EventHandler MoveAction;
        protected virtual void OnDownAction(EventArgs e)
        {
            if (DownAction != null)
            {
                DownAction(this, e);
            }
        }
        protected virtual void OnUpAction(EventArgs e)
        {
            if (UpAction != null)
            {
                UpAction(this, e);
            }
        }
        protected virtual void OnMoveAction(EventArgs e)
        {
            if (MoveAction != null)
            {
                MoveAction(this, e);
            }
        }

        public void ExecEvent(ExecType type)
        {
            switch (type)
            {
                case ExecType.Down:
                    OnDownAction(new EventArgs());
                    break;
                case ExecType.Up:
                    OnUpAction(new EventArgs());
                    break;
                case ExecType.Move:
                    OnMoveAction(new EventArgs());
                    break;
            }
        }

        //
        public uint Id { set; get; }

        // 座標情報
        public double Top { set; get; }
        public double Bottom { set; get; }
        public double Left { set; get; }
        public double Right { set; get; }

        // 表示情報
        public Visibility Visible { set; get; }

        //
        public double Width { set; get; }
        public double Height { set; get; }

        // 移動可能
        public bool Moving { set; get; }
        // 移動可能距離(中心点からの高さ、幅)
        public double Range { set; get; }

        private Window win = Application.Current.MainWindow;

        public vButton()
        {
            Id = uint.MaxValue;

            Top = double.MaxValue;
            Bottom = double.MaxValue;
            Left = double.MaxValue;
            Right = double.MaxValue;

            Visible = Visibility.Collapsed;

            Width = double.MaxValue;
            Height = double.MaxValue;

            Moving = false; // 移動不可
            Range = 50.0;
        }

        public bool hitTest(Point now)
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
                x = win.Width - Right - Width / 2;

            if (Top != double.MaxValue)
                y = Top + Height / 2;
            else
                y = win.Height - Bottom - Height / 2;

            if ((x - now.X) * (x - now.X) + (y - now.Y) * (y - now.Y) <= (radian * radian))
            {
                return true;

            }
            return false;
        }

        public Point GetPosition(Point pos)
        {
            // ボタンの中心点を求める
            double x;
            double y;
            // 中心座標を計算する
            if (Left != double.MaxValue)
                x = Left + Width / 2;
            else
                x = win.Width - Right - Width / 2;

            if (Top != double.MaxValue)
                y = Top + Height / 2;
            else
                y = win.Height - Bottom - Height / 2;

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
    }

    public class vButtonDictionay
    {
        public Dictionary<string, vButton> vButtonDic = new Dictionary<string, vButton>();

        public vButtonDictionay()
        {
            vButtonDic.Add("AnalogStick0", new vButton() { Top = 150.0, Left = 300.0, Visible = System.Windows.Visibility.Visible, Moving = true });
            vButtonDic.Add("AnalogStick1", new vButton() { Top = 150.0, Right = 300.0, Visible = System.Windows.Visibility.Visible, Moving = true });
            vButtonDic.Add("Button01", new vButton() { Top = 120.0, Right = 100.0, Visible = System.Windows.Visibility.Visible });
            vButtonDic.Add("Button02", new vButton() { Top = 170.0, Right = 50.0, Visible = System.Windows.Visibility.Visible });
            vButtonDic.Add("Button03", new vButton() { Top = 220.0, Right = 100.0, Visible = System.Windows.Visibility.Visible });
            vButtonDic.Add("Button04", new vButton() { Top = 170.0, Right = 150.0, Visible = System.Windows.Visibility.Visible });
            vButtonDic.Add("Button05", new vButton() { Top = 50.0, Right = 150.0, Visible = System.Windows.Visibility.Visible });
            vButtonDic.Add("Button06", new vButton() { Top = 50.0, Right = 80.0, Visible = System.Windows.Visibility.Visible });
            vButtonDic.Add("Button07", new vButton() { Top = 50.0, Left = 220.0, Visible = System.Windows.Visibility.Visible });
            vButtonDic.Add("Button08", new vButton() { Top = 50.0, Right = 220.0, Visible = System.Windows.Visibility.Visible });
            vButtonDic.Add("Button09", new vButton() { });
            vButtonDic.Add("Button10", new vButton() { Top = 240.0, Left = 210.0, Visible = System.Windows.Visibility.Visible });
            vButtonDic.Add("Button11", new vButton() { });
            vButtonDic.Add("Button12", new vButton() { Top = 240.0, Right = 210.0, Visible = System.Windows.Visibility.Visible });
            vButtonDic.Add("Button_UP", new vButton() { Top = 100.0, Left = 100.0, Visible = System.Windows.Visibility.Visible });
            vButtonDic.Add("Button_DOWN", new vButton() { Top = 200.0, Left = 100.0, Visible = System.Windows.Visibility.Visible });
            vButtonDic.Add("Button_LEFT", new vButton() { Top = 150.0, Left = 50.0, Visible = System.Windows.Visibility.Visible });
            vButtonDic.Add("Button_RIGHT", new vButton() { Top = 150.0, Left = 150.0, Visible = System.Windows.Visibility.Visible });
            vButtonDic.Add("Keyboard", new vButton() { Bottom = 0.0 , Left = 300.0, Visible = System.Windows.Visibility.Visible });
            vButtonDic.Add("Crop", new vButton() { Bottom = 0.0, Right = 300.0, Visible = System.Windows.Visibility.Visible });
            vButtonDic.Add("Config", new vButton() { Top = 0.0, Right = 40.0, Visible = System.Windows.Visibility.Visible });
            vButtonDic.Add("Exit", new vButton() { Top = 0.0, Right = 0.0, Visible = System.Windows.Visibility.Visible });
        }
    }
}
