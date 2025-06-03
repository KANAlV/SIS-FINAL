using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace SIS_FINAL
{
    public partial class Form2 : Form
    {
        string connectionString = "server=localhost;database=sis_final;user=root;password=;";
        public Form2()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            bool check = true;
            if (comboBox1.Text.Trim() == "" || comboBox1.Text == null) { check = false; }
            if (comboBox2.Text.Trim() == "" || comboBox2.Text == null) { check = false; }
            if (check == true)
            {
                int Adv_PK = 0;

                string insertQuery = @"INSERT INTO `grade-section`
                (`grade-sec`, `section_name`, `adviser_pk`)
                VALUES (
                    @gs,
                    @sn,
                    @adv_pk
                )";

                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    MySqlCommand command = new MySqlCommand(insertQuery, connection);
                    command.Parameters.AddWithValue("@gs", $"{comboBox1.Text}-{comboBox2.Text}");
                    command.Parameters.AddWithValue("@sn", textBox1.Text);
                    command.Parameters.AddWithValue("@adv_pk", Adv_PK);
                    try
                    {
                        connection.Open();
                        int rowsInserted = command.ExecuteNonQuery();
                        MessageBox.Show($"{rowsInserted} row(s) inserted.");
                        Console.WriteLine($"{rowsInserted} row(s) inserted.");
                        Debug.WriteLine($"{rowsInserted} row(s) inserted.");
                        Close();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error: " + ex.Message);
                        Debug.WriteLine("Error: " + ex.Message);
                    }
                }
            }
        }
    }
}
