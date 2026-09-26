using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Vhod.Models;

namespace Vhod
{
    public partial class MainWindow : Window
    {
        private int failedAttempts = 0;
        private int failedPuzzles = 0;

        private bool captchaRequired = false;
        private int[] currentOrder = { 0, 1, 2, 3 };
        private int selectedIndex = -1;
        private string currentLogin = "";

        private const string ConnStr =
            @"data source=DESKTOP-NOBEE5C;initial catalog=Trade2;integrated security=True;trustservercertificate=True;MultipleActiveResultSets=True;App=EntityFramework";

        public MainWindow()
        {
            InitializeComponent();

            failedAttempts = 0;
            failedPuzzles = 0;
            captchaRequired = false;
            currentLogin = "";
            selectedIndex = -1;
            currentOrder = new[] { 0, 1, 2, 3 };

            LoginBorder.Visibility = Visibility.Visible;
            CaptchaBorder.Visibility = Visibility.Collapsed;
        }

        private void Vhod_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(LoginBox.Text))
                {
                    MessageBox.Show(
                        "Поле «Логин» обязательно для заполнения.",
                        "Ошибка ввода",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);
                    LoginBox.Focus();
                    return;
                }

                if (string.IsNullOrWhiteSpace(PassBox.Password))
                {
                    MessageBox.Show(
                        "Поле «Пароль» обязательно для заполнения.",
                        "Ошибка ввода",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);
                    PassBox.Focus();
                    return;
                }

                if (captchaRequired)
                {
                    if (CaptchaBorder.Visibility != Visibility.Visible)
                    {
                        CaptchaBorder.Visibility = Visibility.Visible;
                        LoginBorder.Visibility = Visibility.Collapsed;
                        LoadPuzzleImages();
                    }

                    MessageBox.Show(
                        "Сначала соберите пазл, чтобы продолжить вход.",
                        "Требуется капча",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);
                    return;
                }

                bool isBlocked = false;
                using (var conn = new System.Data.SqlClient.SqlConnection(ConnStr))
                {
                    conn.Open();
                    var cmd = new System.Data.SqlClient.SqlCommand(
                        "SELECT User_Status FROM [User] WHERE User_Login = @login", conn);
                    cmd.Parameters.AddWithValue("@login", LoginBox.Text);
                    var result = cmd.ExecuteScalar();

                    if (result != null && result != DBNull.Value)
                        isBlocked = (bool)result;
                }

                if (isBlocked)
                {
                    MessageBox.Show(
                        "Вы заблокированы. Обратитесь к администратору",
                        "Доступ запрещён",
                        MessageBoxButton.OK,
                        MessageBoxImage.Stop);
                    return;
                }

