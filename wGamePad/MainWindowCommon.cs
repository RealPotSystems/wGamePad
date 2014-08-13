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
    public partial class MainWindow : Window
    {
        private const int WM_MOUSEMOVE = 0x0200;
        private const int WM_LBUTTONDOWN = 0x0201;
        private const int WM_LBUTTONUP = 0x0202;

        private const int WM_POINTERUPDATE = 0x0245;
        private const int WM_POINTERDOWN = 0x0246;
        private const int WM_POINTERUP = 0x0247;

        private const int GWL_EXSTYLE = -20;
        private const int WS_EX_NOACTIVATE = 0x8000000;

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        static extern IntPtr SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        private static int GET_X_LPARAM(IntPtr lParam)
        {
            return (int)lParam & 0xFFFF;
        }
        private static int GET_Y_LPARAM(IntPtr lParam)
        {
            return (int)lParam >> 16;
        }
        private static uint GET_POINTERID_WPARAM(IntPtr wParam)
        {
            return (uint)wParam & 0xFFFF;
        }
    }
}