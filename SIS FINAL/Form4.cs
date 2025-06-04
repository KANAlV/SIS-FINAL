using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;

namespace SIS_FINAL
{
    public partial class Form4 : Form
    {
        string connectionString = "server=localhost;database=sis_final;user=root;password=;";
        private string selectedPhotoPath = null;
        string studPK;
        byte[] photoBytes = null;
        public Form4(string stuPK)
        {
            this.studPK = stuPK;
            InitializeComponent();
            sectionQuery();
            getStuCount(stuPK);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            bool check = true;
            if (textBox1.Text.Trim() == "" || textBox1.Text == null) { check = false; }
            if (textBox2.Text.Trim() == "" || textBox2.Text == null) { check = false; }
            if (textBox9.Text.Trim() == "" || textBox9.Text == null) { check = false; }
            if (textBox8.Text.Trim() == "" || textBox8.Text == null) { check = false; }
            if (comboBox1.Text.Trim() == "" || comboBox1.Text == null) { check = false; }
            if (comboBox3.Text.Trim() == "" || comboBox3.Text == null) { check = false; }
            if (textBox12.Text.Trim() == "" || textBox12.Text == null) { check = false; }
            if (check == true)
            {
                string photoFilePath = selectedPhotoPath;
                byte[] photoBytes = null;

                if (!string.IsNullOrEmpty(photoFilePath) && File.Exists(photoFilePath))
                {
                    photoBytes = File.ReadAllBytes(photoFilePath);
                }

                string updateStudentQuery = @"
                    UPDATE students
                    SET
                        photo = @photo,
                        surname = @surname,
                        first_name = @first_name,
                        middle_name = @middle_name,
                        suffix = @suffix,
                        gender = @gender,
                        `grade-sec_pk` = @gs,
                        enrolled = @enrolled
                    WHERE student_pk = @student_pk;
                ";

                using (MySqlConnection connection = new MySqlConnection(connectionString))
                using (MySqlCommand command = new MySqlCommand(updateStudentQuery, connection))
                {
                    command.Parameters.AddWithValue("@surname", textBox1.Text);
                    command.Parameters.AddWithValue("@first_name", textBox2.Text);
                    command.Parameters.AddWithValue("@middle_name", textBox3.Text);
                    command.Parameters.AddWithValue("@suffix", textBox4.Text);
                    command.Parameters.AddWithValue("@gender", comboBox1.Text);

                    if (comboBox2.SelectedValue != null)
                    {
                        command.Parameters.AddWithValue("@gs", comboBox2.SelectedValue.ToString());
                    }
                    else
                    {
                        command.Parameters.AddWithValue("@gs", DBNull.Value);
                    }

                    command.Parameters.AddWithValue("@enrolled", comboBox4.Text);

                    if (photoBytes != null)
                    {
                        command.Parameters.Add("@photo", MySqlDbType.LongBlob).Value = photoBytes;
                    }
                    else
                    {
                        command.Parameters.Add("@photo", MySqlDbType.LongBlob).Value = this.photoBytes;
                    }

                    // Add primary key parameter to identify the record to update
                    command.Parameters.AddWithValue("@student_pk", this.studPK); // Replace this with your actual student PK reference

                    try
                    {
                        connection.Open();
                        int rowsUpdated = command.ExecuteNonQuery();

                        MessageBox.Show($"{rowsUpdated} student record updated.");

                        string updateGuardianQuery = @"
                        UPDATE guardian
                        SET
                            surname = @surname,
                            first_name = @first_name,
                            middle_name = @middle_name,
                            suffix = @suffix,
                            relationship = @relationship,
                            gender = @gender,
                            email = @email,
                            phone = @phone
                        WHERE students_pk = @students_pk;
                    ";

                        using (MySqlCommand command2 = new MySqlCommand(updateGuardianQuery, connection))
                        {
                            command2.Parameters.AddWithValue("@students_pk", this.studPK); // same as above, adjust as needed
                            command2.Parameters.AddWithValue("@surname", textBox9.Text);
                            command2.Parameters.AddWithValue("@first_name", textBox8.Text);
                            command2.Parameters.AddWithValue("@middle_name", textBox7.Text);
                            command2.Parameters.AddWithValue("@suffix", textBox6.Text);
                            command2.Parameters.AddWithValue("@gender", comboBox3.Text);
                            command2.Parameters.AddWithValue("@relationship", textBox12.Text);
                            command2.Parameters.AddWithValue("@email", textBox10.Text);
                            command2.Parameters.AddWithValue("@phone", textBox11.Text);

                            int rowsUpdated2 = command2.ExecuteNonQuery();
                            MessageBox.Show($"{rowsUpdated2} guardian record updated.");

                            this.Close();
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error updating student or guardian: " + ex.Message);
                    }
                }
            }
            else { MessageBox.Show("Not all required fields are filled"); }
        }

        public class ComboBoxItem
        {
            public string Text { get; set; }
            public string Value { get; set; }

            public ComboBoxItem(string text, string value)
            {
                Text = text;
                Value = value;
            }

            public override string ToString()
            {
                return Text; // what shows in the ComboBox dropdown
            }
        }

        private void sectionQuery()
        {
            string query = "SELECT `grade-section_pk`, `grade-sec` FROM `grade-section`";

            DataTable dt = new DataTable();

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            using (MySqlCommand command = new MySqlCommand(query, connection))
            {
                try
                {
                    connection.Open();
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        dt.Load(reader);
                    }

                    comboBox2.DataSource = dt;
                    comboBox2.DisplayMember = "grade-sec";
                    comboBox2.ValueMember = "grade-section_pk";
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
            }
        }

        private void getStuCount(string stuPK)
        {
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
            WHERE students.student_pk = {stuPK}";
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
                                textBox5.Text = reader["students_no"].ToString();
                                textBox1.Text = reader["surname"].ToString();
                                textBox2.Text = reader["first_name"].ToString();
                                textBox3.Text = reader["middle_name"].ToString();
                                textBox4.Text = reader["suffix"].ToString();
                                comboBox1.Text = reader["gender"].ToString();
                                string gradeSec = reader["grade-sec"].ToString();
                                int index = comboBox2.FindStringExact(gradeSec);
                                if (index >= 0)
                                {
                                    comboBox2.SelectedIndex = index;
                                }
                                else
                                {
                                    comboBox2.SelectedIndex = -1; // Optional: clear selection if not found
                                }
                                // === Image loading ===
                                if (reader["photo"] != DBNull.Value)
                                {
                                    byte[] photoBytes = (byte[])reader["photo"];
                                    this.photoBytes = (byte[])reader["photo"];
                                    using (var ms = new MemoryStream(photoBytes))
                                    {
                                        pictureBox1.Image = Image.FromStream(ms);
                                    }
                                }
                                else
                                {
                                    pictureBox1.Image = null; // clear if no photo
                                }
                                comboBox4.Text = reader["enrolled"].ToString();
                                textBox9.Text = reader["guardian_surname"].ToString();
                                textBox8.Text = reader["guardian_first_name"].ToString();
                                textBox7.Text = reader["guardian_middle_name"].ToString();
                                textBox6.Text = reader["guardian_suffix"].ToString();
                                comboBox3.Text = reader["guardian_gender"].ToString();
                                textBox12.Text = reader["relationship"].ToString();
                                textBox10.Text = reader["email"].ToString();
                                textBox11.Text = reader["phone"].ToString();
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

        private void button2_Click(object sender, EventArgs e)
        {
            SelectAndDisplayImage();
        }
        private void SelectAndDisplayImage()
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "Image Files (*.jpg;*.jpeg;*.png;*.bmp)|*.jpg;*.jpeg;*.png;*.bmp";

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    string selectedFilePath = ofd.FileName;
                    this.selectedPhotoPath = selectedFilePath;

                    // Display the selected image in the PictureBox
                    pictureBox1.Image = Image.FromFile(selectedFilePath);

                    // Optionally, you can store the path or the image bytes for later saving
                }
            }
        }
    }
}
