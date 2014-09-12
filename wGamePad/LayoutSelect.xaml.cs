using System;
using System.Windows;
using System.Windows.Controls;

namespace vGamePad
{
    /// <summary>
    /// LayoutSelect.xaml の相互作用ロジック
    /// </summary>
    public partial class LayoutSelect : Page
    {
        private const string nextpage = "LayoutSetting.xaml";

        public LayoutSelect()
        {
            InitializeComponent();
        }

        private void Set_Click(object sender, RoutedEventArgs e)
        {
            PlayButtonSound.Play();
            var configWindow = (ConfigWindow)Window.GetWindow(this);
            configWindow.navigation.Navigate(new Uri(nextpage, UriKind.Relative));
        }

        private void Create_Click(object sender, RoutedEventArgs e)
        {
            PlayButtonSound.Play();
            // レイアウトモードに移行してもよいかの確認
            var dialog = new DialogWindow.DialogWindow(
                Properties.Resources.LayoutSelectTitle,
                Properties.Resources.LayoutSelectMessage,
                DialogWindow.DialogWindow.DialogStyle.OKCANCEL);
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
