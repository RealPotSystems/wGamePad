using System;
using System.Windows;
using System.Windows.Input;
using System.Diagnostics;
using System.Reflection;
using System.Collections.Generic;
using System.Windows.Media;
using System.Windows.Shapes;
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
        private const string stick = "\uE10A";

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

            AstTime.Content = Properties.Settings.Default.Clock ?
                String.Format(Properties.Resources.ConfigButton02, check_on) :
                String.Format(Properties.Resources.ConfigButton02, check_off);

            BtyTime.Content = Properties.Settings.Default.Battery ?
                String.Format(Properties.Resources.ConfigButton03, check_on) :
                String.Format(Properties.Resources.ConfigButton03, check_off);
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
                int ret = (int)Microsoft.Win32.Registry.GetValue("HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\AutoRotation", "SensorPresent", 0);
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
                int ret = (int)Microsoft.Win32.Registry.GetValue("HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\AutoRotation", "Enable", 0);
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

        private void Maintenance_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;

            if (App.MaintenanceMode)
            {
                App.MaintenanceMode = false;

                mainWindow.LeftContent.Content = stick;
                mainWindow.RightContent.Content = stick;

                List<UIElement> uiList = new List<UIElement>();
                foreach (object c in mainWindow.vGamePadCanvas.Children)
                {
                    // System.Windows.Shapes.Path
                    Debug.WriteLine(((UIElement)c).GetType());
                    if (((UIElement)c).GetType().ToString() == "System.Windows.Shapes.Path")
                    {
                        uiList.Add((UIElement)c);
                    }
                }
                foreach (var c in uiList)
                {
                    mainWindow.vGamePadCanvas.Children.Remove(c);
                }
                mainWindow.vGamePadCanvas.Background = new SolidColorBrush(Colors.Transparent);
            }
            else
            {
                App.MaintenanceMode = true;

                ScaleTransform scaleTransform = new ScaleTransform();

                mainWindow.LeftContent.Content = "左";
                mainWindow.RightContent.Content = "右";
                mainWindow.vGamePadCanvas.Background = new SolidColorBrush(Colors.DarkGray);
                for (int i = 0; i < mainWindow.vGamePadCanvas.ActualWidth; i += App.GRID)
                {
                    Path path = new Path()
                    {
                        Data = new LineGeometry(new Point(i, 0), new Point(i, mainWindow.vGamePadCanvas.ActualHeight)),
                        Stroke = Brushes.White,
                        StrokeThickness = .5
                    };

                    path.Data.Transform = scaleTransform;

                    mainWindow.vGamePadCanvas.Children.Add(path);
                }

                // 横線
                for (int i = 0; i < mainWindow.vGamePadCanvas.ActualHeight; i += App.GRID)
                {
                    Path path = new Path()
                    {
                        Data = new LineGeometry(new Point(0, i), new Point(mainWindow.vGamePadCanvas.ActualWidth, i)),
                        Stroke = Brushes.White,
                        StrokeThickness = .5
                    };

                    path.Data.Transform = scaleTransform;

                    mainWindow.vGamePadCanvas.Children.Add(path);
                }
            }
            Close();
        }

        private void Skeleton_Click(object sender, RoutedEventArgs e)
        {
           Properties.Settings.Default.Skeleton = Properties.Settings.Default.Skeleton ? false : true;
           Skeleton.Content = Properties.Settings.Default.Skeleton ?
               String.Format(Properties.Resources.ConfigButton01, check_on) :
               String.Format(Properties.Resources.ConfigButton01, check_off);
        }

        private void AstTime_Click(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.Clock = Properties.Settings.Default.Clock ? false : true;
            AstTime.Content = Properties.Settings.Default.Clock ?
                String.Format(Properties.Resources.ConfigButton02, check_on) :
                String.Format(Properties.Resources.ConfigButton02, check_off);
        }

        private void BtyTime_Click(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.Battery = Properties.Settings.Default.Battery ? false : true;
            BtyTime.Content = Properties.Settings.Default.Battery ?
                String.Format(Properties.Resources.ConfigButton03, check_on) :
                String.Format(Properties.Resources.ConfigButton03, check_off);
        }
    }
}
