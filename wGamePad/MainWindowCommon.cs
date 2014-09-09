﻿using System;
using System.Windows;
using System.Windows.Interop;
using System.Media;
using System.Windows.Media;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace vGamePad
{
    public static class ViewExtensions
    {
        public static Point GetDpiScaleFactor(this Visual visual)
        {
            var source = PresentationSource.FromVisual(visual);
            if (source != null && source.CompositionTarget != null)
            {
                return new Point(
                    source.CompositionTarget.TransformToDevice.M11,
                    source.CompositionTarget.TransformToDevice.M22);
            }

            return new Point(1.0, 1.0);
        }
    }

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
        public const int GWL_EXSTYLE = -20;
        public const int WS_EX_NOACTIVATE = 0x8000000;

        [System.Runtime.InteropServices.DllImport("user32.dll", SetLastError = true)]
        internal static extern uint SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);
        [System.Runtime.InteropServices.DllImport("user32.dll", SetLastError = true)]
        internal static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        public const int SWP_NOSIZE = 0x0001;
        public const int SWP_NOMOVE = 0x0002;
        public const int SWP_SHOWWINDOW = 0x0040;

        public const int HWND_TOP = 0;
        public const int HWND_TOPMOST = -1;
        public const int HWND_NOTOPMOST = -2;
        [System.Runtime.InteropServices.DllImport("user32.dll", SetLastError = true)]
        [return: System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.Bool)]
        internal static extern bool SetWindowPos(IntPtr hWnd, int hWndInsertAfter, int x, int y, int cx, int cy, int uFlags);
        [System.Runtime.InteropServices.DllImport("user32.dll", SetLastError = true)]
        internal static extern uint keybd_event(byte bVk, byte bScan, uint dwFlags, UIntPtr dwExtraInfo);

        public const byte VK_LWIN = 0x5B;
    }

    public partial class MainWindow : Window
    {
        const int WM_SYSKEYDOWN = 0x0104;
        const int VK_F4 = 0x73;

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            WindowInteropHelper helper = new WindowInteropHelper(this);
            NativeMethods.SetWindowLong(helper.Handle, NativeMethods.GWL_EXSTYLE, NativeMethods.GetWindowLong(helper.Handle, NativeMethods.GWL_EXSTYLE) | NativeMethods.WS_EX_NOACTIVATE);
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