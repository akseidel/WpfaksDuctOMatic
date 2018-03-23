using System;
using System.Reflection;
using System.Windows;
using System.Windows.Input;
using System.Windows.Navigation;

namespace WpfaksDuctOMatic
{
    /// <summary>
    /// Interaction logic for AboutBoxDuctOMatic.xaml
    /// </summary>
    public partial class AboutBoxDuctOMatic : Window
    {
        private double cntrX;
        private double cntrY;
        private AboutClass about = new AboutClass();

        public AboutBoxDuctOMatic(double centerX, double centerY)
        {
            InitializeComponent();
            DataContext = about;
            cntrX = centerX ;
            cntrY = centerY ;
        }

        public void DragWindow(object sender, MouseButtonEventArgs args) {
            // Watch out. Fatal error if not primary button!
            if (args.LeftButton == MouseButtonState.Pressed) { DragMove(); }
        }
        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e) {
            System.Diagnostics.Process.Start(e.Uri.ToString());
        }

        private void Window_Loaded(object sender, RoutedEventArgs e) {
            UpdateLayout();
            Left = cntrX - ActualWidth / 2;
            Top = cntrY - ActualHeight / 2;
        }



    }
}
