namespace SIS_FINAL
{
    partial class Form6
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
            dataGridView1 = new DataGridView();
            subPk = new DataGridViewTextBoxColumn();
            Subject_name = new DataGridViewTextBoxColumn();
            button1 = new Button();
            textBox1 = new TextBox();
            button2 = new Button();
            panel1 = new Panel();
            label3 = new Label();
            textBox2 = new TextBox();
            label2 = new Label();
            panel2 = new Panel();
            button3 = new Button();
            label1 = new Label();
            colorDialog1 = new ColorDialog();
            ((System.ComponentModel.ISupportInitialize)dataGridView1).BeginInit();
            panel1.SuspendLayout();
            panel2.SuspendLayout();
            SuspendLayout();
            // 
            // dataGridView1
            // 
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.AllowUserToDeleteRows = false;
            dataGridView1.AllowUserToResizeColumns = false;
            dataGridView1.AllowUserToResizeRows = false;
            dataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView1.Columns.AddRange(new DataGridViewColumn[] { subPk, Subject_name });
            dataGridView1.Location = new Point(12, 41);
            dataGridView1.Name = "dataGridView1";
            dataGridView1.ReadOnly = true;
            dataGridView1.Size = new Size(154, 268);
            dataGridView1.TabIndex = 0;
            dataGridView1.CellClick += dataGridView1_CellClick;
            // 
            // subPk
            // 
            subPk.HeaderText = "PK";
            subPk.Name = "subPk";
            subPk.ReadOnly = true;
            subPk.Visible = false;
            // 
            // Subject_name
            // 
            Subject_name.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            Subject_name.HeaderText = "Subject";
            Subject_name.Name = "Subject_name";
            Subject_name.ReadOnly = true;
            // 
            // button1
            // 
            button1.Location = new Point(35, 12);
            button1.Name = "button1";
            button1.Size = new Size(101, 23);
            button1.TabIndex = 1;
            button1.Text = "Add Subject";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // textBox1
            // 
            textBox1.Location = new Point(12, 38);
            textBox1.Name = "textBox1";
            textBox1.Size = new Size(162, 23);
            textBox1.TabIndex = 2;
            // 
            // button2
            // 
            button2.Location = new Point(101, 271);
            button2.Name = "button2";
            button2.Size = new Size(73, 23);
            button2.TabIndex = 3;
            button2.Text = "Save";
            button2.UseVisualStyleBackColor = true;
            button2.Click += button2_Click;
            // 
            // panel1
            // 
            panel1.BackColor = SystemColors.Window;
            panel1.Controls.Add(label3);
            panel1.Controls.Add(textBox2);
            panel1.Controls.Add(label2);
            panel1.Controls.Add(panel2);
            panel1.Controls.Add(label1);
            panel1.Controls.Add(textBox1);
            panel1.Controls.Add(button2);
            panel1.Location = new Point(172, 12);
            panel1.Name = "panel1";
            panel1.Size = new Size(182, 297);
            panel1.TabIndex = 4;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(136, 8);
            label3.Name = "label3";
            label3.Size = new Size(38, 15);
            label3.TabIndex = 8;
            label3.Text = "label3";
            label3.Visible = false;
            // 
            // textBox2
            // 
            textBox2.Location = new Point(101, 86);
            textBox2.Name = "textBox2";
            textBox2.ReadOnly = true;
            textBox2.Size = new Size(73, 23);
            textBox2.TabIndex = 7;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label2.Location = new Point(12, 89);
            label2.Name = "label2";
            label2.Size = new Size(89, 15);
            label2.TabIndex = 6;
            label2.Text = "Subject Color:*";
            // 
            // panel2
            // 
            panel2.Controls.Add(button3);
            panel2.Location = new Point(40, 120);
            panel2.Name = "panel2";
            panel2.Size = new Size(100, 100);
            panel2.TabIndex = 5;
            // 
            // button3
            // 
            button3.Location = new Point(13, 38);
            button3.Name = "button3";
            button3.Size = new Size(75, 23);
            button3.TabIndex = 5;
            button3.Text = "Select";
            button3.UseVisualStyleBackColor = true;
            button3.Click += button3_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label1.Location = new Point(12, 20);
            label1.Name = "label1";
            label1.Size = new Size(93, 15);
            label1.TabIndex = 4;
            label1.Text = "Subject Name:*";
            // 
            // Form6
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(4, 108, 179);
            ClientSize = new Size(362, 321);
            Controls.Add(panel1);
            Controls.Add(button1);
            Controls.Add(dataGridView1);
            Name = "Form6";
            Text = "Form6";
            ((System.ComponentModel.ISupportInitialize)dataGridView1).EndInit();
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            panel2.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private DataGridView dataGridView1;
        private Button button1;
        private TextBox textBox1;
        private Button button2;
        private Panel panel1;
        private TextBox textBox2;
        private Label label2;
        private Panel panel2;
        private Label label1;
        private ColorDialog colorDialog1;
        private Label label3;
        private Button button3;
        private DataGridViewTextBoxColumn subPk;
        private DataGridViewTextBoxColumn Subject_name;
    }
}