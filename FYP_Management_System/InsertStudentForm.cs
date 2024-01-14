using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using FYP_Management_System.BL;

namespace FYP_Management_System
{
    public partial class InsertStudentForm : Form
    {
        private Student s;
        bool[] emailSymbols;

        public InsertStudentForm(Student s)
        {
            InitializeComponent();
            this.s = s;
            emailSymbols = new bool[2];
        }

        private void InsertStudentForm_Load(object sender, EventArgs e)
        {
            emailSymbols[0] = false;
            emailSymbols[1] = false;
            dateTimePicker1.Value = dateTimePicker1.MinDate;

            var con = Configuration.getInstance().getConnection();
            SqlCommand cmd = new SqlCommand("Select Value from Lookup Where Category = \'Gender\'", con);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            cmbGender.DataSource = dt;
            cmbGender.ValueMember = "Value";
            cmd.ExecuteNonQuery();

            if (s.RegistrationNumber != "")
            {
                emailSymbols[0] = true;
                emailSymbols[1] = true;

                SqlCommand cmd1 = new SqlCommand("Select Value from Lookup Where Id = @id", con);
                cmd1.Parameters.AddWithValue("@id", s.Gender);

                string value = (string)cmd1.ExecuteScalar();
                cmd1.ExecuteNonQuery();

                txtRNumber.Text = s.RegistrationNumber;
                txtFName.Text = s.FirstName;
                txtLName.Text = s.LastName;
                txtContact.Text = s.Contact;
                txtEmail.Text = s.Email;
                cmbGender.SelectedValue = value;

                DateTime dob = DateTime.Parse(s.Dob);
                dateTimePicker1.Value = dob;

                btnInsert.Hide();
            }
            else
            {
                btnUpdate.Hide();
            }
        }

