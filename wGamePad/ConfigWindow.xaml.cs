using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Forms;
using System.Diagnostics;
using System.Reflection;
using Microsoft.Win32;

namespace vGamePad
{
    /// <summary>
    /// ConfigWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class ConfigWindow : Window
    {
        private const string versionFormat = "{0} version {1}\n{2} {3}.\n";
        private const string sound_on = "\uE15D";
        private const string sound_off = "\uE198";
        private const string ScreenRotate_on = "\uE244";
        private const string ScreenRotate_off = "\uE245";
        private const string check_on = "\uE0A2";
        private const string check_off = "\uE003";

        public ConfigWindow()
        {
            InitializeComponent();

            FileVersionInfo ver =
                FileVersionInfo.GetVersionInfo(
                Assembly.GetExecutingAssembly().Location);
            About.Text = string.Format(versionFormat, ver.ProductName, ver.ProductVersion, ver.LegalCopyright, ver.CompanyName);
            About.Text += MainWindow.devCon.ToString();

            ScreenRotate.IsEnabled = SensorPresent();
            ScreenRotate.Content = AutoRotation() ? ScreenRotate_on : ScreenRotate_off;
            Skeleton.Content = check_off;

        }

        private void Exit_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            PlayButtonSound.Play();
            Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (Properties.Settings.Default.Sound)
            {
                Sound.Content = sound_on;
            }
            else
            {
                Sound.Content = sound_off;
            }
        }

        private void Sound_Click(object sender, RoutedEventArgs e)
        {
            if (Properties.Settings.Default.Sound)
            {
                Properties.Settings.Default.Sound = false;
                Sound.Content = sound_off;
            }
            else
            {
                Properties.Settings.Default.Sound = true;
                Sound.Content = sound_on;
            }
            PlayButtonSound.Play();
        }

        private void ScreenRotate_Click(object sender, RoutedEventArgs e)
        {
            PlayButtonSound.Play();
            SendKeys.Send("^{ESC}o");
            ScreenRotate.Content = AutoRotation() ? ScreenRotate_on : ScreenRotate_off;
        }

        private bool SensorPresent()
        {
            try
            {
                RegistryKey rKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\AutoRotation");
                return (bool)rKey.GetValue("SensorPresent");
            }
            catch(NullReferenceException)
            {
                return false;
            }
        }

        private bool AutoRotation()
        {
            try
            {
                RegistryKey rKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\AutoRotation");
                return (bool)rKey.GetValue("AutoRotation");
            }
            catch (NullReferenceException)
            {
                return false;
            }

        }

        private void SetPostion_Click(object sender, RoutedEventArgs e)
        {
            PlayButtonSound.Play(); 
            try
            {
                Process[] ps = Process.GetProcessesByName("DQXGame");   // ドラクエ10のプロセス名を指定する
                if (ps.Length == 1)
                {
                    ps[0].WaitForInputIdle();
                    NativeMethods.SetWindowPos(
                        ps[0].MainWindowHandle,
                        NativeMethods.HWND_TOP,
                        System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Left,
                        System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Top,
                        0,
                        0,
                        NativeMethods.SWP_NOSIZE | NativeMethods.SWP_SHOWWINDOW);
                }
            }
            catch
            {
            }
        }
    }
}
