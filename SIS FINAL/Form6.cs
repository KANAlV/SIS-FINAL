using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using MySqlX.XDevAPI.Relational;
using Org.BouncyCastle.Utilities.Encoders;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace SIS_FINAL
{
    public partial class Form6 : Form
    {
        string connectionString = "server=localhost;database=sis_final;user=root;password=;";
        public Form6()
        {
            InitializeComponent();
            subjectQuery();
            dataGridView1.RowHeadersVisible = false;
            dataGridView1.MultiSelect = false;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (colorDialog1.ShowDialog() == DialogResult.OK)
            {
                Color selectedColor = colorDialog1.Color;
                string hexColor = $"#{selectedColor.R:X2}{selectedColor.G:X2}{selectedColor.B:X2}";
                textBox2.Text = hexColor;
                panel2.BackColor = ColorTranslator.FromHtml(hexColor);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int totalRows = 0;
            string countQuery = @"
                SELECT COUNT(*) 
                FROM subjects 
                WHERE subject_name LIKE '%Subject%';
            ";

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                // Get count
                using (MySqlCommand countCommand = new MySqlCommand(countQuery, connection))
                {
                    totalRows = Convert.ToInt32(countCommand.ExecuteScalar());
                }

                // Insert new subject
                string insertQuery = @"
                    INSERT INTO subjects (subject_name)
                    VALUES (@sn);
                    SELECT LAST_INSERT_ID();
                ";

                using (MySqlCommand insertCommand = new MySqlCommand(insertQuery, connection))
                {
                    string subjectName = $"Subject {++totalRows}";
                    insertCommand.Parameters.AddWithValue("@sn", subjectName);

                    try
                    {
                        // Execute insert and get the inserted ID
                        long newId = Convert.ToInt64(insertCommand.ExecuteScalar());
                        Console.WriteLine($"Inserted Subject ID: {newId}");
                        Debug.WriteLine($"Inserted Subject ID: {newId}");

                        // Retrieve the newly inserted row
                        string selectQuery = @"
                            SELECT * 
                            FROM subjects 
                            WHERE subject_pk = @id;
                        ";

                        using (MySqlCommand selectCommand = new MySqlCommand(selectQuery, connection))
                        {
                            selectCommand.Parameters.AddWithValue("@id", newId);

                            using (MySqlDataReader reader = selectCommand.ExecuteReader())
                            {
                                if (reader.Read())
                                {
                                    textBox1.Text = reader["subject_name"].ToString();
                                    textBox2.Text = reader["color"].ToString();
                                    label3.Text = newId.ToString();
                                }
                            }
                        }

                        Close();
                        subjectQuery();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error: " + ex.Message);
                        Debug.WriteLine("Error: " + ex.Message);
                    }
                }
            }
        }
        private void subjectQuery()
        {
            dataGridView1.Rows.Clear();
            string selectQuery = @"SELECT * FROM subjects";
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open(); // Add this line!

                using (MySqlCommand selectCommand = new MySqlCommand(selectQuery, connection))
                {
                    using (MySqlDataReader reader = selectCommand.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string sn = reader["subject_name"].ToString();
                            string c = reader["color"].ToString();
                            string sPK = reader["subject_pk"].ToString();

                            dataGridView1.Rows.Add(sPK, sn);
                        }
                    }
                }
            }
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];
                var pkValue = row.Cells[0].Value;
                if (pkValue != null)
                {
                    string query = $"SELECT * FROM subjects WHERE subject_pk = {pkValue}";

                    using (MySqlConnection connection = new MySqlConnection(connectionString))
                    {
                        using (MySqlCommand command = new MySqlCommand(query, connection))
                        {
                            try
                            {
                                connection.Open();
                                using (MySqlDataReader reader = command.ExecuteReader())
                                {
                                    while (reader.Read())
                                    {
                                        textBox1.Text = reader["subject_name"].ToString();
                                        textBox2.Text = reader["color"].ToString();
                                        panel2.BackColor = System.Drawing.ColorTranslator.FromHtml(reader["color"].ToString());
                                        label3.Text = reader["subject_pk"].ToString();

                                    }
                                }
                            }
                            catch (Exception ex)
                            {

                            }
                        }
                    }
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            bool check = true;
            if (textBox1.Text.Trim() == "" || textBox1.Text == null) { check = false; }
            if (textBox2.Text.Trim() == "" || textBox2.Text == null) { check = false; }
            if (check == true)
            {
                string updateStudentQuery = @"
                    UPDATE subjects
                    SET
                        subject_name = @subject_name,
                        color = @color
                    WHERE subject_pk = @subject_pk;
                ";

                using (MySqlConnection connection = new MySqlConnection(connectionString))
                using (MySqlCommand command = new MySqlCommand(updateStudentQuery, connection))
                {
                    command.Parameters.AddWithValue("@subject_name", textBox1.Text);
                    command.Parameters.AddWithValue("@color", textBox2.Text);
                    command.Parameters.AddWithValue("@subject_pk", label3.Text);
                    try
                    {
                        connection.Open();
                        int rowsUpdated = command.ExecuteNonQuery();

                        MessageBox.Show($"{rowsUpdated} subjects record updated.");
                        subjectQuery();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error updating subjects: " + ex.Message);
                    }
                }
            }
            else { MessageBox.Show("Not all required fields are filled"); }
        }
    }
}