        private void btnInsert_Click(object sender, EventArgs e)
        {
            try
            {
                var con = Configuration.getInstance().getConnection();

                SqlCommand cmd4 = new SqlCommand("Select Count(Email) From Person Where @email = Email", con);
                cmd4.Parameters.AddWithValue("@email", txtEmail.Text);
                int isExist = int.Parse(cmd4.ExecuteScalar().ToString());
                cmd4.ExecuteNonQuery();

                checkEmailFormat();
                if(!emailSymbols[0] && !emailSymbols[1])
                {
                    MessageBox.Show("Enter email in correct format.");
                    return;
                }

                if (isExist > 0)
                {
                    MessageBox.Show("Email already exists.");
                    return;
                }

                SqlCommand cmd5 = new SqlCommand("Select Count(RegistrationNo) From Student Where @rNo = RegistrationNo", con);
                cmd5.Parameters.AddWithValue("@rNo", txtRNumber.Text);
                isExist = int.Parse(cmd5.ExecuteScalar().ToString());
                cmd5.ExecuteNonQuery();

                if (isExist > 0)
                {
                    MessageBox.Show("Registration Number already exists.");
                    return;
                }

                if (txtFName.Text != "" && txtRNumber.Text != "")
                {
                    SqlCommand cmd6 = new SqlCommand("Insert into Person (FirstName, Email) values (@FirstName, @Email)", con);
                    cmd6.Parameters.AddWithValue("@FirstName", txtFName.Text);
                    cmd6.Parameters.AddWithValue("@Email", txtEmail.Text);
                    cmd6.ExecuteNonQuery();

                    SqlCommand cmd2 = new SqlCommand("Select Max(Id) From Person", con);
                    int id = int.Parse(cmd2.ExecuteScalar().ToString());
                    cmd2.ExecuteNonQuery();

                    if (txtLName.Text != "")
                    {
                        SqlCommand cmd9 = new SqlCommand("Update Person Set LastName = @LastName Where Id = @id", con);
                        cmd9.Parameters.AddWithValue("@LastName", txtLName.Text);
                        cmd9.Parameters.AddWithValue("@id", id);
                        cmd9.ExecuteNonQuery();
                    }

                    if (txtContact.Text != "")
                    {
                        SqlCommand cmd10 = new SqlCommand("Update Person Set Contact = @Contact Where Id = @id", con);
                        cmd10.Parameters.AddWithValue("@Contact", txtContact.Text);
                        cmd10.Parameters.AddWithValue("@id", id);
                        cmd10.ExecuteNonQuery();
                    }

                    if (dateTimePicker1.Value != dateTimePicker1.MinDate)
                    {
                        SqlCommand cmd12 = new SqlCommand("Update Person Set DateOfBirth = @DateOfBirth Where Id = @id", con); ;
                        cmd12.Parameters.AddWithValue("@DateOfBirth", dateTimePicker1.Text);
                        cmd12.Parameters.AddWithValue("@id", id);
                        cmd12.ExecuteNonQuery();
                    }

                    SqlCommand cmd13 = new SqlCommand("Select Id From Lookup Where Value = @gen", con);
                    cmd13.Parameters.AddWithValue("@gen", cmbGender.SelectedValue.ToString());
                    int genId = int.Parse(cmd13.ExecuteScalar().ToString());
                    cmd13.ExecuteNonQuery();

                    SqlCommand cmd8 = new SqlCommand("Update Person Set Gender = @gen Where Id = @id", con); ;
                    cmd8.Parameters.AddWithValue("@gen", genId);
                    cmd8.Parameters.AddWithValue("@id", id);
                    cmd8.ExecuteNonQuery();

                    SqlCommand cmd3 = new SqlCommand("Insert into Student values (@Id, @RegistrationNumber)", con);
                    cmd3.Parameters.AddWithValue("@Id", id);
                    cmd3.Parameters.AddWithValue("@RegistrationNumber", txtRNumber.Text);
                    cmd3.ExecuteNonQuery();

                    MessageBox.Show("Inserted Successfully");
                    goBackToHome();
                }
                else
                {
                    MessageBox.Show("First Name and Registration Number cannot be empty.");
                    return;
                }
            }
            catch
            {
                MessageBox.Show("Enter Data in Correct Form.\nRegistration Number(string)\nFirst Name(string)\nLast Name(string)\nContact(string)\nEmail(string)\nDate of Birth(Choose from DateTimePicker)\nGender(Choose from ComboBox)");
            }

            txtRNumber.Clear();
            txtFName.Clear();
            txtLName.Clear();
            txtContact.Clear();
            txtEmail.Clear();
            dateTimePicker1.Value = dateTimePicker1.MinDate;
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                var con = Configuration.getInstance().getConnection();

                SqlCommand cmd10 = new SqlCommand("Select Count(RegistrationNo) From Student Where @rNo = RegistrationNo", con);
                cmd10.Parameters.AddWithValue("@rNo", txtRNumber.Text);
                int isExist = int.Parse(cmd10.ExecuteScalar().ToString());
                cmd10.ExecuteNonQuery();

                SqlCommand cmd11 = new SqlCommand("Select Count(Email) From Person Where Email = @email", con);
                cmd11.Parameters.AddWithValue("@email", txtEmail.Text);
                int isExist2 = int.Parse(cmd11.ExecuteScalar().ToString());
                cmd11.ExecuteNonQuery();

                if (s.RegistrationNumber != txtRNumber.Text)
                {
                    if (isExist <= 0)
                    {
                        SqlCommand cmd = new SqlCommand("Update Student Set RegistrationNo = @rNo Where Id = @id", con);
                        cmd.Parameters.AddWithValue("@id", s.Id);
                        cmd.Parameters.AddWithValue("@rNo", txtRNumber.Text);
                        cmd.ExecuteNonQuery();
                    }
                    else
                    {
                        MessageBox.Show("Registration Number already exists.");
                        return;
                    }
                }

                if (txtEmail.Text != s.Email)
                {
                    if (isExist2 <= 0)
                    {
                        checkEmailFormat();

                        if (emailSymbols[0] && emailSymbols[1])
                        {
                            SqlCommand cmd4 = new SqlCommand("Update Person Set Email = @email Where Id = @id", con);
                            cmd4.Parameters.AddWithValue("@id", s.Id);
                            cmd4.Parameters.AddWithValue("@email", txtEmail.Text);
                            cmd4.ExecuteNonQuery();
                        }
                        else
                        {
                            MessageBox.Show("Email not in correct format.");
                        }
                    }
                    else
                    {
                        MessageBox.Show("Email already exists.");
                        return;
                    }
                }

                if (txtFName.Text != s.FirstName)
                {
                    SqlCommand cmd1 = new SqlCommand("Update Person Set FirstName = @name Where Id = @id", con);
                    cmd1.Parameters.AddWithValue("@id", s.Id);
                    cmd1.Parameters.AddWithValue("@name", txtFName.Text);
                    cmd1.ExecuteNonQuery();
                }

                if (txtLName.Text != s.LastName)
                {
                    SqlCommand cmd2 = new SqlCommand("Update Person Set LastName = @name Where Id = @id", con);
                    cmd2.Parameters.AddWithValue("@id", s.Id);
                    cmd2.Parameters.AddWithValue("@name", txtLName.Text);
                    cmd2.ExecuteNonQuery();
                }

                if (txtContact.Text != s.Contact)
                {
                    SqlCommand cmd3 = new SqlCommand("Update Person Set Contact = @contact Where Id = @id", con);
                    cmd3.Parameters.AddWithValue("@id", s.Id);
                    cmd3.Parameters.AddWithValue("@contact", txtContact.Text);
                    cmd3.ExecuteNonQuery();
                }

                SqlCommand cmdTemp = new SqlCommand("Select Id From Lookup Where Value = @gen", con);
                cmdTemp.Parameters.AddWithValue("@gen", cmbGender.SelectedValue.ToString());
                SqlDataAdapter da = new SqlDataAdapter(cmdTemp);
                DataTable dt = new DataTable();
                da.Fill(dt);
                int gender = int.Parse(dt.Rows[0]["Id"].ToString());
                cmdTemp.ExecuteNonQuery();

                if (gender != s.Gender)
                {
                    SqlCommand cmd5 = new SqlCommand("Update Person Set Gender = @gender Where Id = @id", con);
                    cmd5.Parameters.AddWithValue("@id", s.Id);
                    cmd5.Parameters.AddWithValue("@gender", gender);
                    cmd5.ExecuteNonQuery();
                }

                string date = dateTimePicker1.Text;

                if (date != s.Dob)
                {
                    SqlCommand cmd6 = new SqlCommand("Update Person Set DateOfBirth = @dob Where Id = @id", con);
                    cmd6.Parameters.AddWithValue("@id", s.Id);
                    cmd6.Parameters.AddWithValue("@dob", date);
                    cmd6.ExecuteNonQuery();
                }
            }
            catch
            {
                MessageBox.Show("Enter Data in Correct Form.\nRegistration Number(string)\nFirst Name(string)\nLast Name(string)\nContact(string)\nEmail(string)\nDate of Birth(Choose from DateTimePicker)\nGender(Choose from ComboBox)");
            }

