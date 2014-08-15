using System;
using System.Windows;
using System.Windows.Interop;

namespace vGamePad
{
    public abstract class NativeMethods
    {
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        internal static extern uint SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        internal static extern int GetWindowLong(IntPtr hWnd, int nIndex);
    }

    public partial class MainWindow : Window
    {
        const int WM_SYSKEYDOWN = 0x0104;
        const int VK_F4 = 0x73;

        private const int GWL_EXSTYLE = -20;
        private const int WS_EX_NOACTIVATE = 0x8000000;

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            WindowInteropHelper helper = new WindowInteropHelper(this);
            NativeMethods.SetWindowLong(helper.Handle, GWL_EXSTYLE, NativeMethods.GetWindowLong(helper.Handle, GWL_EXSTYLE) | WS_EX_NOACTIVATE);
            HwndSource souce = HwndSource.FromHwnd(helper.Handle);
            souce.AddHook(WndProc);
        }

        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if ((msg == WM_SYSKEYDOWN) && (wParam.ToInt32() == VK_F4))
            {
                handled = true;
            }
            return IntPtr.Zero;
        }
    }
}