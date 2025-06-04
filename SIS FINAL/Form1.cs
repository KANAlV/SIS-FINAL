using MySql.Data.MySqlClient;
using MySqlX.XDevAPI.Relational;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace SIS_FINAL
{
    public partial class Form1 : Form
    {
        string connectionString = "server=localhost;database=sis_final;user=root;password=;";
        int StuPage = 0;
        int gsPK = 0;
        public Form1()
        {
            InitializeComponent();
            studentQuery();
            sectionQuery();
            subjectQuery();
            dataGridView1.RowHeadersVisible = false;
            dataGridView1.MultiSelect = false;
            dataGridView2.RowHeadersVisible = false;
            dataGridView2.MultiSelect = false;
            dataGridView3.RowHeadersVisible = false;
            dataGridView3.MultiSelect = false;
            dataGridView4.RowHeadersVisible = false;
            dataGridView4.MultiSelect = false;
            dataGridView5.RowHeadersVisible = false;
            dataGridView5.MultiSelect = false;
        }

        private void textBox1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text == "Search...")
            {
                textBox1.Clear();
                textBox1.ForeColor = Color.Black;
            }
        }
        private void textBox2_Click(object sender, EventArgs e)
        {
            if (textBox2.Text == "Search...")
            {
                textBox2.Clear();
                textBox2.ForeColor = Color.Black;
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            this.StuPage = 0;
            studentQuery();
        }

        private void dataGridView3_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            dataGridView3.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridView3.Rows[e.RowIndex];
                var pkValue = row.Cells[0].Value;
                if (pkValue != null)
                {
                    string query = $"SELECT * FROM `grade-section` WHERE `grade-section_pk` = {pkValue}";

                    using (MySqlConnection connection = new MySqlConnection(connectionString))
                    {
                        using (MySqlCommand command = new MySqlCommand(query, connection))
                        {
                            try
                            {
                                connection.Open();
                                using (MySqlDataReader reader = command.ExecuteReader())
                                {
                                    if (reader.Read())
                                    {
                                        this.gsPK = int.Parse(pkValue.ToString());
                                        label33.Text = reader["grade-sec"].ToString();
                                        label34.Text = reader["section_name"].ToString();
                                        label33.Visible = true;
                                        label34.Visible = true;
                                        button6.Visible = true;
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
            addSection.ShowDialog();
        }

        private void sectionQuery()
        {
            string searchText = textBox2.Text.Trim();
            dataGridView3.Rows.Clear();
            int maxpage = pageCounter("grade-section");
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

                    }
                }
            }

        }
        private void studentQuery()
        {
            textBox3.Text = this.StuPage.ToString();
            int offset = 20 * this.StuPage;
            dataGridView1.Rows.Clear();
            int maxpage = pageCounter("students");
            string query;

            // Check if the search box is empty or contains "Search..."
            if (string.IsNullOrEmpty(textBox1.Text.Trim()) || textBox1.Text.Trim().Equals("Search...", StringComparison.OrdinalIgnoreCase))
            {
                // Normal query (no WHERE)
                query = @$"
                    SELECT 
                        students.student_pk, students.students_no, students.surname, students.first_name, students.gender, students.enrolled, students.dateAdded,
                        `grade-section`.`grade-sec`, `grade-section`.`section_name`
                    FROM students
                    LEFT JOIN `grade-section` ON students.`grade-sec_pk` = `grade-section`.`grade-section_pk`
                    ORDER BY students_no ASC
                    LIMIT 20 OFFSET {offset};
                ";
            }
            else
            {
                // Search query with WHERE
                query = @$"
                    SELECT 
                        students.student_pk, 
                        students.students_no, 
                        students.surname, 
                        students.first_name, 
                        students.gender, 
                        students.enrolled, 
                        students.dateAdded,
                        `grade-section`.`grade-sec`, 
                        `grade-section`.`section_name`
                    FROM students
                    LEFT JOIN `grade-section` 
                        ON students.`grade-sec_pk` = `grade-section`.`grade-section_pk`
                    WHERE students.students_no LIKE @searchText 
                        OR students.surname LIKE @searchText  
                        OR students.first_name LIKE @searchText
                    ORDER BY students.students_no ASC
                    LIMIT 20 OFFSET {offset};
                ";
            }

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        if (!string.IsNullOrEmpty(textBox1.Text.Trim()) && !textBox1.Text.Trim().Equals("Search...", StringComparison.OrdinalIgnoreCase))
                        {
                            // Add parameter for the WHERE clause
                            command.Parameters.AddWithValue("@searchText", $"%{textBox1.Text.Trim()}%");
                        }
                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            dataGridView1.Rows.Clear(); // Clear existing rows before adding new

                            while (reader.Read())
                            {
                                string stuPK = reader["student_pk"].ToString();
                                string stuNo = reader["students_no"].ToString();
                                string sur = reader["surname"].ToString();
                                string fn = reader["first_name"].ToString();
                                string gndr = reader["gender"].ToString();

                                // Grade-section fields may be null if no matching record
                                string grdSec = reader["grade-sec"] == DBNull.Value ? "N/a" : reader["grade-sec"].ToString();

                                string status = reader["enrolled"].ToString();
                                string added = reader["dateAdded"].ToString();

                                dataGridView1.Rows.Add(stuPK, status, stuNo, sur, fn, grdSec, added);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {

                }
            }
        }


        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            sectionQuery();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            sectionQuery();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Form3 enrol = new Form3();
            enrol.ShowDialog();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            studentQuery();
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];

                // Assuming PK is in the first column (index 0)
                var pkValue = row.Cells[0].Value;
                if (pkValue != null)
                {
                    string primaryKey = pkValue.ToString();
                    string query = @$"SELECT
                        students.student_pk,
                        students.students_no,
                        students.photo,
                        students.surname,
                        students.first_name,
                        students.middle_name,
                        students.suffix,
                        students.gender,
                        students.enrolled,
                        students.dateAdded,
                        `grade-section`.`grade-sec`,
                        `grade-section`.`section_name`,
                        guardian.surname AS guardian_surname,
                        guardian.first_name AS guardian_first_name,
                        guardian.middle_name AS guardian_middle_name,
                        guardian.suffix AS guardian_suffix,
                        guardian.gender AS guardian_gender,
                        guardian.relationship,
                        guardian.phone,
                        guardian.email
                    FROM students
                    LEFT JOIN `grade-section` ON students.`grade-sec_pk` = `grade-section`.`grade-section_pk`
                    LEFT JOIN guardian ON students.student_pk = guardian.students_pk
                    WHERE students.student_pk = {primaryKey}";

                    using (MySqlConnection connection = new MySqlConnection(connectionString))
                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        try
                        {
                            connection.Open();
                            using (MySqlDataReader reader = command.ExecuteReader())
                            {
                                if (reader.Read())
                                {
                                    // student data
                                    string stuPK = reader["student_pk"].ToString();
                                    label31.Text = reader["student_pk"].ToString();
                                    label24.Text = reader["students_no"].ToString();
                                    label4.Text = reader["surname"].ToString();
                                    label5.Text = reader["first_name"].ToString();
                                    label6.Text = reader["middle_name"].ToString();
                                    label8.Text = reader["suffix"].ToString();
                                    label10.Text = reader["gender"].ToString();
                                    label12.Text = reader["grade-sec"] == DBNull.Value ? "N/a" : reader["grade-sec"].ToString();
                                    label29.Text = reader["enrolled"].ToString();
                                    label30.Text = $"Added: {reader["dateAdded"].ToString()}";

                                    // guardian data
                                    label16.Text = reader["guardian_surname"].ToString();
                                    label17.Text = reader["guardian_first_name"].ToString();
                                    label18.Text = reader["guardian_middle_name"].ToString();
                                    label19.Text = reader["guardian_gender"].ToString();
                                    label21.Text = reader["relationship"].ToString();
                                    label25.Text = reader["phone"].ToString();
                                    label27.Text = reader["email"].ToString();

                                    // Show labels
                                    groupBox1.Visible = true;
                                    groupBox2.Visible = true;
                                    label30.Visible = true;
                                    button5.Visible = true;

                                    // === Image loading ===
                                    if (reader["photo"] != DBNull.Value)
                                    {
                                        byte[] photoBytes = (byte[])reader["photo"];
                                        using (var ms = new MemoryStream(photoBytes))
                                        {
                                            pictureBox1.Image = Image.FromStream(ms);
                                        }
                                    }
                                    else
                                    {
                                        pictureBox1.Image = null; // clear if no photo
                                    }
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
        }

        private void button5_Click(object sender, EventArgs e)
        {
            Form4 editStu = new Form4(label31.Text);
            editStu.ShowDialog();
        }

        private void button7_Click(object sender, EventArgs e)
        {

        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            this.StuPage++;
            studentQuery();
        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            this.StuPage = this.StuPage + 2;
            studentQuery();
        }

        private void linkLabel3_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            this.StuPage = this.StuPage + 3;
            studentQuery();
        }

        private void linkLabel4_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            this.StuPage--;
            studentQuery();
        }

        private void linkLabel5_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            this.StuPage = this.StuPage - 2;
            studentQuery();
        }

        private void linkLabel6_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            this.StuPage = this.StuPage - 3;
            studentQuery();
        }

        private int pageCounter(string table)
        {
            string countQuery = null;

            switch (table)
            {
                case "students":
                    if (string.IsNullOrEmpty(textBox1.Text.Trim()) || textBox1.Text.Trim().Equals("Search...", StringComparison.OrdinalIgnoreCase))
                    {
                        countQuery = @"SELECT COUNT(*) FROM students;";
                    }
                    else
                    {
                        countQuery = @"
                            SELECT COUNT(*) 
                            FROM students
                            WHERE students.students_no LIKE @searchText
                                OR students.surname LIKE @searchText
                                OR students.first_name LIKE @searchText;
                        ";
                    }
                    break;

                case "grade-section":
                    if (string.IsNullOrEmpty(textBox2.Text.Trim()) || textBox2.Text.Trim().Equals("Search...", StringComparison.OrdinalIgnoreCase))
                    {
                        countQuery = @"SELECT COUNT(*) FROM `grade-section`;";
                    }
                    else
                    {
                        countQuery = @"
                            SELECT COUNT(*)
                            FROM `grade-section`
                            WHERE `grade-sec` LIKE @searchText
                                OR `section_name` LIKE @searchText;
                        ";
                    }
                    break;

                default:
                    throw new ArgumentException($"Unknown table: {table}");
            }

            int totalRows = 0;
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            using (MySqlCommand command = new MySqlCommand(countQuery, connection))
            {
                // Add parameter if needed
                if (countQuery.Contains("@searchText"))
                {
                    string searchText = "";
                    if (table == "students")
                        searchText = textBox1.Text.Trim();
                    else if (table == "grade-section")
                        searchText = textBox2.Text.Trim();

                    command.Parameters.AddWithValue("@searchText", $"%{searchText}%");
                }

                connection.Open();
                totalRows = Convert.ToInt32(command.ExecuteScalar());
            }

            int pageSize = 20;
            int totalPages = (int)Math.Ceiling((double)totalRows / pageSize);
            //MessageBox.Show(totalPages.ToString());
            return totalPages;
        }

        private void button6_Click(object sender, EventArgs e)
        {
            Form5 setSched = new Form5(gsPK);
            setSched.ShowDialog();
        }
        private void subjectQuery()
        {
            dataGridView5.Rows.Clear();
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

                            int rowIndex = dataGridView5.Rows.Add(sPK, "", sn);

                            if (!string.IsNullOrEmpty(c))
                            {
                                try
                                {
                                    Color cellColor = ColorTranslator.FromHtml(c);
                                    dataGridView5.Rows[rowIndex].Cells[1].Style.BackColor = cellColor;
                                }
                                catch
                                {
                                    // Handle invalid color formats gracefully
                                    dataGridView5.Rows[rowIndex].Cells[1].Style.BackColor = ColorTranslator.FromHtml("FFFFFF");
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
