using System;
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

namespace SIS_FINAL
{
    public partial class Form3 : Form
    {
        string connectionString = "server=localhost;database=sis_final;user=root;password=;";
        private string selectedPhotoPath = null;
        public Form3()
        {
            InitializeComponent();
            sectionQuery();
            getStuCount();
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

                string insertStudentQuery = @"
                    INSERT INTO students
                    (students_no, photo, surname, first_name, middle_name, suffix, gender, `grade-sec_pk`, enrolled)
                    VALUES
                    (@students_no, @photo, @surname, @first_name, @middle_name, @suffix, @gender, @gs, @enrolled);
                ";

                using (MySqlConnection connection = new MySqlConnection(connectionString))
                using (MySqlCommand command = new MySqlCommand(insertStudentQuery, connection))
                {
                    command.Parameters.AddWithValue("@students_no", textBox5.Text);
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
                        command.Parameters.Add("@photo", MySqlDbType.LongBlob).Value = DBNull.Value;
                    }

                    try
                    {
                        connection.Open();
                        int rowsInserted = command.ExecuteNonQuery();
                        long studentPK = command.LastInsertedId;

                        MessageBox.Show($"{rowsInserted} student record inserted.");

                        string insertGuardianQuery = @"
                            INSERT INTO guardian
                            (students_pk, surname, first_name, middle_name, suffix, relationship, gender, email, phone)
                            VALUES
                            (@students_pk, @surname, @first_name, @middle_name, @suffix, @relationship, @gender, @email, @phone);
                        ";

                        using (MySqlCommand command2 = new MySqlCommand(insertGuardianQuery, connection))
                        {
                            command2.Parameters.AddWithValue("@students_pk", studentPK);
                            command2.Parameters.AddWithValue("@surname", textBox9.Text);
                            command2.Parameters.AddWithValue("@first_name", textBox8.Text);
                            command2.Parameters.AddWithValue("@middle_name", textBox7.Text);
                            command2.Parameters.AddWithValue("@suffix", textBox6.Text);
                            command2.Parameters.AddWithValue("@gender", comboBox3.Text);
                            command2.Parameters.AddWithValue("@relationship", textBox12.Text);
                            command2.Parameters.AddWithValue("@email", textBox10.Text);
                            command2.Parameters.AddWithValue("@phone", textBox11.Text);

                            int rowsInserted2 = command2.ExecuteNonQuery();
                            MessageBox.Show($"{rowsInserted2} guardian record inserted.");

                            //Inserting Grades
                            if (comboBox2.SelectedValue.ToString() != null)
                            {
                                try
                                {
                                    string subPks = fetchSubPKs(int.Parse(comboBox2.SelectedValue.ToString()));
                                    string[] subPKArray = subPks.Split(',');
                                    foreach (string subPK in subPKArray)
                                    {
                                        string insertGradesQuery = @"
                                        INSERT INTO grades
                                        (student_pk, subject_pk)
                                        VALUES
                                        (@students_pk, @subject_pk);
                                    ";

                                        using (MySqlCommand command3 = new MySqlCommand(insertGradesQuery, connection))
                                        {
                                            command3.Parameters.AddWithValue("@students_pk", studentPK);
                                            command3.Parameters.AddWithValue("@subject_pk", subPK);

                                            int rowsInserted3 = command3.ExecuteNonQuery();
                                            this.Close();
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show("Error inserting student or guardian: " + ex.Message);
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error inserting student or guardian: " + ex.Message);
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

        private void getStuCount()
        {
            string query = "SELECT MAX(students_no) FROM `students`";
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
                                int grdSecPK = int.Parse(reader["students_no"].ToString());
                                grdSecPK++;
                                textBox5.Text = grdSecPK.ToString();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        textBox5.Text = "100000000001";
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

        private string fetchSubPKs(int gs)
        {
            string query = @$"
                SELECT subject_pk FROM `grade-section` WHERE `grade-section_pk` = {gs};
            ";

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                string subPK = reader["subject_pk"].ToString();

                                return subPK;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex.Message);
                }
            }
            return null;
        }
    }
}
