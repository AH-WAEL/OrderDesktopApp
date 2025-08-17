using desktopAPI.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace desktopAPI
{
    public partial class adminForm : Form
    {
        public adminForm()
        {
            InitializeComponent();
            Logging.LogUserAction("Navigation", "AdminForm opened", "Admin form initialized");
        }

        private void adminForm_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            ProductsForm mainForm = new ProductsForm();
            mainForm.Show();
            this.Hide();
        }
    }
}
