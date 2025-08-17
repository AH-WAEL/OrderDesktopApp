using System;
using System.ComponentModel;
using System.Net.Http.Json;
using System.Text.Json;
using System.Windows.Forms;
using Microsoft.AspNetCore.SignalR.Client;

namespace desktopAPI
{
    public partial class Form3 : Form
    {
        private HubConnection _hubConnection;
        private BindingList<Order> _messages = new BindingList<Order>();        public Form3()
        {
            InitializeComponent();
            Logging.LogUserAction("Navigation", "Form3 (Live Stream) opened", "SignalR live stream form initialized");

            // Initialize the DataGridView properly
            dataGridView1.AutoGenerateColumns = true;
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.AllowUserToDeleteRows = false;
            dataGridView1.ReadOnly = true;
            
            // Force handle creation to avoid timing issues
            if (!dataGridView1.IsHandleCreated)
            {
                var handle = dataGridView1.Handle; // This forces handle creation
            }
            
            // Set the data source after handle creation
            dataGridView1.DataSource = _messages;
            Logging.LogUserAction("UI", "DataGridView initialized", "DataGridView configured for live order display");
        }        private async void Form3_Load(object sender, EventArgs e)
        {
            try
            {
                Logging.LogUserAction("SignalR", "Attempting to connect to SignalR hub", "Starting connection to http://localhost:8544/orderHub");
                
                _hubConnection = new HubConnectionBuilder()
                    .WithUrl("http://localhost:8544/orderHub") // Use the correct port!
                    .WithAutomaticReconnect()
                    .Build(); 
                    
                _hubConnection.On<string>("ReceiveOrder", (message) =>
                {
                    try
                    {
                        Logging.LogUserAction("SignalR", "Message received from hub", $"Message length: {message?.Length ?? 0}");
                        
                        var orderMessage = JsonSerializer.Deserialize<Order>(message);
                        if (orderMessage != null)
                        {
                            Logging.LogUserAction("Data", "Order message processed", $"Order ID: {orderMessage.Id}, Item: {orderMessage.Item}");
                            
                            // Check if form is still valid and Invoke is needed                                
                            if (this.IsHandleCreated && !this.IsDisposed)
                            {
                                if (this.InvokeRequired)
                                {
                                    this.Invoke(new Action(() =>
                                    {
                                        AddOrderSafely(orderMessage);
                                    }));
                                }
                                else
                                {
                                    AddOrderSafely(orderMessage);
                                }
                            }
                        }
                        else
                        {
                            Logging.LogWarning("Data", "Order deserialization failed", "Received message could not be deserialized to order object");
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error processing SignalR message: {ex.Message}");
                        Logging.LogError("SignalR", "Processing received message", ex, "Error in SignalR message handler");
                    }
                });

                await _hubConnection.StartAsync();
                Console.WriteLine("SignalR connection established");
                MessageBox.Show("Connected to SignalR Hub");
                Logging.LogUserAction("SignalR", "Successfully connected to SignalR hub", "Connection established and ready to receive messages");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Could not connect to SignalR hub:\n" + ex.Message);
                Console.WriteLine("SignalR connection failed: " + ex.ToString());
                Logging.LogError("SignalR", "Failed to connect to hub", ex, "SignalR connection initialization failed");
            }
        }


        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }        private void AddOrderSafely(Order orderMessage)
        {
            try
            {
                Logging.LogUserAction("UI", "Adding order to DataGridView", $"Order ID: {orderMessage.Id}, Item: {orderMessage.Item}");
                
                // Check if DataGridView is ready and form is in valid state
                if (dataGridView1 != null && dataGridView1.IsHandleCreated && !dataGridView1.IsDisposed && !this.IsDisposed)
                {
                    // Show the message box first
                    MessageBox.Show($"SignalR Message Received: Order ID {orderMessage.Id}, Item: {orderMessage.Item}");

                    // Completely unbind the DataGridView to prevent binding issues
                    dataGridView1.DataSource = null;

                    // Add the order to the binding list while unbound
                    _messages.Add(orderMessage);

                    // Rebind with the updated list
                    dataGridView1.DataSource = _messages;
                    dataGridView1.Refresh();

                    Logging.LogUserAction("UI", "Order successfully added to grid", $"Total orders in grid: {_messages.Count}");
                    Console.WriteLine($"Order added successfully: ID {orderMessage.Id}");
                }
                else
                {
                    Console.WriteLine("DataGridView not ready, order not added");
                    Logging.LogWarning("UI", "DataGridView not ready", "Order could not be added - DataGridView in invalid state");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding order to grid: {ex.Message}");
                MessageBox.Show($"Error adding order: {ex.Message}");
                Logging.LogError("UI", "Adding order to grid", ex, $"Failed to add order ID: {orderMessage?.Id}");
            }
        }

        private void Form3_FormClosed(object sender, FormClosedEventArgs e)
        {
            Logging.LogUserAction("Navigation", "Form3 (Live Stream) closed", "SignalR live stream form closed by user");
            
            if (_hubConnection != null)
            {
                Logging.LogUserAction("SignalR", "Disconnecting from SignalR hub", "Cleaning up SignalR connection");
                try
                {
                    _hubConnection.DisposeAsync();
                }
                catch (Exception ex)
                {
                    Logging.LogError("SignalR", "Error disconnecting hub", ex, "Error during SignalR cleanup");
                }
            }
        }
    }
}
