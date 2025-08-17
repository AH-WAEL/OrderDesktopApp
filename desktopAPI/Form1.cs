using desktopAPI.Services;
using Microsoft.AspNetCore.SignalR.Client;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Web;


namespace desktopAPI
 {
    public partial class Form1 : Form
    {
        private HttpClient client1 = new HttpClient();
        private HttpClient client2 = new HttpClient();
        public Form1()
        {
            InitializeComponent();
            client1.BaseAddress = new Uri("http://localhost:8080/api/");
            client2.BaseAddress = new Uri("http://localhost:50000/api/");
            Logging.LogUserAction("Navigation", "Form1 (Main) opened", "Main application form initialized");
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Logging.LogUserAction("UI", "Form1 Load event triggered", "Main form loaded successfully");
        }

        private void Form1_Load_1(object sender, EventArgs e)
        {

        }
        private async void button1_Click(object sender, EventArgs e)
        {
            Logging.LogUserAction("API", "Get All Orders button clicked", "Fetching all orders from API");
            try
            {
                dataGridView1.DataSource = await client1.GetFromJsonAsync<List<Order>>("order");
                Logging.LogUserAction("API", "Get All Orders successful", "All orders loaded into DataGridView");
            }
            catch (Exception ex)
            {
                Logging.LogError("API", "Get All Orders failed", ex, "Error fetching all orders");
                MessageBox.Show($"Error fetching orders: {ex.Message}");
            }
        }
        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
            {
                var cellValue = dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;
                Logging.LogUserAction("UI", "DataGridView cell clicked", $"Row: {e.RowIndex}, Column: {e.ColumnIndex}, Value: {cellValue}");
            }
        }
        private async void button2_Click(object sender, EventArgs e)
        {
            Logging.LogUserAction("API", "Get Order by ID button clicked", $"Attempting to fetch order with ID: {textBox1.Text}");

            // Validate input: check if it's not empty and is a valid integer
            if (string.IsNullOrWhiteSpace(textBox1.Text) || !int.TryParse(textBox1.Text, out int id))
            {
                MessageBox.Show("Please enter a valid number.");
                Logging.LogWarning("Validation", "Invalid Order ID input", $"User entered invalid ID: {textBox1.Text}");
                return; // Exit the method if validation fails
            }

            try
            {
                // Make the API call
                var response = await client1.GetAsync($"order/{id}");

                // Check if the response indicates success
                if (response.IsSuccessStatusCode)
                {
                    // Deserialize the response and bind it to the DataGridView
                    var order = await response.Content.ReadFromJsonAsync<Order>();
                    if (order != null)
                    {
                        dataGridView1.DataSource = new List<Order> { order };
                        MessageBox.Show("done");
                        Logging.LogUserAction("API", "Get Order by ID successful", $"Order ID {id} found and displayed");
                    }
                    else
                    {
                        MessageBox.Show("Order not found.");
                        Logging.LogWarning("API", "Order not found", $"Order ID {id} returned null");
                    }
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    MessageBox.Show("Order not found.");
                    Logging.LogWarning("API", "Order not found", $"Order ID {id} not found (404)");
                }
                else
                {
                    MessageBox.Show($"Error: {response.StatusCode} - {response.ReasonPhrase}");
                    Logging.LogWarning("API", "Get Order API error", $"Status: {response.StatusCode}, Reason: {response.ReasonPhrase}");
                }
            }
            catch (HttpRequestException ex)
            {
                MessageBox.Show($"Error fetching data: {ex.Message}");
                Logging.LogError("API", "HTTP Request failed", ex, $"Error fetching order ID {id}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An unexpected error occurred: {ex.Message}");
                Logging.LogError("API", "Unexpected error in Get Order", ex, $"Order ID {id}");
            }
        }
        private void button3_Click(object sender, EventArgs e)
        {
            Logging.LogUserAction("Navigation", "Create New Order button clicked", "Opening Form2 for creating new order");
            Form2 f = new Form2(textBox1.Text);
            f.Show();
        }
        private void button4_Click(object sender, EventArgs e)
        {
            Logging.LogUserAction("Navigation", "Edit Order button clicked", $"Opening Form2 for editing order ID: {textBox1.Text}");

            if (string.IsNullOrWhiteSpace(textBox1.Text) || !int.TryParse(textBox1.Text, out int id))
            {
                MessageBox.Show("Please enter a valid number.");
                Logging.LogWarning("Validation", "Invalid Order ID for edit", $"User entered invalid ID: {textBox1.Text}");
                return; // Exit the method if validation fails
            }

            Form2 f = new Form2(textBox1.Text); // Pass the content of textBox1
            f.Show();
        }
        private async void button5_Click(object sender, EventArgs e)
        {
            Logging.LogUserAction("API", "Delete Order button clicked", $"Attempting to delete order ID: {textBox1.Text}");

            if (string.IsNullOrWhiteSpace(textBox1.Text) || !int.TryParse(textBox1.Text, out int id))
            {
                MessageBox.Show("Please enter a valid number.");
                Logging.LogWarning("Validation", "Invalid Order ID for delete", $"User entered invalid ID: {textBox1.Text}");
                return; // Exit the method if validation fails
            }

            try
            {
                int deleteId = int.Parse(textBox1.Text);
                var deleteResponse = await client1.DeleteAsync($"order/{deleteId}");
                if (deleteResponse.IsSuccessStatusCode)
                {
                    MessageBox.Show("Order deleted successfully.");
                    Logging.LogUserAction("API", "Delete Order successful", $"Order ID {deleteId} deleted successfully");
                    dataGridView1.DataSource = await client1.GetFromJsonAsync<List<Order>>("order");
                    Logging.LogUserAction("UI", "Orders list refreshed", "DataGridView updated after deletion");
                }
                else
                {
                    MessageBox.Show("Error deleting order: " + deleteResponse.ReasonPhrase);
                    Logging.LogWarning("API", "Delete Order failed", $"Failed to delete order ID {deleteId}: {deleteResponse.ReasonPhrase}");
                }
            }
            catch (Exception ex)
            {
                Logging.LogError("API", "Delete Order error", ex, $"Error deleting order ID {textBox1.Text}");
                MessageBox.Show($"Error deleting order: {ex.Message}");
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            Logging.LogUserAction("Navigation", "Live Stream button clicked", "Opening Form3 for SignalR live stream");
            Form3 f = new Form3();
            f.Show();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Logging.LogUserAction("Navigation", "Form1 (Main) closing", "Main application form closing");
        }

        private async void button7_Click(object sender, EventArgs e)
        {
            Logging.LogUserAction("Navigation", "Logout button clicked", "User initiated logout");
            await AuthApiService.LogoutAsync(this);
        }
    }
}
