using MySql.Data.MySqlClient;
using Mysqlx.Resultset;
using MySqlX.XDevAPI.Relational;
using System.Data;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using static System.Runtime.InteropServices.JavaScript.JSType;
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
            sectionComboQuery();
            studentQuery();
            sectionQuery();
            stuQueryGrd();
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
            dataGridView6.RowHeadersVisible = false;
            dataGridView6.MultiSelect = false;
            dataGridView7.RowHeadersVisible = false;
            dataGridView7.MultiSelect = false;
            dataGridView8.RowHeadersVisible = false;
            dataGridView8.MultiSelect = false;
            dataGridView9.RowHeadersVisible = false;
            dataGridView9.MultiSelect = false;
            dataGridView10.RowHeadersVisible = false;
            dataGridView10.MultiSelect = false;
            dataGridView11.RowHeadersVisible = false;
            dataGridView11.MultiSelect = false;
            dataGridView12.RowHeadersVisible = false;
            dataGridView12.MultiSelect = false;
            dataGridView13.RowHeadersVisible = false;
            dataGridView13.MultiSelect = false;
            dataGridView14.RowHeadersVisible = false;
            dataGridView14.MultiSelect = false;
            dataGridView15.RowHeadersVisible = false;
            dataGridView15.MultiSelect = false;
            dataGridView16.RowHeadersVisible = false;
            dataGridView16.MultiSelect = false;
            dataGridView17.RowHeadersVisible = false;
            dataGridView17.MultiSelect = false;
            dataGridView18.RowHeadersVisible = false;
            dataGridView18.MultiSelect = false;
            dataGridView19.RowHeadersVisible = false;
            dataGridView19.MultiSelect = false;
            dataGridView20.RowHeadersVisible = false;
            dataGridView20.MultiSelect = false;
            dataGridView21.RowHeadersVisible = false;
            dataGridView21.MultiSelect = false;
            foreach (DataGridViewColumn column in dataGridView2.Columns)
            {
                column.SortMode = DataGridViewColumnSortMode.NotSortable;
            }
            foreach (DataGridViewColumn column in dataGridView4.Columns)
            {
                column.SortMode = DataGridViewColumnSortMode.NotSortable;
            }
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
                                        generateTable(dataGridView4);
                                        LoadTimeDataFromDatabase(pkValue.ToString(), dataGridView4);
                                        subjectQuery(dataGridView5);
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

        private void generateTable(DataGridView DGV)
        {
            DGV.Columns.Clear();
            DGV.Rows.Clear();
            DGV.Columns.Add("TimeSlot", "Time Slot");

            string[] daysOfWeek = { "Mon", "Tue", "Wed", "Thu", "Fri" };
            foreach (var day in daysOfWeek)
            {
                DGV.Columns.Add(day, day);
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

                int rowIndex = DGV.Rows.Add();
                DGV.Rows[rowIndex].Cells[0].Value = timeSlot;

                for (int col = 1; col <= daysOfWeek.Length; col++)
                {
                    DGV.Rows[rowIndex].Cells[col].Value = "";
                }
            }

            DGV.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
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
            dataGridView3.ClearSelection();
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
                    dataGridView1.ClearSelection();
                }
                catch (Exception ex)
                {

                }
            }
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            stuQueryGrd();
        }

        private void button7_Click_1(object sender, EventArgs e)
        {
            stuQueryGrd();
        }

        private void stuQueryGrd()
        {
            dataGridView7.Rows.Clear();
            subjectQuery(dataGridView9, comboBox2.Text);
            string query;

            // Check if the search box is empty or contains "Search..."
            if (string.IsNullOrEmpty(textBox4.Text.Trim()) || textBox4.Text.Trim().Equals("Search...", StringComparison.OrdinalIgnoreCase))
            {
                // Normal query
                query = @$"
                    SELECT student_pk, students_no, surname, first_name FROM students WHERE `grade-sec_pk` = @spk;
                ";
            }
            else
            {
                // Search query
                query = @$"
                    SELECT student_pk, students_no, surname, first_name FROM students
                    WHERE `grade-sec_pk` = @spk
                        AND (students_no LIKE @searchText OR
                        surname LIKE @searchText OR 
                        first_name LIKE @searchText);
                ";
            }

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        if (!string.IsNullOrEmpty(textBox4.Text.Trim()) && !textBox4.Text.Trim().Equals("Search...", StringComparison.OrdinalIgnoreCase))
                        {
                            // Add parameter for the WHERE clause
                            command.Parameters.AddWithValue("@searchText", $"%{textBox4.Text.Trim()}%");
                        }
                        command.Parameters.AddWithValue("@spk", comboBox2.SelectedValue.ToString());
                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            dataGridView7.Rows.Clear(); // Clear existing rows before adding new

                            while (reader.Read())
                            {
                                string stuPK = reader["student_pk"].ToString();
                                string stuNo = reader["students_no"].ToString();
                                string sur = reader["surname"].ToString();
                                string fn = reader["first_name"].ToString();

                                dataGridView7.Rows.Add(stuPK, stuNo, sur, fn);
                            }
                        }
                    }
                    dataGridView7.ClearSelection();
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
                        students.`grade-sec_pk`,
                        `grade-section`.`grade-sec`,
                        `grade-section`.`section_name`,
                        `grade-section`.`subject_pk`,
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
                                    string subject_pk = reader["subject_pk"].ToString();
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

                                    //Sched
                                    generateTable(dataGridView2);
                                    LoadTimeDataFromDatabase(reader["grade-sec_pk"].ToString(), dataGridView2);
                                    subjectQuery(dataGridView6);
                                    dataGridView2.ClearSelection();

                                    //Grade
                                    dataGridView21.Rows.Clear();
                                    string[] dataArray = subject_pk.Split(',');
                                    foreach (string x in dataArray)
                                    {
                                        gradeSummary(stuPK, x, reader["grade-sec_pk"].ToString());
                                    }
                                    dataGridView21.ClearSelection();

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

        private void gradeSummary(string stuPk, string subPk, string gsG)
        {
            string selectGrdQuery = @$"
                SELECT grades.*,
                    subjects.subject_name
                FROM grades
                LEFT JOIN subjects 
                    ON grades.subject_pk = subjects.subject_pk
                WHERE grades.student_pk = @stuPk
                    AND grades.subject_pk = @subPk
                    AND grades.gs_pk = @gsG
            ";

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    using (MySqlCommand selectGrdCommand = new MySqlCommand(selectGrdQuery, connection))
                    {
                        selectGrdCommand.Parameters.AddWithValue("@stuPk", stuPk);
                        selectGrdCommand.Parameters.AddWithValue("@subPk", subPk);
                        selectGrdCommand.Parameters.AddWithValue("@gsG", gsG);
                        using (MySqlDataReader readerGrd = selectGrdCommand.ExecuteReader())
                        {
                            if (readerGrd.Read())
                            {
                                // Parse weights from the reader
                                string subName = readerGrd["subject_name"].ToString();
                                double.TryParse(readerGrd["WW"]?.ToString(), out double ww);
                                double.TryParse(readerGrd["PT"]?.ToString(), out double pt);
                                double.TryParse(readerGrd["QE"]?.ToString(), out double qe);
                                ww /= 100;
                                pt /= 100;
                                qe /= 100;

                                textBox5.Text = readerGrd["WW"].ToString();
                                textBox6.Text = readerGrd["PT"].ToString();
                                textBox7.Text = readerGrd["QE"].ToString();

                                // List of columns and their associated controls
                                var columns = new (string Column, DataGridView DGV)[]
                                {
                                    ("WW_1", dataGridView8),
                                    ("PT_1", dataGridView10),
                                    ("QE_1", dataGridView11),
                                    ("WW_2", dataGridView14),
                                    ("PT_2", dataGridView13),
                                    ("QE_2", dataGridView12),
                                    ("WW_3", dataGridView17),
                                    ("PT_3", dataGridView16),
                                    ("QE_3", dataGridView15),
                                    ("WW_4", dataGridView20),
                                    ("PT_4", dataGridView19),
                                    ("QE_4", dataGridView18)
                                };

                                // Store intermediate results for weighted totals
                                double[] wwGrades = new double[4];
                                double[] ptGrades = new double[4];
                                double[] qeGrades = new double[4];

                                foreach (var (column, dgv) in columns)
                                {
                                    string cellData = readerGrd[column]?.ToString();

                                    if (!string.IsNullOrWhiteSpace(cellData))
                                    {
                                        loadCell(cellData, dgv);

                                        double grade = Math.Round(computeGrade(cellData), 2);

                                        // Determine which index to store
                                        int index = int.Parse(column.Substring(3)) - 1;

                                        if (column.StartsWith("WW_"))
                                        {
                                            wwGrades[index] = grade;
                                        }
                                        else if (column.StartsWith("PT_"))
                                        {
                                            ptGrades[index] = grade;
                                        }
                                        else if (column.StartsWith("QE_"))
                                        {
                                            qeGrades[index] = grade;
                                        }
                                    }
                                    else
                                    {
                                        dgv.Rows.Clear();
                                    }
                                }
                                string c1 = null, c2 = null, c3 = null, c4 = null;
                                for (int i = 0; i < 4; i++)
                                {
                                    double weightedTotal = (wwGrades[i] * ww) + (ptGrades[i] * pt) + (qeGrades[i] * qe);
                                    string formattedTotal = $"{Math.Round(weightedTotal, 2):F2}%";
                                    switch (i)
                                    {
                                        case 0: c1 = formattedTotal; break;
                                        case 1: c2 = formattedTotal; break;
                                        case 2: c3 = formattedTotal; break;
                                        case 3: c4 = formattedTotal; break;
                                    }
                                }
                                double total = 0;
                                int count = 0;
                                foreach (string c in new[] { c1, c2, c3, c4 })
                                {
                                    if (!string.IsNullOrWhiteSpace(c))
                                    {
                                        if (double.TryParse(c.TrimEnd('%'), out double val))
                                        {
                                            total += val;
                                            count++;
                                        }
                                    }
                                }
                                double avg = count > 0 ? total / count : 0;

                                dataGridView21.Rows.Add(subName, c1, c2, c3, c4, $"{avg:F2}%");
                            }
                        }
                    }
                }

                catch
                {
                    MessageBox.Show("An error occurred while loading grades.");
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

                try
                {
                    connection.Open();
                    totalRows = Convert.ToInt32(command.ExecuteScalar());
                }
                catch
                {
                    MessageBox.Show("Cannot Connect to Database");
                }
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
        private void subjectQuery(DataGridView DGV)
        {
            DGV.Rows.Clear();
            string selectQuery = @"SELECT * FROM subjects";

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                try
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

                                int rowIndex = DGV.Rows.Add(sPK, "", sn);

                                if (!string.IsNullOrEmpty(c))
                                {
                                    try
                                    {
                                        Color cellColor = ColorTranslator.FromHtml(c);
                                        DGV.Rows[rowIndex].Cells[1].Style.BackColor = cellColor;
                                    }
                                    catch
                                    {
                                        // Handle invalid color formats gracefully
                                        DGV.Rows[rowIndex].Cells[1].Style.BackColor = ColorTranslator.FromHtml("FFFFFF");
                                    }
                                }
                            }
                        }
                    }
                }
                catch
                {
                    MessageBox.Show("Cannot Connect to Database");
                }
            }
            dataGridView5.ClearSelection();
        }

        private void subjectQuery(DataGridView DGV, string spk)
        {
            DGV.Rows.Clear();

            // Collect all subject_pk values into a list
            List<string> subPKList = new List<string>();

            string selectQuery = "SELECT subject_pk FROM `grade-section` WHERE `grade-sec` = @spk";

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    // First query: get subject_pk(s) from grade-section
                    using (MySqlCommand selectCommand = new MySqlCommand(selectQuery, connection))
                    {
                        selectCommand.Parameters.AddWithValue("@spk", spk);

                        using (MySqlDataReader reader = selectCommand.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                string subPks = reader["subject_pk"].ToString();
                                string[] subPKArray = subPks.Split(',');
                                foreach (string subPK in subPKArray)
                                {
                                    if (!string.IsNullOrWhiteSpace(subPK))
                                    {
                                        subPKList.Add(subPK.Trim());
                                    }
                                }
                            }

                            reader.Close(); // Close outer reader before nested queries
                        }

                        // Now fetch each subject from the subjects table
                        foreach (string subPK in subPKList)
                        {
                            string selectQuery1 = "SELECT * FROM subjects WHERE subject_pk = @subpk";

                            using (MySqlCommand selectCommand1 = new MySqlCommand(selectQuery1, connection))
                            {
                                selectCommand1.Parameters.AddWithValue("@subpk", subPK);

                                using (MySqlDataReader reader1 = selectCommand1.ExecuteReader())
                                {
                                    if (reader1.Read())
                                    {
                                        string sn = reader1["subject_name"].ToString();
                                        string c = reader1["color"].ToString(); // stored hex string like "#FF0000"
                                        string sPK = reader1["subject_pk"].ToString();

                                        int rowIndex = DGV.Rows.Add(sPK, "", sn);

                                        if (!string.IsNullOrEmpty(c))
                                        {
                                            try
                                            {
                                                Color cellColor = ColorTranslator.FromHtml(c);
                                                DGV.Rows[rowIndex].Cells[1].Style.BackColor = cellColor;
                                            }
                                            catch
                                            {
                                                // Default to white background if parsing fails
                                                DGV.Rows[rowIndex].Cells[1].Style.BackColor = ColorTranslator.FromHtml("#FFFFFF");
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Cannot Connect to Database: " + ex.Message);
                }
            }
            DGV.ClearSelection();
        }

        //Loading Time
        private void LoadTimeDataFromDatabase(string PK, DataGridView DGV)
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
                    cmd.Parameters.AddWithValue("@GradeSectionPK", PK);
                    var result = cmd.ExecuteScalar();
                    if (result != null)
                    {
                        string timeData = result.ToString();

                        // Parse the timeData manually
                        List<List<int>> parsedData = ParseTimeData(timeData);

                        // Fill the DataGridView
                        for (int rowIndex = 0; rowIndex < parsedData.Count && rowIndex < DGV.Rows.Count; rowIndex++)
                        {
                            var row = DGV.Rows[rowIndex];
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
                                    row.Cells[dayIndex + 1].Style.BackColor = DGV.DefaultCellStyle.BackColor;
                                }
                            }
                        }
                    }
                }
            }
            DGV.ClearSelection();
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

        private void sectionComboQuery()
        {
            string query = "SELECT `grade-section_pk`, `grade-sec` FROM `grade-section`";

            DataTable dat = new DataTable();

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            using (MySqlCommand command = new MySqlCommand(query, connection))
            {
                try
                {
                    connection.Open();
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        dat.Load(reader);
                    }

                    comboBox2.DataSource = dat;
                    comboBox2.DisplayMember = "grade-sec";
                    comboBox2.ValueMember = "grade-section_pk";
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
            }
        }

        private void textBox4_Click(object sender, EventArgs e)
        {
            if (textBox4.Text == "Search...")
            {
                textBox4.Clear();
                textBox4.ForeColor = Color.Black;
            }
        }

        private void tabControl2_TabIndexChanged(object sender, EventArgs e)
        {
            sectionComboQuery();
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            stuQueryGrd();
        }

        //grades
        int dgv7 = 0;
        int dgv9 = 0;
        private void dataGridView7_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            dataGridView7.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridView7.Rows[e.RowIndex];

                // Assuming PK is in the first column (index 0)
                this.dgv7 = int.Parse(row.Cells[0].Value.ToString());
                label62.Visible = true;
                label62.Text = $"{row.Cells[2].Value.ToString()}, {row.Cells[3].Value.ToString()}";
                if (this.dgv9 != 0)
                {
                    loadGrades();
                }
            }
        }

        private void dataGridView9_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            dataGridView9.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridView9.Rows[e.RowIndex];

                // Assuming PK is in the first column (index 0)
                this.dgv9 = int.Parse(row.Cells[0].Value.ToString());
                label65.Visible = true;
                label65.Text = row.Cells[2].Value.ToString();
                if (this.dgv7 != 0)
                {
                    loadGrades();
                }
            }
        }

        private void loadGrades()
        {
            dataGridView8.Rows.Clear();
            dataGridView14.Rows.Clear();
            dataGridView17.Rows.Clear();
            dataGridView20.Rows.Clear();
            dataGridView10.Rows.Clear();
            dataGridView13.Rows.Clear();
            dataGridView16.Rows.Clear();
            dataGridView19.Rows.Clear();
            dataGridView11.Rows.Clear();
            dataGridView12.Rows.Clear();
            dataGridView15.Rows.Clear();
            dataGridView18.Rows.Clear();
            tabControl3.Enabled = true;
            string selectQuery = @$"SELECT * FROM grades WHERE student_pk = {this.dgv7} AND subject_pk = {this.dgv9}";

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    using (MySqlCommand selectCommand = new MySqlCommand(selectQuery, connection))
                    {
                        using (MySqlDataReader reader = selectCommand.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                // Parse weights from the reader
                                double.TryParse(reader["WW"]?.ToString(), out double ww);
                                double.TryParse(reader["PT"]?.ToString(), out double pt);
                                double.TryParse(reader["QE"]?.ToString(), out double qe);
                                ww /= 100;
                                pt /= 100;
                                qe /= 100;

                                textBox5.Text = reader["WW"].ToString();
                                textBox6.Text = reader["PT"].ToString();
                                textBox7.Text = reader["QE"].ToString();

                                // List of columns and their associated controls
                                var columns = new (string Column, DataGridView DGV, Label Label)[]
                                {
                                    ("WW_1", dataGridView8, label49),
                                    ("PT_1", dataGridView10, label48),
                                    ("QE_1", dataGridView11, label47),
                                    ("WW_2", dataGridView14, label50),
                                    ("PT_2", dataGridView13, label51),
                                    ("QE_2", dataGridView12, label52),
                                    ("WW_3", dataGridView17, label54),
                                    ("PT_3", dataGridView16, label55),
                                    ("QE_3", dataGridView15, label56),
                                    ("WW_4", dataGridView20, label58),
                                    ("PT_4", dataGridView19, label59),
                                    ("QE_4", dataGridView18, label60)
                                };

                                // Store intermediate results for weighted totals
                                double[] wwGrades = new double[4];
                                double[] ptGrades = new double[4];
                                double[] qeGrades = new double[4];

                                foreach (var (column, dgv, label) in columns)
                                {
                                    string cellData = reader[column]?.ToString();

                                    if (!string.IsNullOrWhiteSpace(cellData))
                                    {
                                        loadCell(cellData, dgv);

                                        double grade = computeGrade(cellData); // keep grade as double
                                        label.Text = $"{Math.Round(grade, 2)}%"; // rounding only here!

                                        // Determine which index to store
                                        int index = int.Parse(column.Substring(3)) - 1;

                                        if (column.StartsWith("WW_"))
                                        {
                                            wwGrades[index] = grade;
                                        }
                                        else if (column.StartsWith("PT_"))
                                        {
                                            ptGrades[index] = grade;
                                        }
                                        else if (column.StartsWith("QE_"))
                                        {
                                            qeGrades[index] = grade;
                                        }
                                    }
                                    else
                                    {
                                        dgv.Rows.Clear();
                                        label.Text = "0%";
                                    }
                                }

                                // Now compute weighted totals per set (_1 to _4)
                                for (int i = 0; i < 4; i++)
                                {
                                    // Use unrounded values for weighted total
                                    double weightedTotal = (wwGrades[i] * ww) + (ptGrades[i] * pt) + (qeGrades[i] * qe);

                                    // Round only at display time
                                    string formattedTotal = $"{Math.Round(weightedTotal, 2):F2}%";

                                    switch (i)
                                    {
                                        case 0:
                                            label46.Text = formattedTotal;
                                            break;
                                        case 1:
                                            label53.Text = formattedTotal;
                                            break;
                                        case 2:
                                            label57.Text = formattedTotal;
                                            break;
                                        case 3:
                                            label61.Text = formattedTotal;
                                            break;
                                    }
                                }
                            }
                        }
                    }
                }

                catch
                {
                    MessageBox.Show("An error occurred while loading grades.");
                }

            }
        }

        private void loadCell(string data, DataGridView DGV)
        {
            string[] dataArray = data.Split(',');
            foreach (string x in dataArray)
            {
                string[] score = x.Split('/');
                DGV.Rows.Add(score[0], score[1]);
            }
        }

        private double computeGrade(string data)
        {
            List<double> myList = new List<double>();
            string[] dataArray = data.Split(',');

            foreach (string x in dataArray)
            {
                string[] score = x.Split('/');
                double total = double.Parse(score[0]);
                double obtained = double.Parse(score[1]);

                // Multiply first to avoid losing precision (numerator * 100) / denominator
                double percentage = (obtained * 100) / total;
                myList.Add(percentage);
            }

            // Compute the average
            double average = 0;
            if (myList.Count > 0)
            {
                double sum = 0;
                foreach (double item in myList)
                {
                    sum += item;
                }
                average = sum / myList.Count;
            }

            return Math.Round(average, 2);
        }

        private void button8_Click(object sender, EventArgs e)
        {
            string ww_1 = serializeDataGridView(dataGridView8);
            string pt_1 = serializeDataGridView(dataGridView10);
            string qe_1 = serializeDataGridView(dataGridView11);
            string ww_2 = serializeDataGridView(dataGridView14);
            string pt_2 = serializeDataGridView(dataGridView13);
            string qe_2 = serializeDataGridView(dataGridView12);
            string ww_3 = serializeDataGridView(dataGridView17);
            string pt_3 = serializeDataGridView(dataGridView16);
            string qe_3 = serializeDataGridView(dataGridView15);
            string ww_4 = serializeDataGridView(dataGridView20);
            string pt_4 = serializeDataGridView(dataGridView19);
            string qe_4 = serializeDataGridView(dataGridView18);

            string updateQuery = @"
                UPDATE grades
                SET
                    WW = @WW,
                    PT = @PT,
                    QE = @QE,
                    WW_1 = @WW_1,
                    PT_1 = @PT_1,
                    QE_1 = @QE_1,
                    WW_2 = @WW_2,
                    PT_2 = @PT_2,
                    QE_2 = @QE_2,
                    WW_3 = @WW_3,
                    PT_3 = @PT_3,
                    QE_3 = @QE_3,
                    WW_4 = @WW_4,
                    PT_4 = @PT_4,
                    QE_4 = @QE_4
                WHERE
                    student_pk = @student_pk AND
                    subject_pk = @subject_pk;
            ";
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            using (MySqlCommand command = new MySqlCommand(updateQuery, connection))
            {
                command.Parameters.AddWithValue("@WW", textBox5.Text);
                command.Parameters.AddWithValue("@PT", textBox6.Text);
                command.Parameters.AddWithValue("@QE", textBox7.Text);
                command.Parameters.AddWithValue("@WW_1", ww_1);
                command.Parameters.AddWithValue("@PT_1", pt_1);
                command.Parameters.AddWithValue("@QE_1", qe_1);
                command.Parameters.AddWithValue("@WW_2", ww_2);
                command.Parameters.AddWithValue("@PT_2", pt_2);
                command.Parameters.AddWithValue("@QE_2", qe_2);
                command.Parameters.AddWithValue("@WW_3", ww_3);
                command.Parameters.AddWithValue("@PT_3", pt_3);
                command.Parameters.AddWithValue("@QE_3", qe_3);
                command.Parameters.AddWithValue("@WW_4", ww_4);
                command.Parameters.AddWithValue("@PT_4", pt_4);
                command.Parameters.AddWithValue("@QE_4", qe_4);
                command.Parameters.AddWithValue("@student_pk", this.dgv7.ToString());
                command.Parameters.AddWithValue("@subject_pk", this.dgv9.ToString());

                connection.Open();
                command.ExecuteNonQuery();
                loadGrades();
            }
        }
        private string serializeDataGridView(DataGridView dgv)
        {
            List<string> rowData = new List<string>();

            foreach (DataGridViewRow row in dgv.Rows)
            {
                // Skip the new row at the bottom if AllowUserToAddRows is enabled
                if (row.IsNewRow) continue;

                // Get cell values
                string cell0 = row.Cells[0].Value?.ToString() ?? "";
                string cell1 = row.Cells[1].Value?.ToString() ?? "";

                // Format them as "cell0/cell1"
                rowData.Add($"{cell0}/{cell1}");
            }

            // Join all rows with commas
            string result = string.Join(",", rowData);
            return result;
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked == true)
            {
                textBox5.ReadOnly = false;
                textBox6.ReadOnly = false;
                textBox7.ReadOnly = false;
            }
            else
            {
                textBox5.ReadOnly = true;
                textBox6.ReadOnly = true;
                textBox7.ReadOnly = true;
            }
        }

        private void isAlphaNumerical(object sender, EventArgs e)
        {
            System.Windows.Forms.TextBox tb = sender as System.Windows.Forms.TextBox;
            if (tb != null)
            {
                int selectionStart = tb.SelectionStart;
                string filteredText = Regex.Replace(tb.Text, @"[^0-9]", "");
                if (tb.Text != filteredText)
                {
                    tb.Text = filteredText;
                    tb.SelectionStart = Math.Min(selectionStart, tb.Text.Length);
                }
            }
        }

        private void dataGridView_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            if (e.Control is System.Windows.Forms.TextBox textBox)
            {
                // Remove any previously attached event handler to avoid duplication
                textBox.TextChanged -= TextBox_TextChanged_FilterNumbers;
                textBox.TextChanged += TextBox_TextChanged_FilterNumbers;
            }
        }

        private void TextBox_TextChanged_FilterNumbers(object sender, EventArgs e)
        {
            if (sender is System.Windows.Forms.TextBox textBox)
            {
                int selectionStart = textBox.SelectionStart;
                string filteredText = Regex.Replace(textBox.Text, @"[^0-9]", "");
                if (textBox.Text != filteredText)
                {
                    textBox.Text = filteredText;
                    textBox.SelectionStart = Math.Min(selectionStart, textBox.Text.Length);
                }
            }
        }
    }
}
