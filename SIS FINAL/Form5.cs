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
            LoadTimeDataFromDatabase();
            foreach (DataGridViewColumn column in dataGridView1.Columns)
            {
                column.SortMode = DataGridViewColumnSortMode.NotSortable;
            }
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

        private void button1_Click(object sender, EventArgs e)
        {
            string[] daysOfWeek = { "Mon", "Tue", "Wed", "Thu", "Fri" };

            // List to hold each row's data
            List<string> timeSlotsData = new List<string>();

            // Loop through each row in the DataGridView
            for (int rowIndex = 0; rowIndex < dataGridView1.Rows.Count; rowIndex++)
            {
                var row = dataGridView1.Rows[rowIndex];

                // Skip empty rows (just in case)
                if (row.Cells[0].Value == null || string.IsNullOrWhiteSpace(row.Cells[0].Value.ToString()))
                    continue;

                // Prepare a single row's data (subject PKs or 0)
                List<string> rowData = new List<string>();

                for (int colIndex = 1; colIndex <= daysOfWeek.Length; colIndex++)
                {
                    var cell = row.Cells[colIndex];
                    Color cellColor = cell.Style.BackColor;

                    if (cellColor != dataGridView1.DefaultCellStyle.BackColor)
                    {
                        // Get the subject PK by color
                        string hexCode = ColorTranslator.ToHtml(cellColor).ToUpper();
                        string subjectPK = GetSubjectPKByColor(hexCode);

                        // If found, store subject PK, else store 0
                        if (!string.IsNullOrEmpty(subjectPK) && int.TryParse(subjectPK, out int pk))
                        {
                            rowData.Add(pk.ToString());
                        }
                        else
                        {
                            rowData.Add("0");
                        }
                    }
                    else
                    {
                        rowData.Add("0");
                    }
                }

                // Store this row as a JSON-like string (e.g. [1,0,0,0,0])
                string rowDataString = $"[{string.Join(",", rowData)}]";
                timeSlotsData.Add(rowDataString);
            }

            // Combine all rows into one big array
            string finalTimeSlots = $"[{string.Join(",", timeSlotsData)}]";

            // Save to the grade-section table
            using (var conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                string updateQuery = @"
                    UPDATE `grade-section`
                    SET time = @TimeSlots
                    WHERE `grade-section_pk` = @GradeSectionPK";

                using (var cmd = new MySqlCommand(updateQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@TimeSlots", finalTimeSlots);
                    cmd.Parameters.AddWithValue("@GradeSectionPK", this.PK);

                    cmd.ExecuteNonQuery();
                    Close();
                }
            }
        }

        private string GetSubjectPKByColor(string hexCode)
        {
            using (var conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT subject_pk FROM subjects WHERE color = @ColorHex";
                using (var cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@ColorHex", hexCode);
                    var result = cmd.ExecuteScalar();
                    return result?.ToString() ?? string.Empty;
                }
            }
        }

        //Loading Time
        private void LoadTimeDataFromDatabase()
        {
            using (var conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                string query = @"
                    SELECT time
                    FROM `grade-section`
                    WHERE `grade-section_pk` = @GradeSectionPK";

                using (var cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@GradeSectionPK", this.PK);
                    var result = cmd.ExecuteScalar();
                    if (result != null)
                    {
                        string timeData = result.ToString();

                        // Parse the timeData manually
                        List<List<int>> parsedData = ParseTimeData(timeData);

                        // Fill the DataGridView
                        for (int rowIndex = 0; rowIndex < parsedData.Count && rowIndex < dataGridView1.Rows.Count; rowIndex++)
                        {
                            var row = dataGridView1.Rows[rowIndex];
                            var dayData = parsedData[rowIndex];

                            for (int dayIndex = 0; dayIndex < dayData.Count && dayIndex < 5; dayIndex++)
                            {
                                int subjectPK = dayData[dayIndex];

                                if (subjectPK != 0)
                                {
                                    string colorHex = GetColorHexBySubjectPK(subjectPK);
                                    if (!string.IsNullOrEmpty(colorHex))
                                    {
                                        Color cellColor = ColorTranslator.FromHtml(colorHex);
                                        row.Cells[dayIndex + 1].Style.BackColor = cellColor;
                                    }
                                }
                                else
                                {
                                    row.Cells[dayIndex + 1].Style.BackColor = dataGridView1.DefaultCellStyle.BackColor;
                                }
                            }
                        }
                    }
                }
            }
        }

        private List<List<int>> ParseTimeData(string timeData)
        {
            // Remove whitespace
            timeData = timeData.Replace(" ", "").Replace("\r", "").Replace("\n", "");

            // Remove outer brackets
            timeData = timeData.TrimStart('[').TrimEnd(']');

            var rows = new List<List<int>>();

            // Split rows
            string[] rowStrings = timeData.Split(new string[] { "],[" }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var rowStr in rowStrings)
            {
                string cleanRow = rowStr.Trim('[', ']');
                string[] values = cleanRow.Split(',');

                var intList = new List<int>();
                foreach (var value in values)
                {
                    if (int.TryParse(value, out int intValue))
                    {
                        intList.Add(intValue);
                    }
                    else
                    {
                        intList.Add(0); // default if parsing fails
                    }
                }

                rows.Add(intList);
            }

            return rows;
        }

        // Helper function to get the color from subject_pk
        private string GetColorHexBySubjectPK(int subjectPK)
        {
            using (var conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT color FROM subjects WHERE subject_pk = @SubjectPK";
                using (var cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@SubjectPK", subjectPK);
                    var result = cmd.ExecuteScalar();
                    return result?.ToString().ToUpper() ?? string.Empty;
                }
            }
        }
    }
}
