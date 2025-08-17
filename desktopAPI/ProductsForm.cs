using desktopAPI.Services;
using Newtonsoft.Json.Linq;
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
    public partial class ProductsForm : Form
    {
        private readonly HttpClient client;
        public ProductsForm()
        {

            InitializeComponent();

            var clientHandler = new HttpClientHandler();

            var authDelegateHandler = new AuthDelegateHandler(this)
            {
                InnerHandler = clientHandler,
            };

            client = new HttpClient(authDelegateHandler);

            client.BaseAddress = new Uri("https://localhost:7043/api/");
            Logging.LogUserAction("Navigation", "Products Form opened", "Products form initialized");
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            Logging.LogUserAction("API", "Get All Products button clicked", "Fetching all Products from API");
            try
            {
                if (string.IsNullOrEmpty(AuthApiService.AccessToken))
                {
                    MessageBox.Show("You need to be logged in to access products.", "Authentication Required",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                dataGridView1.DataSource = await client.GetFromJsonAsync<List<Product>>("Product");
                Logging.LogUserAction("API", "Get All products successful", "All products loaded into DataGridView");
            }
            catch (Exception ex)
            {
                Logging.LogError("API", "Get All Products failed", ex, "Error fetching all products");
                MessageBox.Show($"Error fetching products: {ex.Message}");
            }
        }

        private async void button2_Click(object sender, EventArgs e)
        {
            Logging.LogUserAction("API", "Get Product by ID button clicked", $"Attempting to fetch Product with ID: {textBox1.Text}");

            if (string.IsNullOrWhiteSpace(textBox1.Text) || !int.TryParse(textBox1.Text, out int id))
            {
                MessageBox.Show("Please enter a valid number.");
                Logging.LogWarning("Validation", "Invalid Order ID input", $"User entered invalid ID: {textBox1.Text}");
                return; // Exit the method if validation fails
            }

            try
            {
                var response = await client.GetAsync($"Product/{id}");

                // Check if the response indicates success
                if (response.IsSuccessStatusCode)
                {
                    // Deserialize the response and bind it to the DataGridView
                    var product = await response.Content.ReadFromJsonAsync<Product>();
                    if (product != null)
                    {
                        dataGridView1.DataSource = new List<Product> { product };
                        Logging.LogUserAction("API", "Get Product by ID successful", $"Product ID {id} found and displayed");
                    }
                    else
                    {
                        MessageBox.Show("Product not found.");
                        Logging.LogWarning("API", "Product not found", $"Product ID {id} returned null");
                    }
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    MessageBox.Show("Product not found.");
                    Logging.LogWarning("API", "Product not found", $"Product ID {id} not found (404)");
                }
                else
                {
                    MessageBox.Show($"Error: {response.StatusCode} - {response.ReasonPhrase}");
                    Logging.LogWarning("API", "Get Product API error", $"Status: {response.StatusCode}, Reason: {response.ReasonPhrase}");
                }
            }
            catch (HttpRequestException ex)
            {
                MessageBox.Show($"Error fetching data: {ex.Message}");
                Logging.LogError("API", "HTTP Request failed", ex, $"Error fetching product ID {id}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An unexpected error occurred: {ex.Message}");
                Logging.LogError("API", "Unexpected error in Get Product", ex, $"Product ID {id}");
            }

        }

        private void button3_Click(object sender, EventArgs e)
        {
            Logging.LogUserAction("Navigation", "Create New Order button clicked", "Opening Form2 for creating new order");
            CreateProduct f = new CreateProduct(textBox1.Text);
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

            CreateProduct f = new CreateProduct(textBox1.Text); // Pass the content of textBox1
            f.Show();
        }

        private async void button5_Click(object sender, EventArgs e)
        {
            Logging.LogUserAction("API", "Delete Product button clicked", $"Attempting to delete product ID: {textBox1.Text}");

            if (string.IsNullOrWhiteSpace(textBox1.Text) || !int.TryParse(textBox1.Text, out int id))
            {
                MessageBox.Show("Please enter a valid number.");
                Logging.LogWarning("Validation", "Invalid Product ID for delete", $"User entered invalid ID: {textBox1.Text}");
                return; // Exit the method if validation fails
            }

            try
            {
                int deleteId = int.Parse(textBox1.Text);
                var deleteResponse = await client.DeleteAsync($"product/{deleteId}");
                if (deleteResponse.IsSuccessStatusCode)
                {
                    MessageBox.Show("Product deleted successfully.");
                    Logging.LogUserAction("API", "Delete Product successful", $"Product ID {deleteId} deleted successfully");
                    dataGridView1.DataSource = await client.GetFromJsonAsync<List<Product>>("product");
                    Logging.LogUserAction("UI", "Products list refreshed", "DataGridView updated after deletion");
                }
                else
                {
                    MessageBox.Show("Error deleting product: " + deleteResponse.ReasonPhrase);
                    Logging.LogWarning("API", "Delete Product failed", $"Failed to delete product ID {deleteId}: {deleteResponse.ReasonPhrase}");
                }
            }
            catch (Exception ex)
            {
                Logging.LogError("API", "Delete Product error", ex, $"Error deleting product ID {textBox1.Text}");
                MessageBox.Show($"Error deleting product: {ex.Message}");
            }
        }

        private void ProductsForm_Load(object sender, EventArgs e)
        {

        }

        private async void button7_Click(object sender, EventArgs e)
        {
            Logging.LogUserAction("Navigation", "Logout button clicked", "User initiated logout");
            await AuthApiService.LogoutAsync(this);
        }
    }
}
