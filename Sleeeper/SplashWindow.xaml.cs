using System.Windows;

namespace Sleeper
{
    public partial class SplashWindow : Window
    {
        public SplashWindow()
        {
            InitializeComponent();
        }

        // Video başarıyla biterse
        private void IntroVideo_MediaEnded(object sender, RoutedEventArgs e)
        {
            OpenMainWindow();
        }

        // Video BOZUKSA veya BULUNAMAZSA (Donmayı engeller)
        private void IntroVideo_MediaFailed(object sender, ExceptionRoutedEventArgs e)
        {
            OpenMainWindow();
        }

        private void OpenMainWindow()
        {
            MainWindow main = new MainWindow();
            main.Show();
            this.Close();
        }
    }
}