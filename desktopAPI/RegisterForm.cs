using desktopAPI.Models;
using desktopAPI.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Http.Json;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
namespace desktopAPI
{
    public partial class RegisterForm : Form
    {
        private HttpClient client = new HttpClient();
        public RegisterForm()
        {
            InitializeComponent();
            label6.Visible = false;
            textBox3.UseSystemPasswordChar = true;
            textBox4.UseSystemPasswordChar = true;
            client.BaseAddress = new Uri("http://localhost:50000/api/Users/");
            Logging.LogUserAction("Navigation", "RegisterForm opened");

        }

        private void RegisterForm_Load(object sender, EventArgs e)
        {


        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                // If checkbox is checked, show password
                textBox3.UseSystemPasswordChar = false;
                textBox4.UseSystemPasswordChar = false;
            }
            else
            {
                // If checkbox is unchecked, hide password
                textBox3.UseSystemPasswordChar = true;
                textBox4.UseSystemPasswordChar = true;
            }
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            label6.Visible = false;
            if (string.IsNullOrWhiteSpace(textBox1.Text) || string.IsNullOrWhiteSpace(textBox2.Text) || string.IsNullOrWhiteSpace(textBox3.Text) || string.IsNullOrWhiteSpace(textBox4.Text))
            {
                System.Media.SystemSounds.Hand.Play();
                label6.Text = "Please fill out all the boxes!";
                label6.Visible = true;
                return;
            }

            if (!IsValidEmail(textBox2.Text))
            {
                System.Media.SystemSounds.Hand.Play();
                label6.Text = "Please enter a valid email address!";
                label6.Visible = true;
                return;
            }


            if (textBox3.Text != textBox4.Text)
            {
                System.Media.SystemSounds.Hand.Play();
                label6.Text = "The passwords dont match!";
                label6.Visible = true;
                return;
            }

            button1.Enabled = false;
            button1.Text = "Registering...";

            UserRegisterDTO userRegisterDTO = new UserRegisterDTO
            {
                username = textBox1.Text.Trim(),
                email = textBox2.Text.Trim(),
                password = textBox3.Text.Trim(),
                Role = "User"
            };

            var response = await client.PostAsJsonAsync("register", userRegisterDTO);
            var responseContent = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                registerResponseDto registerResponseDto = JsonConvert.DeserializeObject<registerResponseDto>(responseContent);
                var result = MessageBox.Show($"Register Successful!\n" +
                                      $"Welcome: {registerResponseDto.username}\n" +
                                      $"Role: {registerResponseDto.Role}\n" +
                                      $"Email: {registerResponseDto.email}\n");

                this.Hide();
                var loginForm = new Login();
                loginForm.Show();
                this.Close();

            }
            else
            {
                button1.Enabled = Enabled;
                button1.Text = "Register";

                var result = MessageBox.Show("username or email is already used! Do you want to login?", "Account already exists", MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    this.Hide();
                    var loginForm = new Login();
                    loginForm.Show();
                    this.Close();
                }
            }

        }

        private bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            try
            {
                var mailAddress = new MailAddress(email);
                return mailAddress.Address == email;
            }
            catch
            {
                return false;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Hide();
            var loginForm = new Login();
            loginForm.Show();
            this.Close();
        }
    }

}


