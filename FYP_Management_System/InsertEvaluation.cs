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
    public partial class InsertEvaluation : Form
    {
        private Evaluation eval;

        public InsertEvaluation(Evaluation eval)
        {
            InitializeComponent();
            this.eval = eval;
        }

        private void InsertEvaluation_Load(object sender, EventArgs e)
        {
            if (eval.Name != "")
            {
                txtName.Text = eval.Name;
                txtMarks.Text = eval.TotalMarks.ToString();
                txtWeightage.Text = eval.TotalWeightage.ToString();

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
                SqlCommand cmd4 = new SqlCommand("Select Count(Name) From Evaluation Where @name = Name", con);
                cmd4.Parameters.AddWithValue("@name", txtName.Text);
                int isExist = int.Parse(cmd4.ExecuteScalar().ToString());
                cmd4.ExecuteNonQuery();

                if (isExist > 0)
                {
                    MessageBox.Show("Evaluation with this name already exists.");
                    return;
                }

                if (txtName.Text != "" && txtMarks.Text != "" && txtWeightage.Text != "")
                {
                    SqlCommand cmd = new SqlCommand("Insert into Evaluation values (@Name, @TotalMarks, @TotalWeightage)", con);
                    cmd.Parameters.AddWithValue("@Name", txtName.Text);
                    cmd.Parameters.AddWithValue("TotalMarks", int.Parse(txtMarks.Text));
                    cmd.Parameters.AddWithValue("@TotalWeightage", int.Parse(txtWeightage.Text));
                    cmd.ExecuteNonQuery();
                }
                else
                {
                    MessageBox.Show("Fill all the fields.");
                }
            }
            catch
            {
                MessageBox.Show("Enter data in correct format:\n Name(string)\n Total Marks(int)\n Total Weightage(int).");
            }

            MessageBox.Show("SUCCESS.");
            goBackToHome();
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                var con = Configuration.getInstance().getConnection();

                if (eval.Name != txtName.Text)
                {
                    SqlCommand cmd4 = new SqlCommand("Select Count(Name) From Evaluation Where @name = Name", con);
                    cmd4.Parameters.AddWithValue("@name", txtName.Text);
                    int isExist = int.Parse(cmd4.ExecuteScalar().ToString());
                    cmd4.ExecuteNonQuery();

                    if (isExist > 0)
                    {
                        MessageBox.Show("Evaluation with this name already exists.");
                        return;
                    }

                    SqlCommand cmd = new SqlCommand("Update Evaluation Set Name = @name Where Id = @id", con);
                    cmd.Parameters.AddWithValue("@id", eval.Id);
                    cmd.Parameters.AddWithValue("@name", txtName.Text);
                    cmd.ExecuteNonQuery();
                }

                if (eval.TotalMarks.ToString() != txtMarks.Text)
                {
                    SqlCommand cmd1 = new SqlCommand("Update Evaluation Set TotalMarks = @totalMarks Where Id = @id", con);
                    cmd1.Parameters.AddWithValue("@id", eval.Id);
                    cmd1.Parameters.AddWithValue("@totalMarks", txtMarks.Text);
                    cmd1.ExecuteNonQuery();
                }

                if (eval.TotalWeightage.ToString() != txtWeightage.Text)
                {
                    SqlCommand cmd2 = new SqlCommand("Update Evaluation Set TotalWeightage = @totalWeightage Where Id = @id", con);
                    cmd2.Parameters.AddWithValue("@id", eval.Id);
                    cmd2.Parameters.AddWithValue("@totalWeightage", txtWeightage.Text);
                    cmd2.ExecuteNonQuery();
                }
            }
            catch
            {
                MessageBox.Show("Enter data in correct format:\n Name(string)\n Total Marks(int)\n Total Weightage(int).");
            }

            MessageBox.Show("SUCCESS.");
            goBackToHome();
        }

        private void txtMarks_TextChanged(object sender, EventArgs e)
        {
            int l = txtMarks.Text.Length - 1;

            if (l >= 0 && txtMarks.Text[l] != '0' && txtMarks.Text[l] != '1' && txtMarks.Text[l] != '2' && txtMarks.Text[l] != '3' && txtMarks.Text[l] != '4' && txtMarks.Text[l] != '5' && txtMarks.Text[l] != '6' && txtMarks.Text[l] != '7' && txtMarks.Text[l] != '8' && txtMarks.Text[l] != '9')
            {
                MessageBox.Show("Only Digits are acceptable as Marks.");
                string temp = txtMarks.Text.Remove(l, 1);
                txtMarks.Text = temp;
            }
        }

        private void txtWeightage_TextChanged(object sender, EventArgs e)
        {
            int l = txtWeightage.Text.Length - 1;

            if (l >= 0 && txtWeightage.Text[l] != '0' && txtWeightage.Text[l] != '1' && txtWeightage.Text[l] != '2' && txtWeightage.Text[l] != '3' && txtWeightage.Text[l] != '4' && txtWeightage.Text[l] != '5' && txtWeightage.Text[l] != '6' && txtWeightage.Text[l] != '7' && txtWeightage.Text[l] != '8' && txtWeightage.Text[l] != '9')
            {
                MessageBox.Show("Only Digits are acceptable in Weightage.");
                string temp = txtWeightage.Text.Remove(l, 1);
                txtWeightage.Text = temp;
            }

            if( int.Parse(txtWeightage.Text) > 100)
            {
                MessageBox.Show("Weightage cannot be more than 100.");
                string temp = txtWeightage.Text.Remove(l, 1);
                txtWeightage.Text = temp;
            }
        }

        private void goBackToHome()
        {
            HomePageForm f = new HomePageForm();
            this.Hide();
            f.Show();
        }
    }
}
