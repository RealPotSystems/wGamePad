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

namespace vGamePad.DialogWindow
{
    /// <summary>
    /// DialogWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class DialogWindow : Window
    {
        public enum DialogStyle
        {
            OK,
            OKCANCEL
        }
        public DialogWindow(string t, string m, DialogStyle s = DialogStyle.OK)
        {
            InitializeComponent();

            MessageTitle.Content = t;
            MessageText.Text = m;

            if (s == DialogStyle.OK)
            {
                OK.Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                Yes.Visibility = System.Windows.Visibility.Visible;
                No.Visibility = System.Windows.Visibility.Visible;
            }
        }

        private void OnOkClick(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void OnCancelClick(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}

