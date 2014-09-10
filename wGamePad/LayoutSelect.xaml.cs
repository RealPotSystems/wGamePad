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

namespace vGamePad
{
    /// <summary>
    /// LayoutSelect.xaml の相互作用ロジック
    /// </summary>
    public partial class LayoutSelect : Page
    {
        public LayoutSelect()
        {
            InitializeComponent();
        }

        private void Set_Click(object sender, RoutedEventArgs e)
        {
            PlayButtonSound.Play();
            ConfigWindow.navigation.Navigate(new Uri("LayoutSetting.xaml", UriKind.Relative));
        }

        private void Create_Click(object sender, RoutedEventArgs e)
        {
            PlayButtonSound.Play();
            // レイアウトモードに移行してもよいかの確認
            var dialog = new DialogWindow.DialogWindow("ボタンレイアウトの作成", "ボタンレイアウト作成モードに移行します。\nよろしいですか？", DialogWindow.DialogWindow.DialogStyle.OKCANCEL);
            var ret = dialog.ShowDialog();
            if (ret == true)
            {
                var configWindow = (ConfigWindow)Window.GetWindow(this);
                configWindow.ConfigGrid.Visibility = System.Windows.Visibility.Hidden;
                // レイアウト作成モードに移行
                // レイアウトモードは上にコマンドエリアを表示
                var layoutWindow = new LayoutWindow();
                layoutWindow.ShowDialog();
                configWindow.ConfigGrid.Visibility = System.Windows.Visibility.Visible;
            }
        }
    }
}
