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
    public partial class InsertAdvisorForm : Form
    {
        public Advisor a;
        private bool[] emailSymbols;
        
        public InsertAdvisorForm(Advisor a)
        {
            InitializeComponent();
            this.a = a;
            emailSymbols = new bool[2];
        }

        private void InsertAdvisorForm_Load(object sender, EventArgs e)
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

            SqlCommand cmd1 = new SqlCommand("Select Value from Lookup Where Category = \'Designation\'", con);
            SqlDataAdapter da1 = new SqlDataAdapter(cmd1);
            DataTable dt1 = new DataTable();
            da1.Fill(dt1);
            cmbDesignation.DataSource = dt1;
            cmbDesignation.ValueMember = "Value";
            cmd1.ExecuteNonQuery();

            if (a.FirstName != "")
            {
                emailSymbols[0] = true;
                emailSymbols[1] = true;

                SqlCommand cmd2 = new SqlCommand("Select Value from Lookup Where Id = @id", con);
                cmd2.Parameters.AddWithValue("@id", a.Gender);

                string gender = (string)cmd2.ExecuteScalar();
                cmd2.ExecuteNonQuery();

                SqlCommand cmd3 = new SqlCommand("Select Value from Lookup Where Id = @id", con);
                cmd3.Parameters.AddWithValue("@id", a.Designation);

                string designation = (string)cmd3.ExecuteScalar();
                cmd3.ExecuteNonQuery();

                txtFirstName.Text = a.FirstName;
                txtLastName.Text = a.LastName;
                txtContact.Text = a.Contact.ToString();
                txtEmail.Text = a.Email;
                cmbDesignation.SelectedValue = designation;
                cmbGender.SelectedValue = gender;

                DateTime dob = DateTime.Parse(a.Dob.ToString());
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
                if (!emailSymbols[0] && !emailSymbols[1])
                {
                    MessageBox.Show("Enter email in correct format.");
                    return;
                }

                if (isExist > 0)
                {
                    MessageBox.Show("Email already exists.");
                    return;
                }

                if (txtFirstName.Text != "")
                {
                    SqlCommand cmd6 = new SqlCommand("Insert into Person (FirstName, Email) values (@FirstName, @Email)", con);
                    cmd6.Parameters.AddWithValue("@FirstName", txtFirstName.Text);
                    cmd6.Parameters.AddWithValue("@Email", txtEmail.Text);
                    cmd6.ExecuteNonQuery();

                    SqlCommand cmd2 = new SqlCommand("Select Max(Id) From Person", con);
                    int id = int.Parse(cmd2.ExecuteScalar().ToString());
                    cmd2.ExecuteNonQuery();

                    if (txtLastName.Text != "")
                    {
                        SqlCommand cmd9 = new SqlCommand("Update Person Set LastName = @LastName Where Id = @id", con);
                        cmd9.Parameters.AddWithValue("@LastName", txtLastName.Text);
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

                    SqlCommand cmd17 = new SqlCommand("Select Id From Lookup Where Value = @gen", con);
                    cmd17.Parameters.AddWithValue("@gen", cmbGender.SelectedValue.ToString());
                    int genId = int.Parse(cmd17.ExecuteScalar().ToString());
                    cmd17.ExecuteNonQuery();

                    SqlCommand cmd8 = new SqlCommand("Update Person Set Gender = @gen Where Id = @id", con); ;
                    cmd8.Parameters.AddWithValue("@gen", genId);
                    cmd8.Parameters.AddWithValue("@id", id);
                    cmd8.ExecuteNonQuery();

                    SqlCommand cmd13 = new SqlCommand("Select Id From Lookup Where Value = @des", con);
                    cmd13.Parameters.AddWithValue("@des", cmbDesignation.SelectedValue.ToString());
                    SqlDataAdapter da13 = new SqlDataAdapter(cmd13);
                    DataTable dt13 = new DataTable();
                    da13.Fill(dt13);
                    int desId = int.Parse(dt13.Rows[0]["Id"].ToString());
                    cmd13.ExecuteNonQuery();

                    SqlCommand cmd3 = new SqlCommand("Insert into Advisor (Id, Designation) values (@Id, @Designation)", con);
                    cmd3.Parameters.AddWithValue("@Id", id);
                    cmd3.Parameters.AddWithValue("@Designation", desId);
                    cmd3.ExecuteNonQuery();

                    if(txtSalary.Text != "")
                    {
                        SqlCommand cmd12 = new SqlCommand("Update Advisor Set Salary = @Salary Where Id = @id", con); ;
                        cmd12.Parameters.AddWithValue("@Salary", txtSalary.Text);
                        cmd12.Parameters.AddWithValue("@id", id);
                        cmd12.ExecuteNonQuery();
                    }

                    MessageBox.Show("Inserted Successfully");
                    goBackToHome();
                }
                else
                {
                    MessageBox.Show("First Name and Email cannot be empty.");
                    return;
                }
            }
            catch
            {
                MessageBox.Show("Enter Data in Correct Form.\nFirst Name(string)\nLast Name(string)\nContact(string)\nEmail(string)\nDate of Birth(Choose from DateTimePicker)\nGender(Choose from ComboBox)\nDesignation(Choose from ComboBox)\nSalary(float).");
            }

            txtFirstName.Clear();
            txtLastName.Clear();
            txtContact.Clear();
            txtEmail.Clear();
            dateTimePicker1.Value = dateTimePicker1.MinDate;
            txtSalary.Clear();
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                var con = Configuration.getInstance().getConnection();

                SqlCommand cmd11 = new SqlCommand("Select Count(Email) From Person Where Email = @email", con);
                cmd11.Parameters.AddWithValue("@email", txtEmail.Text);
                int isExist2 = int.Parse(cmd11.ExecuteScalar().ToString());
                cmd11.ExecuteNonQuery();

                if (txtEmail.Text != a.Email)
                {
                    if (isExist2 <= 0)
                    {
                        checkEmailFormat();

                        if (emailSymbols[0] && emailSymbols[1])
                        {
                            SqlCommand cmd4 = new SqlCommand("Update Person Set Email = @email Where Id = @id", con);
                            cmd4.Parameters.AddWithValue("@id", a.Id);
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

                if (txtFirstName.Text != a.FirstName)
                {
                    SqlCommand cmd1 = new SqlCommand("Update Person Set FirstName = @name Where Id = @id", con);
                    cmd1.Parameters.AddWithValue("@id", a.Id);
                    cmd1.Parameters.AddWithValue("@name", txtFirstName.Text);
                    cmd1.ExecuteNonQuery();
                }

                if (txtLastName.Text != a.LastName)
                {
                    SqlCommand cmd2 = new SqlCommand("Update Person Set LastName = @name Where Id = @id", con);
                    cmd2.Parameters.AddWithValue("@id", a.Id);
                    cmd2.Parameters.AddWithValue("@name", txtLastName.Text);
                    cmd2.ExecuteNonQuery();
                }

                if (txtContact.Text != a.Contact)
                {
                    SqlCommand cmd3 = new SqlCommand("Update Person Set Contact = @contact Where Id = @id", con);
                    cmd3.Parameters.AddWithValue("@id", a.Id);
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

                if (gender != a.Gender)
                {
                    SqlCommand cmd5 = new SqlCommand("Update Person Set Gender = @gender Where Id = @id", con);
                    cmd5.Parameters.AddWithValue("@id", a.Id);
                    cmd5.Parameters.AddWithValue("@gender", gender);
                    cmd5.ExecuteNonQuery();
                }

                string date = dateTimePicker1.Text;
                
                if (date != a.Dob)
                {
                    SqlCommand cmd6 = new SqlCommand("Update Person Set DateOfBirth = @dob Where Id = @id", con);
                    cmd6.Parameters.AddWithValue("@id", a.Id);
                    cmd6.Parameters.AddWithValue("@dob", date);
                    cmd6.ExecuteNonQuery();
                }

                SqlCommand cmdTemp1 = new SqlCommand("Select Id From Lookup Where Value = @des", con);
                cmdTemp1.Parameters.AddWithValue("@des", cmbDesignation.SelectedValue.ToString());
                SqlDataAdapter da1 = new SqlDataAdapter(cmdTemp1);
                DataTable dt1 = new DataTable();
                da1.Fill(dt1);
                int designation = int.Parse(dt.Rows[0]["Id"].ToString());
                cmdTemp1.ExecuteNonQuery();

                if (a.Designation != designation)
                {
                    SqlCommand cmd7 = new SqlCommand("Update Advisor Set Designation = @designation Where Id = @id", con);
                    cmd7.Parameters.AddWithValue("@id", a.Id);
                    cmd7.Parameters.AddWithValue("@designation", designation);
                    cmd7.ExecuteNonQuery();
                }

                if (a.Salary.ToString() != txtSalary.Text)
                {
                    SqlCommand cmd8 = new SqlCommand("Update Advisor Set Salary = @sal Where Id = @id", con);
                    cmd8.Parameters.AddWithValue("@id", a.Id);
                    cmd8.Parameters.AddWithValue("@sal", txtSalary.Text);
                    cmd8.ExecuteNonQuery();
                }
            }
            catch
            {
                MessageBox.Show("Enter Data in Correct Form.\nFirst Name(string)\nLast Name(string)\nContact(string)\nEmail(string)\nDate of Birth(Choose from DateTimePicker)\nGender(Choose from ComboBox)\nDesignation(Choose from ComboBox)\nSalary(float).");
            }

            goBackToHome();
        }

        private void goBackToHome()
        {
            HomePageForm f = new HomePageForm();
            this.Hide();
            f.Show();
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

        private void txtContact_TextChanged(object sender, EventArgs e)
        {
            int l = txtContact.Text.Length - 1;

            if (l >= 0 && txtContact.Text[l] != '0' && txtContact.Text[l] != '1' && txtContact.Text[l] != '2' && txtContact.Text[l] != '3' && txtContact.Text[l] != '4' && txtContact.Text[l] != '5' && txtContact.Text[l] != '6' && txtContact.Text[l] != '7' && txtContact.Text[l] != '8' && txtContact.Text[l] != '9')
            {
                MessageBox.Show("Only Digits are acceptable in Contact.");
                string temp = txtContact.Text.Remove(l, 1);
                txtContact.Text = temp;
            }
            if (l == 11)
            {
                MessageBox.Show("Maximum digits limit reached.");
                string temp = txtContact.Text.Remove(l, 1);
                txtContact.Text = temp;
            }
        }

        private void txtSalary_TextChanged(object sender, EventArgs e)
        {
            int l = txtContact.Text.Length - 1;

            if (l >= 0 && txtContact.Text[l] != '0' && txtContact.Text[l] != '1' && txtContact.Text[l] != '2' && txtContact.Text[l] != '3' && txtContact.Text[l] != '4' && txtContact.Text[l] != '5' && txtContact.Text[l] != '6' && txtContact.Text[l] != '7' && txtContact.Text[l] != '8' && txtContact.Text[l] != '9')
            {
                MessageBox.Show("Only Digits are acceptable in Salary.");
                string temp = txtContact.Text.Remove(l, 1);
                txtContact.Text = temp;
            }
        }
    }
}
