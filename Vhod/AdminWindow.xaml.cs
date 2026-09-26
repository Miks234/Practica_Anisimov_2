using Vhod.Models;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace Vhod
{
    public partial class AdminWindow : Window
    {
        private const string ConnStr =
            @"data source=DESKTOP-NOBEE5C;initial catalog=Trade2;integrated security=True;trustservercertificate=True;MultipleActiveResultSets=True;App=EntityFramework";

        public AdminWindow()
        {
            InitializeComponent();
            LoadUsers();
        }

        private void LoadUsers()
        {
            var list = new List<User>();

            try
            {
                using (var conn = new System.Data.SqlClient.SqlConnection(ConnStr))
                {
                    conn.Open();
                    var cmd = new System.Data.SqlClient.SqlCommand(
                        "SELECT User_ID, User_Role, User_Surname, User_Name, " +
                        "User_Patronymic, User_Login, User_Password, User_Status " +
                        "FROM [User]", conn);

                    var reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        list.Add(new User
                        {
                            User_ID = (int)reader["User_ID"],
                            User_Role = (int)reader["User_Role"],
                            User_Surname = reader["User_Surname"].ToString(),
                            User_Name = reader["User_Name"].ToString(),
                            User_Patronymic = reader["User_Patronymic"]?.ToString(),
                            User_Login = reader["User_Login"].ToString(),
                            User_Password = reader["User_Password"].ToString(),
                            User_Status = (bool)reader["User_Status"]
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Ошибка загрузки пользователей: " + ex.Message,
                    "Ошибка базы данных",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }

            UsersItemControl.ItemsSource = null;
            UsersItemControl.ItemsSource = list;
        }

        private void ToggleUserStatus_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            if (btn == null) return;

            int userId = (int)btn.Tag;

            try
            {
                using (var conn = new System.Data.SqlClient.SqlConnection(ConnStr))
                {
                    conn.Open();
                    var cmd = new System.Data.SqlClient.SqlCommand(
                        "UPDATE [User] SET User_Status = CASE WHEN User_Status = 1 THEN 0 ELSE 1 END WHERE User_ID = @id",
                        conn);
                    cmd.Parameters.AddWithValue("@id", userId);
                    cmd.ExecuteNonQuery();
                }

                LoadUsers();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Ошибка обновления статуса: " + ex.Message,
                    "Ошибка базы данных",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            new MainWindow().Show();
            Close();
        }
    }
}