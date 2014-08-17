using System;
using System.Windows;
using System.Windows.Interop;
using System.Media;

namespace vGamePad
{
    public static class PlayButtonSound
    {
        private static SoundPlayer player = new SoundPlayer(Properties.Resources.Sound01);

        public enum PlayType
        {
            Normal,
            Sync,
            Loop,
        }

        public static void Play(PlayType p = PlayType.Normal)
        {
            if (Properties.Settings.Default.Sound)
            {
                switch (p)
                {
                    case PlayType.Normal:
                        player.Play();
                        break;
                    case PlayType.Sync:
                        player.PlaySync();
                        break;
                    case PlayType.Loop:
                        // ループは止める方法が無いのでいったん未実装
                        break;
                }
            }
        }
    }

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