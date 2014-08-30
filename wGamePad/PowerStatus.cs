using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Forms;
using System.Windows.Threading;
using Microsoft.Win32;

namespace vGamePad
{
    class PowerStatus
    {
        public class PlayTime
        {
            /// <summary>
            /// プレイ時間を表す文字列
            /// </summary>
            public string _PlayTimeString;

            /// <summary>
            /// 基準値となるバッテリー残量
            /// </summary>
            private int _StartBatteryLife;

            /// <summary>
            /// 基準値となる時刻
            /// </summary>
            private DateTime _StartTime;

            /// <summary>
            /// 計算するタイミングを管理する
            /// </summary>
            private int _CtrlBreakBatteryLife;

            /// <summary>
            /// プレイ基準時刻
            /// </summary>
            private DateTime _PlayTime;

            /// <summary>
            /// プレイ可能秒数
            /// </summary>
            private int _PlaySecond;

            /// <summary>
            /// コンストラクタ
            /// </summary>
            public PlayTime()
            {
                Initialize();
            }

            public void Initialize()
            {
                _PlayTimeString = "";
                _StartBatteryLife = -1;
                _StartTime = DateTime.MinValue;
                _CtrlBreakBatteryLife = -1;
                _PlaySecond = -1;
            }

            public void CalcPlayTime(double BatteryLifePercent)
            {
                // バッテリー残量が10%未満の場合、無条件で計算せず返却
                if (BatteryLifePercent <= 0.100f)
                {
                    _PlayTimeString = Properties.Resources.PowerStatusString01; // "充電してください";
                    return;
                }

                // バッテリーの残量を扱いやすいように1000倍しint型へ
                int n = (int)(BatteryLifePercent * 1000);

                // 初めてこのメソッドがコールされた時
                if (_StartBatteryLife == -1)
                {
                    // 計測を開始する基準点を設定する
                    _StartBatteryLife = n - 1;

                    _PlayTimeString = Properties.Resources.PowerStatusString02; // "初期化中...";
                    return;
                }

                // 最初に基準点になった時
                if (_StartBatteryLife >= n && _CtrlBreakBatteryLife == -1)
                {
                    // 開始時刻を設定する
                    _StartTime = DateTime.Now;

                    // 次のチェック時間を設定する
                    _CtrlBreakBatteryLife = n - 1;

                    _PlayTimeString = Properties.Resources.PowerStatusString03; // "計算中...";
                    return;
                }

                // 二回目以降の再計算
                if (_CtrlBreakBatteryLife >= n)
                {
                    // 開始時刻との差を取得する
                    _PlayTime = DateTime.Now;
                    TimeSpan ts = _PlayTime - _StartTime;

                    // 次のチェック時間を設定する
                    _CtrlBreakBatteryLife = n - 1;

                    // 0.1%減る秒数を求める
                    int substract = _StartBatteryLife - n;    // これが母数
                    double seconds = ts.TotalSeconds / substract;

                    // バッテリーの残量からプレイ可能時間(秒)を作成する
                    seconds = (n - 0.060f) * seconds;
                    _PlaySecond = (int)seconds;

                    ts = new TimeSpan(0, 0, _PlaySecond);
                    _PlayTimeString = string.Format(Properties.Resources.PowerStatusString04 /* "残りプレイ時間 {0:00}時間{1:00}分" */ , ts.Hours, ts.Minutes);
                }
            }
        }
        private PlayTime playTime = new PlayTime();
        private DispatcherTimer timer = null;
        public ObjectDataProvider provider { get; set; }
        public bool battery = true;
        public PowerStatus()
        {
            if (!GetSystemBatteryStatus())
            {
                battery = false;
            }
            else
            {
                timer = new DispatcherTimer();
                timer.Tick += timer_Tick;
                timer.Interval = new TimeSpan(0, 0, 1);
                timer.Start();
                SystemEvents.PowerModeChanged += SystemEvents_PowerModeChanged;
            }
        }

        private void SystemEvents_PowerModeChanged(object sender, PowerModeChangedEventArgs e)
        {
            switch (e.Mode)
            {
                case Microsoft.Win32.PowerModes.StatusChange:
                    if (SystemInformation.PowerStatus.PowerLineStatus == PowerLineStatus.Online)
                    {
                        playTime.Initialize();
                    }
                    break;
            }
            if (provider != null)
                provider.Refresh();
        }

        void timer_Tick(object sender, EventArgs e)
        {
            if (provider != null)
                provider.Refresh();
        }

        public string CurrentPowerStatus()
        {
            if ( battery == false )
            {
                return Properties.Resources.PowerStatusString05; // "バッテリーがありません";
            }

            string percent;
            float f = SystemInformation.PowerStatus.BatteryLifePercent;
            if (f > 1)
            {
                percent = Properties.Resources.PowerStatusString06; // "???";
            }
            else
            {
                percent = String.Format(Properties.Resources.PowerStatusString07 /* "{0,3}" */, f * 100);
            }

            string mode;
            string status;
            if ((SystemInformation.PowerStatus.BatteryChargeStatus & BatteryChargeStatus.Charging) == BatteryChargeStatus.Charging)
            {
                mode = "🔌";
                status = Properties.Resources.PowerStatusString08; // "充電中";
            }
            else if (SystemInformation.PowerStatus.PowerLineStatus == PowerLineStatus.Online && f == 1.00)
            {
                mode = "🔌";
                status = Properties.Resources.PowerStatusString09; // "AC電源";
            }
            else
            {
                mode = "🔋";
                playTime.CalcPlayTime(SystemInformation.PowerStatus.BatteryLifePercent);
                status = playTime._PlayTimeString;
            }

            return String.Format(Properties.Resources.PowerStatusString10 /* "{0} {1}%:{2}" */ ,mode, percent, status);
        }

        public static bool GetSystemBatteryStatus()
        {
            if ((SystemInformation.PowerStatus.BatteryChargeStatus & BatteryChargeStatus.NoSystemBattery) == BatteryChargeStatus.NoSystemBattery)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}
