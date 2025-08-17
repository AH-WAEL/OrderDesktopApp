using desktopAPI.Models;
using desktopAPI.Services;
using Newtonsoft.Json;
using System;
using System.Net.Http.Json;
using System.Windows.Forms;

namespace desktopAPI
{
    public partial class Login : Form
    {
        public string Username { get; private set; }
        private HttpClient client = new HttpClient();

        public Login()
        {
            InitializeComponent();
            textBox2.UseSystemPasswordChar = true;
            client.BaseAddress = new Uri("http://localhost:50000/api/Users/");
            Logging.LogUserAction("Navigation", "Form4 (Login) opened");
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            Logging.LogUserAction("Authentication", "Login button clicked", "User attempting to login");

            if (string.IsNullOrWhiteSpace(textBox1.Text) || string.IsNullOrWhiteSpace(textBox2.Text))
            {
                MessageBox.Show("Error! Enter username and password");
                Logging.LogWarning("Authentication", "Login attempt failed", "Empty username or password provided");
                return;
            }

            button1.Enabled = false;
            button1.Text = "Logging in...";

            try
            {
                var loginDto = new LoginDto
                {
                    username = textBox1.Text.Trim(),
                    password = textBox2.Text.Trim()
                };

                var response = await client.PostAsJsonAsync("login", loginDto);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    TokenResponseDto tokenResponseDto = JsonConvert.DeserializeObject<TokenResponseDto>(responseContent);

                    // Set tokens
                    AuthApiService.SetTokens(tokenResponseDto.AccessToken, tokenResponseDto.RefreshToken, tokenResponseDto.RefreshTokenExpiryTime);

                    if (!string.IsNullOrEmpty(AuthApiService.Username))
                    {
                        MessageBox.Show($"Login Successful!\n" +
                                      $"Welcome: {AuthApiService.Username}\n" +
                                      $"Role: {AuthApiService.Role}\n" +
                                      $"Email: {AuthApiService.Email}\n" +
                                      $"Refresh Token expires: {tokenResponseDto.RefreshTokenExpiryTime:dd/MM/yyyy HH:mm:ss}");

                        Username = AuthApiService.Username;

                        Logging.SetCurrentUsername(AuthApiService.Username);
                        Logging.LogUserAction("Authentication", "Login successful", $"User '{loginDto.username}' logged in successfully");

                        textBox1.Clear();
                        textBox2.Clear();
                        this.Hide();



                        if (AuthApiService.Role == Roles.Admin)
                        {
                            adminForm mainForm = new adminForm();
                            mainForm.Show();

                        }
                        else
                        {
                            adminForm mainForm = new adminForm();
                            mainForm.Show();

                            //Form1 mainForm = new Form1();
                            //mainForm.Show();
                        }

                        Logging.LogUserAction("Navigation", "Main form opened after login", "User redirected to main application form");
                    }
                    else
                    {
                        MessageBox.Show("Login succeeded but failed to parse user information from token.");
                    }
                }
                else
                {
                    MessageBox.Show($"Login failed: {responseContent}");
                    Logging.LogWarning("Authentication", "Login failed", responseContent);
                    textBox2.Clear();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Login error: {ex.Message}");
                Logging.LogError("Authentication", "Login exception", ex);
            }
            finally
            {
                button1.Enabled = true;
                button1.Text = "Login";
            }
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            Logging.LogUserAction("UI", "Password field changed", "User typing in password field");
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            Logging.LogUserAction("UI", "Username field changed", "User typing in username field");
        }

        private void Form4_Load(object sender, EventArgs e)
        {
            Logging.LogUserAction("UI", "Form4 Load event triggered", "Login form loaded and ready for user input");
        }

        private void Form4_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (string.IsNullOrEmpty(Username))
            {
                Logging.LogUserAction("Navigation", "Form4 (Login) closed without login", "User closed login form without providing credentials");
            }
            else
            {
                Logging.LogUserAction("Navigation", "Form4 (Login) closing", "Login form closing after successful authentication");
            }
        }

        // Allow Enter key to trigger login
        private void textBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                button1_Click(sender, e);
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                // If checkbox is checked, show password
                textBox2.UseSystemPasswordChar = false;
            }
            else
            {
                // If checkbox is unchecked, hide password
                textBox2.UseSystemPasswordChar = true;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            RegisterForm mainForm = new RegisterForm();
            mainForm.Show();
            this.Hide();
        }
    }

}
public class Roles
{
    public static string Admin => "Admin";

}
