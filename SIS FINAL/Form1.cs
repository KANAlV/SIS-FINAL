using MySql.Data.MySqlClient;
using System.Windows.Forms;

namespace SIS_FINAL
{
    public partial class Form1 : Form
    {
        string connectionString = "server=localhost;database=sis_final;user=root;password=;";
        public Form1()
        {
            InitializeComponent();
            sectionQuery();
            dataGridView1.RowHeadersVisible = false;
            dataGridView2.RowHeadersVisible = false;
            dataGridView3.RowHeadersVisible = false;
            dataGridView4.RowHeadersVisible = false;
        }

        private void textBox1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text != "Search...")
            {
                textBox1.Clear();
                textBox1.ForeColor = Color.Black;
            }
        }
        private void textBox2_Click(object sender, EventArgs e)
        {
            if (textBox2.Text != "Search...")
            {
                textBox2.Clear();
                textBox2.ForeColor = Color.Black;
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void dataGridView3_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            // Simulated "database" values:
            Dictionary<string, string> timeRangesPerDay = new Dictionary<string, string>()
            {
                { "Mon", "'7:00-8:30','10:00-11:30'" },
                { "Tue", "'8:00-9:30','13:00-14:30'" },
                { "Wed", "'9:00-10:30'" },
                { "Thu", "'7:30-9:00','15:00-16:30'" },
                { "Fri", "'10:00-11:30','14:00-15:30'" }
            };

            Dictionary<string, string> colorsPerDay = new Dictionary<string, string>()
            {
                { "Mon", "'blue','red'" },
                { "Tue", "'green','orange'" },
                { "Wed", "'purple'" },
                { "Thu", "'yellow','pink'" },
                { "Fri", "'gray','cyan'" }
            };

            // Setup DataGridView2
            dataGridView4.Columns.Clear();
            dataGridView4.Rows.Clear();

            // First column: Time Slot labels
            dataGridView4.Columns.Add("TimeSlot", "Time Slot");

            // Monday to Friday columns
            string[] daysOfWeek = { "Mon", "Tue", "Wed", "Thu", "Fri" };
            foreach (var day in daysOfWeek)
            {
                dataGridView4.Columns.Add(day, day);
            }

            // Generate time slots from 5 AM to 7 PM
            TimeSpan startTime = new TimeSpan(5, 0, 0);
            TimeSpan endTime = new TimeSpan(19, 0, 0);
            TimeSpan slotDuration = new TimeSpan(0, 30, 0); // 30-minute slots

            for (var time = startTime; time < endTime; time += slotDuration)
            {
                string timeSlot = $"{DateTime.Today.Add(time):hh\\:mm tt}";
                int rowIndex = dataGridView4.Rows.Add();
                dataGridView4.Rows[rowIndex].Cells[0].Value = timeSlot;

                // For each day, parse time ranges and colors then apply them
                for (int col = 1; col <= daysOfWeek.Length; col++)
                {
                    string day = daysOfWeek[col - 1];
                    var cell = dataGridView4.Rows[rowIndex].Cells[col];

                    // Check if we have data for the day
                    if (timeRangesPerDay.ContainsKey(day))
                    {
                        // Parse time ranges
                        var timeRanges = timeRangesPerDay[day]
                            .Split(',')
                            .Select(s => s.Trim('\''))
                            .Select(s => s.Split('-'))
                            .Select(parts => new
                            {
                                Start = TimeSpan.Parse(parts[0]),
                                End = TimeSpan.Parse(parts[1])
                            })
                            .ToList();

                        // Parse colors
                        var colors = colorsPerDay[day]
                            .Split(',')
                            .Select(s => s.Trim('\''))
                            .ToList();

                        // Apply color if time slot overlaps with any range
                        for (int i = 0; i < timeRanges.Count; i++)
                        {
                            var range = timeRanges[i];
                            if (time < range.End && (time + slotDuration) > range.Start)
                            {
                                cell.Style.BackColor = Color.FromName(colors[i]);
                            }
                        }
                    }
                }
            }

            // Optional: adjust DataGridView styles
            dataGridView4.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Form2 addSection = new Form2();
            addSection.Show();
        }

        private void sectionQuery()
        {
            string searchText = textBox2.Text.Trim();

            string query;

            // Check if the search box is empty or contains "Search..."
            if (string.IsNullOrEmpty(searchText) || searchText.Equals("Search...", StringComparison.OrdinalIgnoreCase))
            {
                // Normal query (no WHERE)
                query = "SELECT * FROM `grade-section`";
            }
            else
            {
                // Search query with WHERE
                query = "SELECT * FROM `grade-section` WHERE `grade-sec` LIKE @searchText OR `section_name` LIKE @searchText";
            }

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    if (!string.IsNullOrEmpty(searchText) && !searchText.Equals("Search...", StringComparison.OrdinalIgnoreCase))
                    {
                        // Add parameter for the WHERE clause
                        command.Parameters.AddWithValue("@searchText", $"%{searchText}%");
                    }

                    try
                    {
                        connection.Open();
                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                string grdSecPK = reader["grade-section_pk"].ToString();
                                string grdSec = reader["grade-sec"].ToString();
                                string secName = reader["section_name"].ToString();

                                dataGridView3.Rows.Add(grdSecPK, grdSec, secName);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error: " + ex.Message);
                    }
                }
            }

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            sectionQuery();
        }
    }
}
