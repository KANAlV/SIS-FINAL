using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Google.Protobuf.WellKnownTypes;
using MySql.Data.MySqlClient;

namespace SIS_FINAL
{
    public partial class Form5 : Form
    {
        string connectionString = "server=localhost;database=sis_final;user=root;password=;";
        int PK;
        string hexCode = null;
        public Form5(int PK)
        {
            this.PK = PK;
            InitializeComponent();
            generateTable();
            secQuery();
            subjectQuery();
            dataGridView1.RowHeadersVisible = false;
            dataGridView1.MultiSelect = false;
            dataGridView2.RowHeadersVisible = false;
            dataGridView2.MultiSelect = false;
            dataGridView1.ClearSelection();
            dataGridView2.ClearSelection();
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
            {
                DataGridViewCell clickedCell = dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex];
                if (!string.IsNullOrEmpty(this.hexCode))
                {
                    if (this.hexCode != "eraser")
                    {
                        // Convert the hex code to a Color object
                        Color color = ColorTranslator.FromHtml(this.hexCode);

                        // Set the background color of the cell
                        clickedCell.Style.BackColor = color;
                    }
                    else
                    {
                        clickedCell.Style.BackColor = dataGridView1.DefaultCellStyle.BackColor;
                    }
                    
                }
            }
        }

        private void secQuery()
        {
            string query = $"SELECT * FROM `grade-section` WHERE `grade-section_pk` = {this.PK}";

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
                                label33.Text = reader["grade-sec"].ToString();
                                label34.Text = reader["section_name"].ToString();
                            }
                        }
                    }
                    catch (Exception ex)
                    {

                    }
                }
            }
        }
        private void subjectQuery()
        {
            dataGridView2.Rows.Clear();
            string selectQuery = @"SELECT * FROM subjects";

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                using (MySqlCommand selectCommand = new MySqlCommand(selectQuery, connection))
                {
                    using (MySqlDataReader reader = selectCommand.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string sn = reader["subject_name"].ToString();
                            string c = reader["color"].ToString(); // stored hex string like "#FF0000"
                            string sPK = reader["subject_pk"].ToString();

                            int rowIndex = dataGridView2.Rows.Add(sPK, "", sn);

                            if (!string.IsNullOrEmpty(c))
                            {
                                try
                                {
                                    Color cellColor = ColorTranslator.FromHtml(c);
                                    dataGridView2.Rows[rowIndex].Cells[1].Style.BackColor = cellColor;
                                }
                                catch
                                {
                                    // Handle invalid color formats gracefully
                                    dataGridView2.Rows[rowIndex].Cells[1].Style.BackColor = ColorTranslator.FromHtml("FFFFFF");
                                }
                            }
                        }
                    }
                }
            }
        }
        private void generateTable()
        {
            dataGridView1.Columns.Clear();
            dataGridView1.Rows.Clear();
            dataGridView1.Columns.Add("TimeSlot", "Time Slot");

            string[] daysOfWeek = { "Mon", "Tue", "Wed", "Thu", "Fri" };
            foreach (var day in daysOfWeek)
            {
                dataGridView1.Columns.Add(day, day);
            }

            // Generate time slots from 5 AM to 7 PM
            TimeSpan startTime = new TimeSpan(5, 0, 0);
            TimeSpan endTime = new TimeSpan(19, 0, 0);
            TimeSpan slotDuration = new TimeSpan(0, 30, 0); // 30-minute slots

            for (var time = startTime; time < endTime; time += slotDuration)
            {
                var start = DateTime.Today.Add(time).ToString("hh:mm tt");
                var end = DateTime.Today.Add(time + slotDuration).ToString("hh:mm tt");
                string timeSlot = $"{start} - {end}";

                int rowIndex = dataGridView1.Rows.Add();
                dataGridView1.Rows[rowIndex].Cells[0].Value = timeSlot;

                for (int col = 1; col <= daysOfWeek.Length; col++)
                {
                    dataGridView1.Rows[rowIndex].Cells[col].Value = "";
                }
            }

            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Form6 subs = new Form6();
            subs.ShowDialog();
            subjectQuery();
        }

        private void dataGridView2_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridView2.Rows[e.RowIndex];
                Color cellColor = row.Cells[1].Style.BackColor;
                this.hexCode = ColorTranslator.ToHtml(cellColor);
                label1.Text = row.Cells[2].Value.ToString();
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (label1.Text == "Eraser")
            {
                this.hexCode = null;
                label1.Text = "none";
            }
            else
            {
                this.hexCode = "eraser";
                label1.Text = "Eraser";
            }
        }
    }
}
