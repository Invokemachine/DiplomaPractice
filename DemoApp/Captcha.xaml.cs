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
using System.Threading;
using System.Windows.Threading;
using System.Diagnostics.Eventing.Reader;

namespace DemoApp
{
    /// <summary>
    /// Логика взаимодействия для Captcha.xaml
    /// </summary>
    public partial class Captcha : Window
    {
        public Captcha()
        {
            InitializeComponent();
        }

        private int time = 15;
        public string str;
        private void CaptchaGenerator()
        {
            Random rnd = new Random();
            int value1 = rnd.Next(0, 10);
            int value2 = rnd.Next(0, 10);
            int value3 = rnd.Next(0, 10);
            int value4 = rnd.Next(0, 10);
            int value5 = rnd.Next(0, 10);
            str = Convert.ToString(value1) + Convert.ToString(value2) + Convert.ToString(value3) + Convert.ToString(value4) + Convert.ToString(value5);
            CaptchaLabel.Content = str;
        }
        private void InputButton_Click(object sender, RoutedEventArgs e)
        {
            time = 15;
            if (str == CaptchaTextBox.Text)
            {
                MessageBox.Show("Верно!", "Внимание!");
                MainWindow mainWindow = new MainWindow();
                mainWindow.Show();
                Close();
            }
            else
            {
                MessageBox.Show("Попробуйте снова!", "Ошибка!");
                CaptchaGenerator();
                InitializeTimer();
            }
        }

        private DispatcherTimer Timer;
        private void InitializeTimer()
        {
            Timer = new DispatcherTimer();
            Timer.Interval = new TimeSpan(0, 0, 1);
            Timer.Tick += Timer_Tick;
            Timer.Start();
        }

        void Timer_Tick(object sender, EventArgs e)
        {
            if (time > 10)
            {
                time--;
                InputButton.IsEnabled = false;
                CaptchaTextBox.IsEnabled = false;
            }
            else
            {
                InputButton.IsEnabled = true;
                CaptchaTextBox.IsEnabled = true;
                Timer.Stop();
            }
        }

        private void UpdateCaptcha_Click(object sender, RoutedEventArgs e)
        {
            CaptchaGenerator();
        }
    }
}
