using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Forms;
using Microsoft.Win32;

namespace vGamePad
{
    class PowerStatus
    {
        public ObjectDataProvider provider { get; set; }
        private bool battery = true;
        public PowerStatus()
        {
            if ((SystemInformation.PowerStatus.BatteryChargeStatus & BatteryChargeStatus.NoSystemBattery)== BatteryChargeStatus.NoSystemBattery)
            {
                battery = false;
            }
            else
            {
                SystemEvents.PowerModeChanged += SystemEvents_PowerModeChanged;
            }
        }

        private void SystemEvents_PowerModeChanged(object sender, PowerModeChangedEventArgs e)
        {
            provider.Refresh();
        }

        public string CurrentPowerStatus()
        {
            if ( battery == false )
            {
                return "バッテリーがありません";
            }
            return "充電中";
        }
    }
}
