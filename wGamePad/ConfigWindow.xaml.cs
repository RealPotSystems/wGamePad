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
using System.Windows.Shapes;

namespace vGamePad
{
    /// <summary>
    /// ConfigWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class ConfigWindow : Window
    {
        private const string versionFormat = "{0} version {1}\nCopyright © 2014 Real Pot Systems (TAKUBON).\n";
        private const string sound_on = "\uE15D";
        private const string sound_off = "\uE198";

        public ConfigWindow()
        {
            InitializeComponent();

            System.Diagnostics.FileVersionInfo ver =
                System.Diagnostics.FileVersionInfo.GetVersionInfo(
                System.Reflection.Assembly.GetExecutingAssembly().Location);
            About.Text = string.Format(versionFormat, ver.ProductName, ver.ProductVersion);
            About.Text += MainWindow.devCon.ToString();
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
    }
}
