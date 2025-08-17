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
    public partial class CreateProduct : Form
    {
        private string id;
        private readonly HttpClient client;
        public CreateProduct(string ID)
        {
            var clientHandler = new HttpClientHandler();

            var authDelegateHandler = new AuthDelegateHandler(this)
            {
                InnerHandler = clientHandler,
            };

            client = new HttpClient(authDelegateHandler);

            client.BaseAddress = new Uri("https://localhost:7043/api/");

            InitializeComponent();
            id = ID;
            if (int.TryParse(ID, out int productId))
            {
                Logging.LogUserAction("Navigation", "CreateProduct Form opened", $"Editing product ID: {productId}");
            }
            else
            {
                Logging.LogUserAction("Navigation", "CreateProduct Form opened", "Creating new product");
            }
        }
        private void label1_Click(object sender, EventArgs e)
        {

        }

        private async void button1_Click(object sender, EventArgs e)
        {
            if (int.TryParse(id, out int ID))
            {
                Logging.LogUserAction("API", "Update Product button clicked", $"Attempting to update product ID: {ID}");

                if (string.IsNullOrWhiteSpace(textBox1.Text) || string.IsNullOrWhiteSpace(textBox2.Text) || string.IsNullOrWhiteSpace(textBox3.Text))
                {
                    MessageBox.Show("Please enter all the fields");
                    Logging.LogWarning("Validation", "Incomplete form data for update", "One or more fields are empty");
                    return;
                }
                try
                {

                    var Product = new Product
                    {
                        Id = ID,
                        Name = textBox1.Text,
                        Description = textBox2.Text,
                        Price = decimal.Parse(textBox3.Text)
                    };

                    Logging.LogUserAction("API", "Sending update request", $"Product ID: {ID}, Name: {Product.Name}, Description: {Product.Description}, Price: {Product.Price}");

                    var updateResponse = await client.PutAsJsonAsync($"product/{ID}", Product);
                    if (updateResponse.IsSuccessStatusCode)
                    {
                        dataGridView1.DataSource = await client.GetFromJsonAsync<List<Product>>("product");
                        Logging.LogUserAction("API", "Update Product successful", $"Product ID {ID} updated successfully");
                        MessageBox.Show("Product updated successfully!");
                    }
                    else
                    {
                        MessageBox.Show("Error updating product: " + updateResponse.ReasonPhrase);
                        Logging.LogWarning("API", "Update Product failed", $"Product ID {ID}: {updateResponse.ReasonPhrase}");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("An error occurred: " + ex.Message);
                    Logging.LogError("API", "Update Product error", ex, $"Product ID {ID}");
                }
            }
            else
            {
                Logging.LogUserAction("API", "Create Product button clicked", "Attempting to create new product");

                if (string.IsNullOrWhiteSpace(textBox1.Text) || string.IsNullOrWhiteSpace(textBox2.Text) || string.IsNullOrWhiteSpace(textBox3.Text))
                {

                    MessageBox.Show("Please enter all the fields");
                    Logging.LogWarning("Validation", "Incomplete form data for create", "One or more fields are empty");
                    return;
                }
                try
                {
                    var Product = new Product
                    {
                        Name = textBox1.Text,
                        Description = textBox2.Text,
                        Price = decimal.Parse(textBox3.Text)
                    };


                    Logging.LogUserAction("API", "Sending create request", $"Item: {Product.Name}, Description: {Product.Description}, Price: {Product.Price}");

                    var response = await client.PostAsJsonAsync("product", Product);

                    if (response.IsSuccessStatusCode)
                    {
                        dataGridView1.DataSource = await client.GetFromJsonAsync<List<Product>>("product");
                        Logging.LogUserAction("API", "Create Product successful", $"New product created: {Product.Name}");
                        MessageBox.Show("Product created successfully!");
                    }
                    else
                    {
                        MessageBox.Show("Error creating product: " + response.ReasonPhrase);
                        Logging.LogWarning("API", "Create Product failed", $"Failed to create product: {response.ReasonPhrase}");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("An error occurred: " + ex.Message);
                    Logging.LogError("API", "Create Product error", ex, "Error creating new product");
                }
            }

        }

        private void CreateProduct_Load(object sender, EventArgs e)
        {

        }
    }
}