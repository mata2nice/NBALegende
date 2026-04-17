using NBALegende.Models;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace NBALegende
{
    public partial class MainWindow : Window
    {
        private bool isPasswordVisible = false;
        private bool isUpdatingPassword = false;
        private List<User> users;

        public MainWindow()
        {
            InitializeComponent();

            UserDataIO userDataIO = new UserDataIO();
            users = userDataIO.LoadUsers();
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            string username = tbUsername.Text.Trim();
            string password;

            if (isPasswordVisible)
            {
                password = tbPasswordVisible.Text;
            }
            else
            {
                password = pbPassword.Password;
            }

            User loggedUser = users.FirstOrDefault(u => u.Username == username && u.Password == password);

            if (loggedUser == null)
            {
                MessageBox.Show(
                    "Pogrešno korisničko ime ili lozinka.",
                    "Greška pri prijavi",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                return;
            }
            LegendsTableWindow legendsTableWindow = new LegendsTableWindow(loggedUser);
            legendsTableWindow.Show();
            this.Close();
        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void tbUsername_GotFocus(object sender, RoutedEventArgs e)
        {
            tbUsernamePlaceholder.Visibility = Visibility.Hidden;
        }

        private void tbUsername_LostFocus(object sender, RoutedEventArgs e)
        {
            if (tbUsername.Text == "")
            {
                tbUsernamePlaceholder.Visibility = Visibility.Visible;
            }
        }

        private void tbUsername_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (tbUsername.Text == "")
            {
                tbUsernamePlaceholder.Visibility = Visibility.Visible;
            }
            else
            {
                tbUsernamePlaceholder.Visibility = Visibility.Hidden;
            }
        }

        private void UpdatePasswordPlaceholder()
        {
            string passwordText;

            if (isPasswordVisible)
            {
                passwordText = tbPasswordVisible.Text;
            }
            else
            {
                passwordText = pbPassword.Password;
            }

            if (passwordText == "")
            {
                pbPasswordPlaceholder.Visibility = Visibility.Visible;
            }
            else
            {
                pbPasswordPlaceholder.Visibility = Visibility.Hidden;
            }
        }

        private void pbPassword_GotFocus(object sender, RoutedEventArgs e)
        {
            pbPasswordPlaceholder.Visibility = Visibility.Hidden;
        }

        private void pbPassword_LostFocus(object sender, RoutedEventArgs e)
        {
            UpdatePasswordPlaceholder();
        }

        private void pbPassword_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (isUpdatingPassword)
            {
                return;
            }

            isUpdatingPassword = true;
            tbPasswordVisible.Text = pbPassword.Password;
            isUpdatingPassword = false;

            UpdatePasswordPlaceholder();
        }

        private void tbPasswordVisible_GotFocus(object sender, RoutedEventArgs e)
        {
            pbPasswordPlaceholder.Visibility = Visibility.Hidden;
        }

        private void tbPasswordVisible_LostFocus(object sender, RoutedEventArgs e)
        {
            UpdatePasswordPlaceholder();
        }

        private void tbPasswordVisible_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (isUpdatingPassword)
            {
                return;
            }

            isUpdatingPassword = true;
            pbPassword.Password = tbPasswordVisible.Text;
            isUpdatingPassword = false;

            UpdatePasswordPlaceholder();
        }

        private void txtTogglePassword_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (isPasswordVisible == false)
            {
                tbPasswordVisible.Text = pbPassword.Password;
                tbPasswordVisible.Visibility = Visibility.Visible;
                pbPassword.Visibility = Visibility.Collapsed;
                txtTogglePassword.Text = "Sakrij lozinku";
                isPasswordVisible = true;

                tbPasswordVisible.Focus();
                tbPasswordVisible.CaretIndex = tbPasswordVisible.Text.Length;
            }
            else
            {
                pbPassword.Password = tbPasswordVisible.Text;
                pbPassword.Visibility = Visibility.Visible;
                tbPasswordVisible.Visibility = Visibility.Collapsed;
                txtTogglePassword.Text = "Prikaži lozinku";
                isPasswordVisible = false;

                pbPassword.Focus();
            }

            UpdatePasswordPlaceholder();
        }
    }
}