using System;
using System.Windows;
using System.Windows.Input;
using System.Diagnostics;
using System.Reflection;
using System.Windows.Navigation;

namespace vGamePad
{
    /// <summary>
    /// ConfigWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class ConfigWindow : Window
    {
        public NavigationService navigation = null;
        private const string versionFormat = "{0} version {1}\n{2} {3}.\n";
        private const string sound_on = "\uE15D";
        private const string sound_off = "\uE198";
        private const string ScreenRotate_on = "\uE244";
        private const string ScreenRotate_off = "\uE245";
        private const string check_on = "\uE0A2";
        private const string check_off = "\uE003";
        private const string stick = "\uE10A";
        private const string regkey = "HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\AutoRotation";
        private const string sensorpresent = "SensorPresent";
        private const string enable = "Enable";
        private const string nextpage = "LayoutSelect.xaml";

        public ConfigWindow()
        {
            InitializeComponent();

            // バージョン情報の編集
            FileVersionInfo ver =
                FileVersionInfo.GetVersionInfo(
                Assembly.GetExecutingAssembly().Location);
            About.Text = string.Format(versionFormat, ver.ProductName, ver.ProductVersion, ver.LegalCopyright, ver.CompanyName);
            About.Text += MainWindow.devCon.ToString();

            // 回転機能ボタン
            ScreenRotate.IsEnabled = SensorPresent();
            ScreenRotate.Content = AutoRotation() ? ScreenRotate_on : ScreenRotate_off;
            Skeleton.Content = check_off;
                
            if ( PowerStatus.GetSystemBatteryStatus() == false ) // バッテリーなし
            {
                Properties.Settings.Default.Battery = false;
                BtyTime.IsEnabled = PowerStatus.GetSystemBatteryStatus();
            }
        }

        private void Exit_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            PlayButtonSound.Play();
            Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Sound.Content = Properties.Settings.Default.Sound ? sound_on : sound_off;

            Skeleton.Content = Properties.Settings.Default.Skeleton ?
                String.Format(Properties.Resources.ConfigButton01, check_on) :
                String.Format(Properties.Resources.ConfigButton01, check_off);

            Barrage.Content = Properties.Settings.Default.Barrage ?
                String.Format(Properties.Resources.ConfigButton04, check_on) :
                String.Format(Properties.Resources.ConfigButton04, check_off);

            ScreenMemo.Content = Properties.Settings.Default.ScreenMemo ?
                String.Format(Properties.Resources.ConfigButton02, check_on) :
                String.Format(Properties.Resources.ConfigButton02, check_off);

            BtyTime.Content = Properties.Settings.Default.Battery ?
                String.Format(Properties.Resources.ConfigButton03, check_on) :
                String.Format(Properties.Resources.ConfigButton03, check_off);

            navigation = LayoutFrame.NavigationService;

            if ( Properties.Settings.Default.Clock )
            {
                Clock1.Content = String.Format(Properties.Resources.ClockString01, check_on);
                Clock2.IsEnabled = true;
                Clock3.IsEnabled = true;
            }
            else
            {
                Clock1.Content = String.Format(Properties.Resources.ClockString01, check_off);
                Clock2.IsEnabled = false;
                Clock3.IsEnabled = false;
            }

            if ( Properties.Settings.Default.AstClock )
            {
                Clock2.Content = String.Format(Properties.Resources.ClockString02, check_on);
                Clock3.Content = String.Format(Properties.Resources.ClockString03, check_off);
            }
            else
            {
                Clock2.Content = String.Format(Properties.Resources.ClockString02, check_off);
                Clock3.Content = String.Format(Properties.Resources.ClockString03, check_on);
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
            NativeMethods.keybd_event(NativeMethods.VK_LWIN, 0, 0, (UIntPtr)0);
            byte cd = (byte)char.ConvertToUtf32("O", 0);
            NativeMethods.keybd_event(cd, 0, 0, (UIntPtr)0);
            NativeMethods.keybd_event(cd, 0, 2, (UIntPtr)0);
            NativeMethods.keybd_event(NativeMethods.VK_LWIN, 0, 2, (UIntPtr)0);
            ScreenRotate.Content = AutoRotation() ? ScreenRotate_on : ScreenRotate_off;
        }

        private bool SensorPresent()
        {
            try
            {
                int ret = (int)Microsoft.Win32.Registry.GetValue(regkey, sensorpresent, 0);
                return ret == 1 ? true : false;
            }
            catch
            {
                return false;
            }
        }

        private bool AutoRotation()
        {
            try
            {
                int ret = (int)Microsoft.Win32.Registry.GetValue(regkey, enable, 0);
                return ret == 1 ? true : false;
            }
            catch
            {
                return false;
            }
        }

        private void SetPostion_Click(object sender, RoutedEventArgs e)
        {
            PlayButtonSound.Play(); 
            try
            {
                Process[] ps = Process.GetProcessesByName(Properties.Resources.ProcessName);   // ドラクエ10のプロセス名を指定する
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

        private void Maintenance_Click(object sender, RoutedEventArgs e)
        {
            PlayButtonSound.Play();
            if (LayoutFrame.Visibility == System.Windows.Visibility.Visible)
            {
                NormalFrame.Visibility = System.Windows.Visibility.Visible;
                LayoutFrame.Visibility = System.Windows.Visibility.Collapsed;

                NormalMenu.Visibility = System.Windows.Visibility.Visible;
                LayoutMenu.Visibility = System.Windows.Visibility.Hidden;
                ClockMenu.Visibility = System.Windows.Visibility.Hidden;
            }
            else
            {
                LayoutFrame.Visibility = System.Windows.Visibility.Visible;
                NormalFrame.Visibility = System.Windows.Visibility.Collapsed;

                NormalMenu.Visibility = System.Windows.Visibility.Hidden;
                LayoutMenu.Visibility = System.Windows.Visibility.Visible;
                ClockMenu.Visibility = System.Windows.Visibility.Hidden;
                if (navigation.Content == null)
                {
                    navigation.Navigate(new Uri(nextpage, UriKind.Relative));
                }
            }
        }

        private void Skeleton_Click(object sender, RoutedEventArgs e)
        {
           PlayButtonSound.Play();
           Properties.Settings.Default.Skeleton = Properties.Settings.Default.Skeleton ? false : true;
           Skeleton.Content = Properties.Settings.Default.Skeleton ?
               String.Format(Properties.Resources.ConfigButton01, check_on) :
               String.Format(Properties.Resources.ConfigButton01, check_off);
        }

        //private void AstTime_Click(object sender, RoutedEventArgs e)
        //{
        //    PlayButtonSound.Play();
        //    Properties.Settings.Default.Clock = Properties.Settings.Default.Clock ? false : true;
        //    AstTime.Content = Properties.Settings.Default.Clock ?
        //        String.Format(Properties.Resources.ConfigButton02, check_on) :
        //        String.Format(Properties.Resources.ConfigButton02, check_off);
        //}

        private void BtyTime_Click(object sender, RoutedEventArgs e)
        {
            PlayButtonSound.Play();
            Properties.Settings.Default.Battery = Properties.Settings.Default.Battery ? false : true;
            BtyTime.Content = Properties.Settings.Default.Battery ?
                String.Format(Properties.Resources.ConfigButton03, check_on) :
                String.Format(Properties.Resources.ConfigButton03, check_off);
        }

        private void LayoutFrame_Navigated(object sender, NavigationEventArgs e)
        {
            if (navigation.CanGoBack)
            {
                GoBack.IsEnabled = true;
            }
            else
            {
                GoBack.IsEnabled = false;
            }
            if (navigation.CanGoForward)
            {
                GoForward.IsEnabled = true;
            }
            else
            {
                GoForward.IsEnabled = false;
            }
        }

        private void GoBack_Click(object sender, RoutedEventArgs e)
        {
            PlayButtonSound.Play();
            if (navigation.CanGoBack)
                navigation.GoBack();
        }

        private void GoForward_Click(object sender, RoutedEventArgs e)
        {
            PlayButtonSound.Play(); 
            if (navigation.CanGoForward)
                navigation.GoForward();
        }

        private void Barrage_Click(object sender, RoutedEventArgs e)
        {
            PlayButtonSound.Play();
            Properties.Settings.Default.Barrage = Properties.Settings.Default.Barrage ? false : true;
            Barrage.Content = Properties.Settings.Default.Barrage ?
                String.Format(Properties.Resources.ConfigButton04, check_on) :
                String.Format(Properties.Resources.ConfigButton04, check_off);
        }

        private void ClockSetting_Click(object sender, RoutedEventArgs e)
        {
            PlayButtonSound.Play();
            if (ClockFrame.Visibility == System.Windows.Visibility.Visible)
            {
                NormalFrame.Visibility = System.Windows.Visibility.Visible;
                LayoutFrame.Visibility = System.Windows.Visibility.Collapsed;
                ClockFrame.Visibility = System.Windows.Visibility.Collapsed;

                NormalMenu.Visibility = System.Windows.Visibility.Visible;
                LayoutMenu.Visibility = System.Windows.Visibility.Collapsed;
                ClockMenu.Visibility = System.Windows.Visibility.Hidden;
            }
            else
            {
                LayoutFrame.Visibility = System.Windows.Visibility.Collapsed;
                NormalFrame.Visibility = System.Windows.Visibility.Collapsed;
                ClockFrame.Visibility = System.Windows.Visibility.Visible;

                NormalMenu.Visibility = System.Windows.Visibility.Hidden;
                LayoutMenu.Visibility = System.Windows.Visibility.Hidden;
                ClockMenu.Visibility = System.Windows.Visibility.Visible;
            }

        }

        private void ScreenMemo_Click(object sender, RoutedEventArgs e)
        {
            PlayButtonSound.Play();
            Properties.Settings.Default.ScreenMemo = Properties.Settings.Default.ScreenMemo ? false : true;
            ScreenMemo.Content = Properties.Settings.Default.ScreenMemo ?
                String.Format(Properties.Resources.ConfigButton02, check_on) :
                String.Format(Properties.Resources.ConfigButton02, check_off);
        }

        private void Clock1_Click(object sender, RoutedEventArgs e)
        {
            PlayButtonSound.Play();
            if (Properties.Settings.Default.Clock)
            {
                Properties.Settings.Default.Clock = false;
                Clock1.Content = String.Format(Properties.Resources.ClockString01, check_off);
                Clock2.IsEnabled = false;
                Clock3.IsEnabled = false;
            }
            else
            {
                Properties.Settings.Default.Clock = true;
                Clock1.Content = String.Format(Properties.Resources.ClockString01, check_on);
                Clock2.IsEnabled = true;
                Clock3.IsEnabled = true;
            }
        }

        private void Clock2_Click(object sender, RoutedEventArgs e)
        {
            PlayButtonSound.Play();
            if (!Properties.Settings.Default.AstClock)
            {
                Properties.Settings.Default.AstClock = true;
                Clock2.Content = String.Format(Properties.Resources.ClockString02, check_on);
                Clock3.Content = String.Format(Properties.Resources.ClockString03, check_off);
            }
        }

        private void Clock3_Click(object sender, RoutedEventArgs e)
        {
            PlayButtonSound.Play();
            if (Properties.Settings.Default.AstClock)
            {
                Properties.Settings.Default.AstClock = false;
                Clock2.Content = String.Format(Properties.Resources.ClockString02, check_off);
                Clock3.Content = String.Format(Properties.Resources.ClockString03, check_on);
            }
        }
    }
}
