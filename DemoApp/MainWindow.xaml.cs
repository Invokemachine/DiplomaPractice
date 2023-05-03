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
using System.Threading;

namespace DemoApp
{
    public partial class MainWindow : Window
    {
        user25Entities1 dbmodel = new user25Entities1();

        public MainWindow()
        {
            InitializeComponent();
        }
        int LogInTriesCounter = 0;
        private void LogInButton_Click(object sender, RoutedEventArgs e)
        {
            if (!String.IsNullOrEmpty(LoginTextBox.Text) && !String.IsNullOrEmpty(UserPasswordBox.Password))
            {
                try
                {
                    foreach (User user in dbmodel.User)
                    {
                        if (user.UserLogin == LoginTextBox.Text && user.UserPassword == UserPasswordBox.Password)
                        {
                            MessageBox.Show("Авторизация успешна!", "Внимание");
                            ClientWindow clientWindow = new ClientWindow();
                            clientWindow.Show();
                            Close();
                        }
                        else if (user.UserLogin != LoginTextBox.Text && user.UserPassword == UserPasswordBox.Password)
                        {
                            MessageBox.Show("Неверный логин!", "Ошибка");
                            LogInTriesCounter = LogInTriesCounter + 1;
                            if (LogInTriesCounter >= 3)
                            {
                                Captcha captcha = new Captcha();
                                captcha.Show();
                                Close();
                            }
                        }
                        else if (user.UserPassword != UserPasswordBox.Password && user.UserLogin == LoginTextBox.Text)
                        {
                            MessageBox.Show("Неверный пароль!", "Ошибка");
                            LogInTriesCounter = LogInTriesCounter + 1;
                            if (LogInTriesCounter >= 3)
                            {
                                Captcha captcha = new Captcha();
                                captcha.Show();
                                Close();
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message.ToString(), "Ошибка");
                }
            }
            else
            {
                MessageBox.Show("Введите данные!", "Ошибка");
                LogInTriesCounter = LogInTriesCounter + 1;
                if (LogInTriesCounter >= 3)
                {
                    Captcha captcha = new Captcha();
                    captcha.Show();
                    Close();
                }
            }
        }

        private void LogInAsGuestButton_Click(object sender, RoutedEventArgs e)
        {
            ClientWindow clientWindow = new ClientWindow();
            clientWindow.Show();
            Close();
        }
    }
}
