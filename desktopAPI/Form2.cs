using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace desktopAPI
{
    public partial class Form2 : Form
    {
        private string id;
        private HttpClient client = new HttpClient(); 
        public Form2(string ID)
        {
            InitializeComponent();
            client.BaseAddress = new Uri("http://localhost:8080/api/");
            id = ID;

            if (int.TryParse(ID, out int orderId))
            {
                Logging.LogUserAction("Navigation", "Form2 (Edit Order) opened", $"Editing order ID: {orderId}");
            }
            else
            {
                Logging.LogUserAction("Navigation", "Form2 (Create Order) opened", "Creating new order");
            }
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
        private async void button1_Click(object sender, EventArgs e)
        {
            if (int.TryParse(id, out int ID))
            {
                Logging.LogUserAction("API", "Update Order button clicked", $"Attempting to update order ID: {ID}");

                if (string.IsNullOrWhiteSpace(textBox1.Text) || string.IsNullOrWhiteSpace(textBox2.Text) || string.IsNullOrWhiteSpace(textBox3.Text))
                {
                    MessageBox.Show("Please enter all the fields");
                    Logging.LogWarning("Validation", "Incomplete form data for update", "One or more fields are empty");
                    return;
                }
                try
                {

                    var Orders = new Order
                    {
                        Item = textBox1.Text,
                        Quantity = int.Parse(textBox2.Text),
                        OrderDate = DateTime.Now,
                        TotalPrice = decimal.Parse(textBox3.Text)
                    };

                    Logging.LogUserAction("API", "Sending update request", $"Order ID: {ID}, Item: {Orders.Item}, Quantity: {Orders.Quantity}, Price: {Orders.TotalPrice}");

                    var updateResponse = await client.PutAsJsonAsync($"order/{ID}", Orders);
                    if (updateResponse.IsSuccessStatusCode)
                    {
                        dataGridView1.DataSource = await client.GetFromJsonAsync<List<Order>>("order");
                        Logging.LogUserAction("API", "Update Order successful", $"Order ID {ID} updated successfully");
                        MessageBox.Show("Order updated successfully!");
                    }
                    else
                    {
                        MessageBox.Show("Error updating order: " + updateResponse.ReasonPhrase);
                        Logging.LogWarning("API", "Update Order failed", $"Order ID {ID}: {updateResponse.ReasonPhrase}");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("An error occurred: " + ex.Message);
                    Logging.LogError("API", "Update Order error", ex, $"Order ID {ID}");
                }
            }
            else
            {
                Logging.LogUserAction("API", "Create Order button clicked", "Attempting to create new order");

                if (string.IsNullOrWhiteSpace(textBox1.Text) || string.IsNullOrWhiteSpace(textBox2.Text) || string.IsNullOrWhiteSpace(textBox3.Text))
                {

                    MessageBox.Show("Please enter all the fields");
                    Logging.LogWarning("Validation", "Incomplete form data for create", "One or more fields are empty");
                    return;
                }
                try
                {
                    var order = new Order
                    {
                        
                        Item = textBox1.Text,
                        Quantity = int.Parse(textBox2.Text),
                        OrderDate = DateTime.Now,
                        TotalPrice = decimal.Parse(textBox3.Text)
                    };

                    Logging.LogUserAction("API", "Sending create request", $"Item: {order.Item}, Quantity: {order.Quantity}, Price: {order.TotalPrice}");

                    var response = await client.PostAsJsonAsync("order", order);

                    if (response.IsSuccessStatusCode)
                    {
                        dataGridView1.DataSource = await client.GetFromJsonAsync<List<Order>>("order");
                        Logging.LogUserAction("API", "Create Order successful", $"New order created: {order.Item}");
                        MessageBox.Show("Order created successfully!");
                    }
                    else
                    {
                        MessageBox.Show("Error creating order: " + response.ReasonPhrase);
                        Logging.LogWarning("API", "Create Order failed", $"Failed to create order: {response.ReasonPhrase}");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("An error occurred: " + ex.Message);
                    Logging.LogError("API", "Create Order error", ex, "Error creating new order");
                }
            }

        }

        private void Form2_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (int.TryParse(id, out int orderId))
            {
                Logging.LogUserAction("Navigation", "Form2 (Edit Order) closing", $"Edit order form closing for order ID: {orderId}");
            }
            else
            {
                Logging.LogUserAction("Navigation", "Form2 (Create Order) closing", "Create order form closing");
            }
        }

        private void Form2_Load(object sender, EventArgs e)
        {

        }
    }
}

