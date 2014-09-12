using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace vGamePad
{
    /// <summary>
    /// LayoutSetting.xaml の相互作用ロジック
    /// </summary>
    public partial class LayoutSetting : Page
    {
        private const string check_on = "\uE0A2";
        private const string check_off = "\uE003";

        List<UIElement> layoutList = new List<UIElement>();

        public LayoutSetting()
        {
            InitializeComponent();
            layoutList.Add(Layout3);    // デフォルト
            layoutList.Add(Layout1);
            layoutList.Add(Layout2);
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            // 設定ファイルの有無を確認し、ボタンの状態を変化させる
            Layout1.IsEnabled = vLayoutControl.LayoutFileExists(1);
            Layout2.IsEnabled = vLayoutControl.LayoutFileExists(2);

            int n = Properties.Settings.Default.Layout;

            Layout1.Content = string.Format("{0} {1}", n == 1 ? check_on : check_off, Properties.Resources.LayoutSettingLayout1);
            Layout2.Content = string.Format("{0} {1}", n == 2 ? check_on : check_off, Properties.Resources.LayoutSettingLayout2);
            Layout3.Content = string.Format("{0} {1}", n == 0 ? check_on : check_off, Properties.Resources.LayoutSettingLayoutDefault);
        }

        private void LayoutClick(object sender, RoutedEventArgs e)
        {
            PlayButtonSound.Play();
            switch(((Button)sender).Name)
            {
                case "Layout1":
                    Properties.Settings.Default.Layout = 1;
                    break;
                case "Layout2":
                    Properties.Settings.Default.Layout = 2;
                    break;
                case "Layout3":
                    Properties.Settings.Default.Layout = 0;
                    break;
            }
            int n = Properties.Settings.Default.Layout;

            Layout1.Content = string.Format("{0} {1}", n == 1 ? check_on : check_off, Properties.Resources.LayoutSettingLayout1);
            Layout2.Content = string.Format("{0} {1}", n == 2 ? check_on : check_off, Properties.Resources.LayoutSettingLayout2);
            Layout3.Content = string.Format("{0} {1}", n == 0 ? check_on : check_off, Properties.Resources.LayoutSettingLayoutDefault);

            // 選択されたレイアウトを適用する
            MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;
            mainWindow.SetLayout();
            mainWindow.SetConfig();
        }
    }
}