                User user = null;
                using (var conn = new System.Data.SqlClient.SqlConnection(ConnStr))
                {
                    conn.Open();
                    var cmd = new System.Data.SqlClient.SqlCommand(
                        "SELECT User_ID, User_Role, User_Surname, User_Name, " +
                        "User_Patronymic, User_Login, User_Password, User_Status " +
                        "FROM [User] WHERE User_Login = @login AND User_Password = @pass", conn);
                    cmd.Parameters.AddWithValue("@login", LoginBox.Text);
                    cmd.Parameters.AddWithValue("@pass", PassBox.Password);

                    var reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        user = new User
                        {
                            User_ID = (int)reader["User_ID"],
                            User_Role = (int)reader["User_Role"],
                            User_Surname = reader["User_Surname"].ToString(),
                            User_Name = reader["User_Name"].ToString(),
                            User_Patronymic = reader["User_Patronymic"]?.ToString(),
                            User_Login = reader["User_Login"].ToString(),
                            User_Password = reader["User_Password"].ToString(),
                            User_Status = (bool)reader["User_Status"]
                        };
                    }
                }

                if (user == null)
                {
                    failedAttempts++;

                    MessageBox.Show(
                        "Вы ввели неверный логин или пароль. Пожалуйста проверьте ещё раз введенные данные.",
                        "Ошибка авторизации",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);

                    if (failedAttempts >= 3)
                    {
                        BlockAndShow(LoginBox.Text);
                    }
                    else
                    {
                        ShowCaptcha(LoginBox.Text);
                    }
                    return;
                }

                failedAttempts = 0;
                failedPuzzles = 0;
                currentLogin = "";
                captchaRequired = false;
                

                MessageBox.Show(
                    "Вы успешно авторизовались",
                    "Успех",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);

                switch (user.User_Role)
                {
                    case 1:
                        new AdminWindow().Show();
                        Close();
                        break;
                    case 2:
                        MessageBox.Show(
                            "Добро пожаловать в систему!",
                            "Авторизация",
                            MessageBoxButton.OK,
                            MessageBoxImage.Information);
                        break;
                    default:
                        MessageBox.Show(
                            "У данного пользователя несуществующая роль. Обратитесь к администратору.",
                            "Ошибка роли",
                            MessageBoxButton.OK,
                            MessageBoxImage.Error);
                        break;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Произошла непредвиденная ошибка: " + ex.Message +
                    "\nПроверьте соединение с базой данных.",
                    "Ошибка базы данных",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }
        private void ShowCaptcha(string login)
        {
            captchaRequired = true;
            currentLogin = login;

            LoginBorder.Visibility = Visibility.Collapsed;
            CaptchaBorder.Visibility = Visibility.Visible;

            var rnd = new Random();
            currentOrder = new[] { 0, 1, 2, 3 }.OrderBy(x => rnd.Next()).ToArray();

            LoadPuzzleImages();
        }

        private void LoadPuzzleImages()
        {
            string imgDir = @"C:\Users\anisi\source\repos\Vhod\Vhod\Img";
            var images = new[] { P0, P1, P2, P3 };

            for (int i = 0; i < 4; i++)
            {
                int picNumber = currentOrder[i] + 1;
                string file = System.IO.Path.Combine(imgDir, picNumber + ".png");

                if (System.IO.File.Exists(file))
                    images[i].Source = new BitmapImage(new Uri(file));
                else
                    images[i].Source = null;
            }

            selectedIndex = -1;
        }

        private void PuzzlePiece_Click(object sender, MouseButtonEventArgs e)
        {
            var border = sender as Border;
            if (border == null) return;

            int index = int.Parse(border.Tag.ToString());

            if (selectedIndex == -1)
            {
                selectedIndex = index;
            }
            else
            {
                int tmp = currentOrder[selectedIndex];
                currentOrder[selectedIndex] = currentOrder[index];
                currentOrder[index] = tmp;

                selectedIndex = -1;
                LoadPuzzleImages();
            }
        }

        private void CheckPuzzle_Click(object sender, RoutedEventArgs e)
        {
            if (!captchaRequired) return;

            bool ok = currentOrder[0] == 0
                   && currentOrder[1] == 1
                   && currentOrder[2] == 2
                   && currentOrder[3] == 3;

            if (ok)
            {
                MessageBox.Show(
                    "Пазл собран верно! Теперь вы можете авторизоваться.",
                    "Успех",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);

                failedPuzzles = 0;
                captchaRequired = false;

                CaptchaBorder.Visibility = Visibility.Collapsed;
                LoginBorder.Visibility = Visibility.Visible;
            }
            else
            {
                failedPuzzles++;

                MessageBox.Show(
                    "Пазл собран неверно. Попробуйте ещё раз.",
                    "Ошибка капчи",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                if (failedPuzzles >= 3)
                {
                    BlockAndShow(currentLogin);
                }
                else
                {
                    var rnd = new Random();
                    currentOrder = currentOrder.OrderBy(x => rnd.Next()).ToArray();
                    LoadPuzzleImages();
                }
            }
        }
        private void BlockAndShow(string login)
        {
            if (string.IsNullOrWhiteSpace(login))
            {
                MessageBox.Show(
                    "Вы заблокированы. Обратитесь к администратору",
                    "Доступ запрещён",
                    MessageBoxButton.OK,
                    MessageBoxImage.Stop);
                return;
            }

            try
            {
                using (var conn = new System.Data.SqlClient.SqlConnection(ConnStr))
                {
                    conn.Open();
                    var cmd = new System.Data.SqlClient.SqlCommand(
                        "UPDATE [User] SET User_Status = 1 WHERE User_Login = @login", conn);
                    cmd.Parameters.AddWithValue("@login", login);
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Не удалось заблокировать пользователя: " + ex.Message,
                    "Ошибка базы данных",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }

            MessageBox.Show(
                "Вы заблокированы. Обратитесь к администратору",
                "Доступ запрещён",
                MessageBoxButton.OK,
                MessageBoxImage.Stop);

            CaptchaBorder.Visibility = Visibility.Collapsed;
            LoginBorder.Visibility = Visibility.Visible;

            failedAttempts = 0;
            failedPuzzles = 0;
            captchaRequired = false;
            currentLogin = "";
        }
        private void Reg_Click(object sender, RoutedEventArgs e)
        {
            new RegistrWindow().Show();
            Close();
        }
    }
}