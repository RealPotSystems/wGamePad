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

namespace wGamePad
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Keyboard_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // 反転
            foreach(object child in ((Grid)sender).Children)
            {
                if (child is Ellipse)
                {
                    ((Ellipse)child).Fill = new SolidColorBrush(Colors.Black);
                }
                if (child is Label)
                {
                    ((Label)child).Foreground = new SolidColorBrush(Colors.White);
                }
            }
        }

        private void Keyboard_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            // 戻し
            foreach (object child in ((Grid)sender).Children)
            {
                if (child is Ellipse)
                {
                    ((Ellipse)child).Fill = new SolidColorBrush(Colors.White);
                }
                if (child is Label)
                {
                    ((Label)child).Foreground = new SolidColorBrush(Colors.Black);
                }
            }

        }
    }
}
