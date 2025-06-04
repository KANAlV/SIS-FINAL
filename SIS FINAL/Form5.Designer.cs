namespace SIS_FINAL
{
    partial class Form5
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
            dataGridView2 = new DataGridView();
            subPK = new DataGridViewTextBoxColumn();
            Color = new DataGridViewTextBoxColumn();
            Subject = new DataGridViewTextBoxColumn();
            label34 = new Label();
            label35 = new Label();
            label33 = new Label();
            label32 = new Label();
            panel1 = new Panel();
            label1 = new Label();
            label2 = new Label();
            button2 = new Button();
            button1 = new Button();
            button3 = new Button();
            ((System.ComponentModel.ISupportInitialize)dataGridView1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)dataGridView2).BeginInit();
            panel1.SuspendLayout();
            SuspendLayout();
            // 
            // dataGridView1
            // 
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.AllowUserToDeleteRows = false;
            dataGridView1.AllowUserToResizeColumns = false;
            dataGridView1.AllowUserToResizeRows = false;
            dataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView1.Location = new Point(15, 62);
            dataGridView1.Name = "dataGridView1";
            dataGridView1.ReadOnly = true;
            dataGridView1.Size = new Size(583, 323);
            dataGridView1.TabIndex = 3;
            dataGridView1.CellClick += dataGridView1_CellClick;
            // 
            // dataGridView2
            // 
            dataGridView2.AllowUserToAddRows = false;
            dataGridView2.AllowUserToDeleteRows = false;
            dataGridView2.AllowUserToResizeColumns = false;
            dataGridView2.AllowUserToResizeRows = false;
            dataGridView2.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView2.Columns.AddRange(new DataGridViewColumn[] { subPK, Color, Subject });
            dataGridView2.Location = new Point(604, 62);
            dataGridView2.Name = "dataGridView2";
            dataGridView2.ReadOnly = true;
            dataGridView2.Size = new Size(185, 294);
            dataGridView2.TabIndex = 4;
            dataGridView2.CellClick += dataGridView2_CellClick;
            // 
            // subPK
            // 
            subPK.HeaderText = "PK";
            subPK.Name = "subPK";
            subPK.ReadOnly = true;
            subPK.Visible = false;
            // 
            // Color
            // 
            Color.HeaderText = "Color";
            Color.Name = "Color";
            Color.ReadOnly = true;
            Color.Width = 50;
            // 
            // Subject
            // 
            Subject.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            Subject.HeaderText = "Subject";
            Subject.Name = "Subject";
            Subject.ReadOnly = true;
            // 
            // label34
            // 
            label34.AutoSize = true;
            label34.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label34.Location = new Point(319, 10);
            label34.Name = "label34";
            label34.Size = new Size(50, 17);
            label34.TabIndex = 15;
            label34.Text = "label34";
            // 
            // label35
            // 
            label35.AutoSize = true;
            label35.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label35.Location = new Point(214, 10);
            label35.Name = "label35";
            label35.Size = new Size(97, 17);
            label35.TabIndex = 14;
            label35.Text = "Section Name:";
            // 
            // label33
            // 
            label33.AutoSize = true;
            label33.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label33.Location = new Point(120, 10);
            label33.Name = "label33";
            label33.Size = new Size(50, 17);
            label33.TabIndex = 0;
            label33.Text = "label33";
            // 
            // label32
            // 
            label32.AutoSize = true;
            label32.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label32.Location = new Point(15, 10);
            label32.Name = "label32";
            label32.Size = new Size(99, 17);
            label32.TabIndex = 1;
            label32.Text = "Grade/Section:";
            // 
            // panel1
            // 
            panel1.BackColor = SystemColors.Window;
            panel1.Controls.Add(button3);
            panel1.Controls.Add(label1);
            panel1.Controls.Add(label2);
            panel1.Controls.Add(button2);
            panel1.Controls.Add(dataGridView1);
            panel1.Controls.Add(label34);
            panel1.Controls.Add(dataGridView2);
            panel1.Controls.Add(label35);
            panel1.Controls.Add(label33);
            panel1.Controls.Add(label32);
            panel1.Location = new Point(-1, 12);
            panel1.Name = "panel1";
            panel1.Size = new Size(801, 397);
            panel1.TabIndex = 16;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label1.Location = new Point(133, 42);
            label1.Name = "label1";
            label1.Size = new Size(37, 17);
            label1.TabIndex = 43;
            label1.Text = "none";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label2.Location = new Point(15, 42);
            label2.Name = "label2";
            label2.Size = new Size(112, 17);
            label2.TabIndex = 42;
            label2.Text = "Selected Subject:";
            // 
            // button2
            // 
            button2.Location = new Point(648, 362);
            button2.Name = "button2";
            button2.Size = new Size(104, 23);
            button2.TabIndex = 41;
            button2.Text = "Edit Subjects";
            button2.UseVisualStyleBackColor = true;
            button2.Click += button2_Click;
            // 
            // button1
            // 
            button1.Location = new Point(713, 415);
            button1.Name = "button1";
            button1.Size = new Size(75, 23);
            button1.TabIndex = 40;
            button1.Text = "Update";
            button1.UseVisualStyleBackColor = true;
            // 
            // button3
            // 
            button3.Location = new Point(728, 33);
            button3.Name = "button3";
            button3.Size = new Size(61, 23);
            button3.TabIndex = 44;
            button3.Text = "Eraser";
            button3.UseVisualStyleBackColor = true;
            button3.Click += button3_Click;
            // 
            // Form5
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(button1);
            Controls.Add(panel1);
            Name = "Form5";
            Text = "Set Schedule";
            ((System.ComponentModel.ISupportInitialize)dataGridView1).EndInit();
            ((System.ComponentModel.ISupportInitialize)dataGridView2).EndInit();
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private DataGridView dataGridView1;
        private DataGridView dataGridView2;
        private Label label34;
        private Label label35;
        private Label label33;
        private Label label32;
        private Panel panel1;
        private Button button1;
        private Button button2;
        private DataGridViewTextBoxColumn subPK;
        private DataGridViewTextBoxColumn Color;
        private DataGridViewTextBoxColumn Subject;
        private Label label1;
        private Label label2;
        private Button button3;
    }
}