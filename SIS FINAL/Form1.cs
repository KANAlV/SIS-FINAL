using MySql.Data.MySqlClient;
using Mysqlx.Notice;
using Mysqlx.Resultset;
using MySqlX.XDevAPI.Relational;
using Org.BouncyCastle.Bcpg;
using System.Data;
using System.Diagnostics;
using System.Runtime.Intrinsics.X86;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using static Mysqlx.Error.Types;
using static System.Runtime.InteropServices.JavaScript.JSType;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace SIS_FINAL
{
    public partial class Form1 : Form
    {
        string connectionString = "server=localhost;database=sis_final;user=root;password=;";
        List<double> finalGrades = new List<double>();
        int StuPage = 0;
        int gsPK = 0;
        public Form1()
        {
            InitializeComponent();
            sectionComboQuery();
            studentQuery();
            sectionQuery();
            stuQueryGrd();
            stuQueryDisc();
            for (int i = 1; i <= 25; i++)
            {
                string dgvName = $"dataGridView{i}";
                Control[] foundControls = this.Controls.Find(dgvName, true);
                if (foundControls.Length > 0 && foundControls[0] is DataGridView dgv)
                {
                    dgv.RowHeadersVisible = false;
                    dgv.MultiSelect = false;
                }
            }

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

            if (string.IsNullOrEmpty(searchText) || searchText.Equals("Search...", StringComparison.OrdinalIgnoreCase))
            {
                query = "SELECT * FROM `grade-section`";
            }
            else
            {
                query = "SELECT * FROM `grade-section` WHERE `grade-sec` LIKE @searchText OR `section_name` LIKE @searchText";
            }

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    if (!string.IsNullOrEmpty(searchText) && !searchText.Equals("Search...", StringComparison.OrdinalIgnoreCase))
                    {
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
                            command.Parameters.AddWithValue("@searchText", $"%{textBox1.Text.Trim()}%");
                        }
                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            dataGridView1.Rows.Clear();

                            while (reader.Read())
                            {
                                string stuPK = reader["student_pk"].ToString();
                                string stuNo = reader["students_no"].ToString();
                                string sur = reader["surname"].ToString();
                                string fn = reader["first_name"].ToString();
                                string gndr = reader["gender"].ToString();

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

            if (string.IsNullOrEmpty(textBox4.Text.Trim()) || textBox4.Text.Trim().Equals("Search...", StringComparison.OrdinalIgnoreCase))
            {
                // Normal query
                query = @$"
                    SELECT student_pk, students_no, surname, first_name FROM students WHERE `grade-sec_pk` = @spk AND enrolled = Enrolled;
                ";
            }
            else
            {
                // Search query
                query = @$"
                    SELECT student_pk, students_no, surname, first_name FROM students
                    WHERE `grade-sec_pk` = @spk
                        AND enrolled = Enrolled
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
                            command.Parameters.AddWithValue("@searchText", $"%{textBox4.Text.Trim()}%");
                        }
                        command.Parameters.AddWithValue("@spk", comboBox2.SelectedValue.ToString());
                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            dataGridView7.Rows.Clear();

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

        private void stuQueryDisc()
        {
            dataGridView22.Rows.Clear();
            string query;

            if (string.IsNullOrEmpty(textBox8.Text.Trim()) || textBox8.Text.Trim().Equals("Search...", StringComparison.OrdinalIgnoreCase))
            {
                // Normal query
                query = @$"
                    SELECT student_pk, students_no, surname, first_name, `grade-sec_pk` FROM students WHERE enrolled = Enrolled;
                ";
            }
            else
            {
                // Search query
                query = @$"
                    SELECT student_pk, students_no, surname, first_name, `grade-sec_pk` FROM students
                    WHERE enrolled = Enrolled
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
                        if (!string.IsNullOrEmpty(textBox8.Text.Trim()) && !textBox8.Text.Trim().Equals("Search...", StringComparison.OrdinalIgnoreCase))
                        {
                            command.Parameters.AddWithValue("@searchText", $"%{textBox8.Text.Trim()}%");
                        }
                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            dataGridView22.Rows.Clear();

                            while (reader.Read())
                            {
                                string stuPK = reader["student_pk"].ToString();
                                string stuNo = reader["students_no"].ToString();
                                string sur = reader["surname"].ToString();
                                string fn = reader["first_name"].ToString();
                                string gsPK = reader["grade-sec_pk"].ToString();

                                dataGridView22.Rows.Add(stuPK, stuNo, sur, fn, gsPK);
                            }
                        }
                    }
                    dataGridView22.ClearSelection();
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
                                    int divBy = this.finalGrades.Count;
                                    double fgSum = 0;
                                    foreach (var grade in finalGrades)
                                    {
                                        fgSum += grade;
                                    }
                                    dataGridView21.Rows.Add("", "", "", "", "Total:", $"{Math.Round((fgSum / divBy), 2)}%");
                                    this.finalGrades.Clear();
                                    dataGridView21.ClearSelection();

                                    //Disciplinary
                                    discDisplayStu(stuPK, reader["grade-sec_pk"].ToString());

                                    //Image loading
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
                                this.finalGrades.Add(Math.Round(avg, 2));
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

            List<string> subPKList = new List<string>();

            string selectQuery = "SELECT subject_pk FROM `grade-section` WHERE `grade-sec` = @spk";

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();

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

                            reader.Close();
                        }

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
                                        string c = reader1["color"].ToString();
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

                        List<List<int>> parsedData = ParseTimeData(timeData);

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
            timeData = timeData.Replace(" ", "").Replace("\r", "").Replace("\n", "");

            timeData = timeData.TrimStart('[').TrimEnd(']');

            var rows = new List<List<int>>();

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
                        intList.Add(0);
                    }
                }

                rows.Add(intList);
            }

            return rows;
        }
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

                this.dgv7 = int.Parse(row.Cells[0].Value.ToString());
                label62.Visible = true;
                label62.Text = $"{row.Cells[2].Value.ToString()}, {row.Cells[3].Value.ToString()}";
                this.dgv9 = 0;
                label65.Visible = false;
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
                textBox5.Clear();
                textBox6.Clear();
                textBox7.Clear();
                tabControl3.Enabled = false;
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
                                double.TryParse(reader["WW"]?.ToString(), out double ww);
                                double.TryParse(reader["PT"]?.ToString(), out double pt);
                                double.TryParse(reader["QE"]?.ToString(), out double qe);
                                ww /= 100;
                                pt /= 100;
                                qe /= 100;

                                textBox5.Text = reader["WW"].ToString();
                                textBox6.Text = reader["PT"].ToString();
                                textBox7.Text = reader["QE"].ToString();

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

                                double[] wwGrades = new double[4];
                                double[] ptGrades = new double[4];
                                double[] qeGrades = new double[4];

                                foreach (var (column, dgv, label) in columns)
                                {
                                    string cellData = reader[column]?.ToString();

                                    if (!string.IsNullOrWhiteSpace(cellData))
                                    {
                                        loadCell(cellData, dgv);

                                        double grade = computeGrade(cellData);
                                        label.Text = $"{Math.Round(grade, 2)}%";

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

                                for (int i = 0; i < 4; i++)
                                {
                                    double weightedTotal = (wwGrades[i] * ww) + (ptGrades[i] * pt) + (qeGrades[i] * qe);

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

                double percentage = (obtained * 100) / total;
                myList.Add(percentage);
            }

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
                if (row.IsNewRow) continue;

                string cell0 = row.Cells[0].Value?.ToString() ?? "";
                string cell1 = row.Cells[1].Value?.ToString() ?? "";

                rowData.Add($"{cell0}/{cell1}");
            }

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

        //Diciplinary
        private void textBox8_TextChanged(object sender, EventArgs e)
        {
            stuQueryDisc();
        }

        private void button9_Click(object sender, EventArgs e)
        {
            stuQueryDisc();
        }

        private void dataGridView22_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridView22.Rows[e.RowIndex];

                this.dgv7 = int.Parse(row.Cells[0].Value.ToString());
                label67.Visible = true;
                label67.Text = $"{row.Cells[2].Value.ToString()}, {row.Cells[3].Value.ToString()}";
                label68.Text = row.Cells[0].Value.ToString();
                label73.Text = row.Cells[4].Value.ToString();
            }
        }

        private void textBox8_Click(object sender, EventArgs e)
        {
            if (textBox8.Text == "Search...")
            {
                textBox8.ForeColor = Color.Black;
                textBox8.Clear();
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            comboBox3.Items.Clear();
            comboBox3.Items.Add("");
            switch (comboBox1.Text)
            {
                case "Low":
                    comboBox3.Items.Add("Disruptive Behavior");
                    comboBox3.Items.Add("Attendance Issues");
                    comboBox3.Items.Add("Academic Dishonesty");
                    comboBox3.Items.Add("Verbal Aggression/Harassment");
                    break;
                case "Medium":
                    comboBox3.Items.Add("Disruptive Behavior");
                    comboBox3.Items.Add("Attendance Issues");
                    comboBox3.Items.Add("Defiance/Noncompliance");
                    comboBox3.Items.Add("Academic Dishonesty");
                    comboBox3.Items.Add("Physical Aggression");
                    comboBox3.Items.Add("Vandalism/Theft");
                    comboBox3.Items.Add("Substance-Related Offenses");
                    break;
                case "High":
                    comboBox3.Items.Add("Physical Aggression");
                    comboBox3.Items.Add("Verbal Aggression/Harassment");
                    comboBox3.Items.Add("Vandalism/Theft");
                    comboBox3.Items.Add("Substance-Related Offenses");
                    comboBox3.Items.Add("Weapons-Related");
                    comboBox3.Items.Add("Sexual Misconduct");
                    break;
            }
        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            string severity = comboBox1.Text;
            string category = comboBox3.Text;
            dataGridView24.Rows.Clear();

            string selectQuery = @$"SELECT * FROM disciplinary_list WHERE category = @category AND severity = @severity";

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    using (MySqlCommand selectCommand = new MySqlCommand(selectQuery, connection))
                    {
                        selectCommand.Parameters.AddWithValue("@severity", severity);
                        selectCommand.Parameters.AddWithValue("@category", category);
                        using (MySqlDataReader reader = selectCommand.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                string issues = reader["issue"].ToString();
                                string[] dataArray = issues.Split('|');
                                foreach (string x in dataArray)
                                {
                                    dataGridView24.Rows.Add(x);
                                }
                            }
                        }
                    }
                }

                catch
                {
                    MessageBox.Show("An error occurred while loading issues.");
                }

            }
        }

        private void dataGridView24_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridView24.Rows[e.RowIndex];

                string issue = row.Cells[0].Value?.ToString() ?? "";

                bool matchFound = false;
                foreach (DataGridViewRow dgv23Row in dataGridView23.Rows)
                {
                    if (dgv23Row.IsNewRow)
                        continue;

                    string cellValue = dgv23Row.Cells[2].Value?.ToString() ?? "";
                    if (cellValue.Equals(issue, StringComparison.OrdinalIgnoreCase))
                    {
                        matchFound = true;
                        break;
                    }
                }

                if (matchFound)
                {
                    MessageBox.Show($"Issue \"{issue}\" is already applied!");
                }
                else
                {
                    dataGridView23.Rows.Add(comboBox3.Text, comboBox1.Text, issue);
                }
            }
        }

        private void dataGridView23_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                dataGridView23.Rows.RemoveAt(e.RowIndex);
            }
        }

        private void button10_Click(object sender, EventArgs e)
        {
            List<string> data = new List<string>();
            List<string> categList = new List<string>();
            List<int> categCount = new List<int>();

            data.Clear();
            categList.Clear();
            categCount.Clear();

            // Collect data from DataGridView23
            foreach (DataGridViewRow row in dataGridView23.Rows)
            {
                if (row.IsNewRow) continue; // skip empty rows

                string category = row.Cells[0].Value?.ToString() ?? "";
                string severity = row.Cells[1].Value?.ToString() ?? "";
                string issue = row.Cells[2].Value?.ToString() ?? "";

                if (!string.IsNullOrWhiteSpace(category))
                {
                    string entry = $"{category}|{severity}|{issue}";
                    data.Add(entry);

                    if (!categList.Contains(category))
                    {
                        categList.Add(category);
                    }
                }
            }

            // Get offence count for each category
            foreach (string item in categList)
            {
                string offenceQuery = @"
                    SELECT MAX(offence_no) AS count
                    FROM disciplinary_records
                    WHERE student_pk = @stuPK AND category = @category
                    LIMIT 1
                ";

                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    try
                    {
                        connection.Open();

                        using (MySqlCommand selectCommand = new MySqlCommand(offenceQuery, connection))
                        {
                            selectCommand.Parameters.AddWithValue("@stuPK", label68.Text);
                            selectCommand.Parameters.AddWithValue("@category", item);

                            using (MySqlDataReader reader = selectCommand.ExecuteReader())
                            {
                                if (reader.Read())
                                {
                                    int currentMax = int.Parse(reader["count"].ToString());
                                    int offenceCount = currentMax + 1;
                                    categCount.Add(offenceCount);
                                }
                                else
                                {
                                    categCount.Add(1);
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        //MessageBox.Show("An error occurred while loading offence count:" + ex.Message);
                        categCount.Add(1); // fallback
                    }
                }
            }

            // Insert data into disciplinary_records
            int totalRowsInserted = 0;
            foreach (string x in data)
            {
                string[] dataArray = x.Split('|');
                int catIndex = categList.IndexOf(dataArray[0]);
                int issCount = categCount[catIndex];

                string insertData = @"
                    INSERT INTO `disciplinary_records` (
                        `student_pk`,
                        `category`,
                        `severity`,
                        `issue`,
                        `comments`,
                        `offence_no`,
                        `gs_pk`
                    )
                    VALUES (
                        @spk,
                        @cat,
                        @sev,
                        @iss,
                        @com,
                        @off,
                        @gsPK
                    )
                ";

                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    using (MySqlCommand command = new MySqlCommand(insertData, connection))
                    {
                        command.Parameters.AddWithValue("@spk", label68.Text);
                        command.Parameters.AddWithValue("@cat", dataArray[0]);
                        command.Parameters.AddWithValue("@sev", dataArray[1]);
                        command.Parameters.AddWithValue("@iss", dataArray[2]);
                        command.Parameters.AddWithValue("@com", richTextBox1.Text);
                        command.Parameters.AddWithValue("@off", issCount);
                        command.Parameters.AddWithValue("@gsPK", label73.Text);

                        try
                        {
                            connection.Open();
                            int rowsInserted = command.ExecuteNonQuery();
                            totalRowsInserted += rowsInserted;
                            Debug.WriteLine($"{rowsInserted} row(s) inserted.");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Error: " + ex.Message);
                            Debug.WriteLine("Error: " + ex.Message);
                        }
                    }
                }
            }

            MessageBox.Show($"{totalRowsInserted} row(s) inserted.");
            data.Clear();
            categList.Clear();
            categCount.Clear();
        }

        private void discDisplayStu(string sPk, string subPK)
        {
            dataGridView25.Rows.Clear();
            string offenceQuery = @"
                    SELECT dr_pk, category, severity, issue FROM disciplinary_records
                    WHERE student_pk = @stuPK AND gs_pk = @gs_pk
                ";

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    using (MySqlCommand selectCommand = new MySqlCommand(offenceQuery, connection))
                    {
                        selectCommand.Parameters.AddWithValue("@stuPK", sPk);
                        selectCommand.Parameters.AddWithValue("@gs_pk", subPK);

                        using (MySqlDataReader reader = selectCommand.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                string PK = reader["dr_pk"].ToString();
                                string Category = reader["category"].ToString();
                                string Severity = reader["severity"].ToString();
                                string Issue = reader["issue"].ToString();

                                dataGridView25.Rows.Add(PK, Category, Severity, Issue);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    dataGridView25.Rows.Add("N/a", "N/a", "No Records");
                    //categCount.Add(1);
                }
            }
        }

        private void dataGridView25_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            richTextBox2.Clear();
            dataGridView25.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridView25.Rows[e.RowIndex];
                var pkValue = row.Cells[0].Value;
                if (pkValue != null)
                {
                    string offenceQuery = @"
                    SELECT comment FROM disciplinary_records
                    WHERE dr_pk = @dr_pk
                ";

                    using (MySqlConnection connection = new MySqlConnection(connectionString))
                    {
                        try
                        {
                            connection.Open();

                            using (MySqlCommand selectCommand = new MySqlCommand(offenceQuery, connection))
                            {
                                selectCommand.Parameters.AddWithValue("@dr_pk", pkValue);

                                using (MySqlDataReader reader = selectCommand.ExecuteReader())
                                {
                                    while (reader.Read())
                                    {
                                        richTextBox2.Text = reader["comment"].ToString();
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            dataGridView25.Rows.Add("N/a", "N/a", "No Records");
                            //categCount.Add(1);
                        }
                    }
                }
            }
        }
    }
}