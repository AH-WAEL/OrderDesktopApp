namespace desktopAPI
{
    partial class ProductsForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            button7 = new Button();
            button6 = new Button();
            label1 = new Label();
            textBox1 = new TextBox();
            dataGridView1 = new DataGridView();
            button5 = new Button();
            button4 = new Button();
            button3 = new Button();
            button2 = new Button();
            button1 = new Button();
            ((System.ComponentModel.ISupportInitialize)dataGridView1).BeginInit();
            SuspendLayout();
            // 
            // button7
            // 
            button7.Location = new Point(634, 12);
            button7.Name = "button7";
            button7.Size = new Size(94, 29);
            button7.TabIndex = 19;
            button7.Text = "Logout";
            button7.UseVisualStyleBackColor = true;
            button7.Click += button7_Click;
            // 
            // button6
            // 
            button6.Location = new Point(580, 390);
            button6.Name = "button6";
            button6.Size = new Size(148, 48);
            button6.TabIndex = 18;
            button6.Text = "Live Feed";
            button6.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(58, 380);
            label1.Name = "label1";
            label1.Size = new Size(24, 20);
            label1.TabIndex = 17;
            label1.Text = "ID";
            // 
            // textBox1
            // 
            textBox1.Location = new Point(134, 377);
            textBox1.Name = "textBox1";
            textBox1.Size = new Size(125, 27);
            textBox1.TabIndex = 16;
            // 
            // dataGridView1
            // 
            dataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView1.Location = new Point(325, 54);
            dataGridView1.Name = "dataGridView1";
            dataGridView1.RowHeadersWidth = 51;
            dataGridView1.Size = new Size(452, 322);
            dataGridView1.TabIndex = 15;
            // 
            // button5
            // 
            button5.Location = new Point(23, 299);
            button5.Name = "button5";
            button5.Size = new Size(254, 38);
            button5.TabIndex = 14;
            button5.Text = "Delete Product";
            button5.UseVisualStyleBackColor = true;
            button5.Click += button5_Click;
            // 
            // button4
            // 
            button4.Location = new Point(23, 232);
            button4.Name = "button4";
            button4.Size = new Size(254, 41);
            button4.TabIndex = 13;
            button4.Text = "Update Product";
            button4.UseVisualStyleBackColor = true;
            button4.Click += button4_Click;
            // 
            // button3
            // 
            button3.Location = new Point(23, 161);
            button3.Name = "button3";
            button3.Size = new Size(254, 45);
            button3.TabIndex = 12;
            button3.Text = "Create new Product";
            button3.UseVisualStyleBackColor = true;
            button3.Click += button3_Click;
            // 
            // button2
            // 
            button2.Location = new Point(23, 98);
            button2.Name = "button2";
            button2.Size = new Size(254, 43);
            button2.TabIndex = 11;
            button2.Text = "Get Product by ID";
            button2.UseVisualStyleBackColor = true;
            button2.Click += button2_Click;
            // 
            // button1
            // 
            button1.Location = new Point(23, 30);
            button1.Name = "button1";
            button1.Size = new Size(254, 40);
            button1.TabIndex = 10;
            button1.Text = "Get all Products";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // ProductsForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(button7);
            Controls.Add(button6);
            Controls.Add(label1);
            Controls.Add(textBox1);
            Controls.Add(dataGridView1);
            Controls.Add(button5);
            Controls.Add(button4);
            Controls.Add(button3);
            Controls.Add(button2);
            Controls.Add(button1);
            Name = "ProductsForm";
            Text = "Products";
            Load += ProductsForm_Load;
            ((System.ComponentModel.ISupportInitialize)dataGridView1).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button button7;
        private Button button6;
        private Label label1;
        private TextBox textBox1;
        private DataGridView dataGridView1;
        private Button button5;
        private Button button4;
        private Button button3;
        private Button button2;
        private Button button1;
    }
}