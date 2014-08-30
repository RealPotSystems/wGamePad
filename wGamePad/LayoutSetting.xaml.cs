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
using System.IO;

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

            Layout1.Content = string.Format("{0} {1}", n == 1 ? check_on : check_off, "レイアウト1");
            Layout2.Content = string.Format("{0} {1}", n == 2 ? check_on : check_off, "レイアウト2");
            Layout3.Content = string.Format("{0} {1}", n == 0 ? check_on : check_off, "デフォルト レイアウト");
        }
    }
}