            goBackToHome();
        }

        private void goBackToHome()
        {
            HomePageForm f = new HomePageForm();
            this.Hide();
            f.Show();
        }

        private void txtContact_TextChanged(object sender, EventArgs e)
        {
            int l = txtContact.Text.Length - 1;

            if(l >= 0 && txtContact.Text[l] != '0' && txtContact.Text[l] != '1' && txtContact.Text[l] != '2' && txtContact.Text[l] != '3' && txtContact.Text[l] != '4' && txtContact.Text[l] != '5' && txtContact.Text[l] != '6' && txtContact.Text[l] != '7' && txtContact.Text[l] != '8' && txtContact.Text[l] != '9')
            {
                MessageBox.Show("Only Digits are acceptable in Contact.");
                string temp = txtContact.Text.Remove(l, 1);
                txtContact.Text = temp;
            }
            if(l == 11)
            {
                MessageBox.Show("Maximum digits limit reached.");
                string temp = txtContact.Text.Remove(l, 1);
                txtContact.Text = temp;
            }
        }

        private void checkEmailFormat()
        {
            foreach (char letter in txtEmail.Text)
            {
                if (letter == '@')
                    emailSymbols[0] = true;
                if (letter == '.')
                    emailSymbols[1] = true;
            }
        }
    }
}