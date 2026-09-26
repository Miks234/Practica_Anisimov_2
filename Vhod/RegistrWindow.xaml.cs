using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Vhod.Models;

namespace Vhod
{
    public partial class RegistrWindow : Window
    {
        private static readonly SolidColorBrush NormalBrush = new SolidColorBrush(Colors.Gray);
        private static readonly SolidColorBrush ErrorBrush = new SolidColorBrush(Colors.Red);

        private const string ConnStr =
            @"data source=DESKTOP-NOBEE5C;initial catalog=Trade2;integrated security=True;trustservercertificate=True;MultipleActiveResultSets=True;App=EntityFramework";

        public RegistrWindow()
        {
            InitializeComponent();
            LoginBox.BorderBrush = NormalBrush;
        }

        private void LettersOnly_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !Regex.IsMatch(e.Text, @"^[A-Za-zА-Яа-яЁё\s\-]+$");
        }

        private void LoginBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string login = LoginBox.Text.Trim();

            if (string.IsNullOrWhiteSpace(login))
            {
                LoginBox.BorderBrush = NormalBrush;
                RegButton.IsEnabled = true;
                return;
            }

            bool exists = false;
            using (var conn = new System.Data.SqlClient.SqlConnection(ConnStr))
            {
                conn.Open();
                var cmd = new System.Data.SqlClient.SqlCommand(
                    "SELECT COUNT(*) FROM [User] WHERE User_Login = @login", conn);
                cmd.Parameters.AddWithValue("@login", login);
                exists = (int)cmd.ExecuteScalar() > 0;
            }

            if (exists)
            {
                LoginBox.BorderBrush = ErrorBrush;
                RegButton.IsEnabled = false;
                LoginBox.ToolTip = "Такой логин уже занят";
            }
            else
            {
                LoginBox.BorderBrush = NormalBrush;
                RegButton.IsEnabled = true;
                LoginBox.ToolTip = null;
            }
        }

        private void Reg_Click(object sender, RoutedEventArgs e)
        {
            if (!IsValidName(SurnameBox.Text) ||
                !IsValidName(NameBox.Text) ||
                !IsValidName(PatronymicBox.Text))
            {
                MessageBox.Show(
                    "Фамилия, имя и отчество должны содержать только буквы.",
                    "Ошибка ввода",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return;
            }

            string surname = SurnameBox.Text.Trim();
            string name = NameBox.Text.Trim();
            string patronymic = PatronymicBox.Text.Trim();
            string login = LoginBox.Text.Trim();
            string pass = PassBox.Text;

            if (string.IsNullOrWhiteSpace(surname) ||
                string.IsNullOrWhiteSpace(name) ||
                string.IsNullOrWhiteSpace(patronymic) ||
                string.IsNullOrWhiteSpace(login) ||
                string.IsNullOrWhiteSpace(pass))
            {
                MessageBox.Show(
                    "Заполните все поля.",
                    "Ошибка ввода",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return;
            }

            try
            {
                using (var conn = new System.Data.SqlClient.SqlConnection(ConnStr))
                {
                    conn.Open();

                    // Проверка уникальности
                    var checkCmd = new System.Data.SqlClient.SqlCommand(
                        "SELECT COUNT(*) FROM [User] WHERE User_Login = @login", conn);
                    checkCmd.Parameters.AddWithValue("@login", login);

                    if ((int)checkCmd.ExecuteScalar() > 0)
                    {
                        MessageBox.Show(
                            "Пользователь с таким логином уже существует.",
                            "Ошибка регистрации",
                            MessageBoxButton.OK,
                            MessageBoxImage.Warning);
                        return;
                    }

                    // Следующий ID
                    var idCmd = new System.Data.SqlClient.SqlCommand(
                        "SELECT ISNULL(MAX(User_ID), 0) + 1 FROM [User]", conn);
                    int nextId = (int)idCmd.ExecuteScalar();

                    // INSERT
                    var insCmd = new System.Data.SqlClient.SqlCommand(
                        "INSERT INTO [User] (User_ID, User_Role, User_Surname, User_Name, User_Patronymic, User_Login, User_Password, User_Status) " +
                        "VALUES (@id, 2, @sur, @name, @patr, @login, @pass, 0)", conn);

                    insCmd.Parameters.AddWithValue("@id", nextId);
                    insCmd.Parameters.AddWithValue("@sur", surname);
                    insCmd.Parameters.AddWithValue("@name", name);
                    insCmd.Parameters.AddWithValue("@patr", patronymic);
                    insCmd.Parameters.AddWithValue("@login", login);
                    insCmd.Parameters.AddWithValue("@pass", pass);

                    insCmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Ошибка сохранения: " + ex.Message,
                    "Ошибка базы данных",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                return;
            }

            MessageBox.Show(
                "Регистрация прошла успешно!",
                "Успех",
                MessageBoxButton.OK,
                MessageBoxImage.Information);

            new MainWindow().Show();
            Close();
        }

        private bool IsValidName(string s)
        {
            if (string.IsNullOrWhiteSpace(s)) return false;
            return Regex.IsMatch(s, @"^[A-Za-zА-Яа-яЁё\s\-]+$");
        }

        private void Nazad_Click(object sender, RoutedEventArgs e)
        {
            new MainWindow().Show();
            Close();
        }
    }
}