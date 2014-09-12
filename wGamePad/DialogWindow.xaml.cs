using System;
using System.Windows;
using System.Windows.Controls;

namespace vGamePad.DialogWindow
{
    /// <summary>
    /// DialogWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class DialogWindow : Window
    {
        public int? result { private set; get; }

        const string _OK = "\uE17E\uE171";          // !
        const string _OKCANCEL = "\uE17E\uE11B";    // ?

        public enum DialogStyle
        {
            OK,
            OKCANCEL,
            ORIGINAL
        }

        public DialogWindow(string t, string m, DialogStyle s = DialogStyle.OK)
        {
            InitializeComponent();

            MessageTitle.Content = t;
            MessageText.Text = m;

            if (s == DialogStyle.OK)
            {
                Type.Content = _OK;
                OK.Visibility = System.Windows.Visibility.Visible;
            }
            else if (s == DialogStyle.OKCANCEL)
            {
                Type.Content = _OKCANCEL;
                Yes.Visibility = System.Windows.Visibility.Visible;
                No.Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                Type.Content = _OKCANCEL;
                Botton1.Visibility = System.Windows.Visibility.Visible;
                Botton2.Visibility = System.Windows.Visibility.Visible;
                Botton3.Visibility = System.Windows.Visibility.Visible;
            }
        }

        private void OnOkClick(object sender, RoutedEventArgs e)
        {
            PlayButtonSound.Play();
            DialogResult = true;
            Close();
        }

        private void OnCancelClick(object sender, RoutedEventArgs e)
        {
            PlayButtonSound.Play();
            DialogResult = false;
            Close();
        }

        private void OnBottonClick(object sender, RoutedEventArgs e)
        {
            PlayButtonSound.Play();
            var botton = (Button)sender;
            switch(botton.Name)
            {
                case "Botton1":
                    result = 1;
                    break;
                case "Botton2":
                    result = 2;
                    break;
                case "Botton3":
                    result = 3;
                    break;
            }
            Close();
        }
    }
}

