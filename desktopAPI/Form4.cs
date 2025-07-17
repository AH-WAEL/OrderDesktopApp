using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace desktopAPI
{
    public partial class Form4 : Form
    {
        public string Username { get; private set; }
        public Form4()
        {
            InitializeComponent();
            Logging.LogUserAction("Navigation", "Form4 (Login) opened");

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }        private void button1_Click(object sender, EventArgs e)
        {
            Logging.LogUserAction("Authentication", "Login button clicked", "User attempting to login");
            
            if (textBox1.Text == "" || textBox2.Text == "")
            {
                MessageBox.Show("error! enter username and password");
                Logging.LogWarning("Authentication", "Login attempt failed", "Empty username or password provided");
                return;
            }
            
            Username = textBox1.Text.Trim();
            Logging.SetCurrentUsername(Username);
            Logging.LogUserAction("Authentication", "Login successful", $"User '{Username}' logged in successfully");
            
            Form1 form1 = new Form1();
            form1.Show();
            
            Logging.LogUserAction("Navigation", "Main form opened after login", "User redirected to main application form");
            this.Hide(); // Hide login form instead of closing to preserve the username
        }        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            Logging.LogUserAction("UI", "Password field changed", "User typing in password field");
        }private void textBox1_TextChanged(object sender, EventArgs e)
        {
            Logging.LogUserAction("UI", "Username field changed", "User typing in username field");
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }        private void Form4_Load(object sender, EventArgs e)
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
    }
}
