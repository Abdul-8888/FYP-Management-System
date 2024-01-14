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
    public partial class EvaluateGroupForm : Form
    {
        private GroupOfStudents g;
        private GroupEvaluation ge;

        public EvaluateGroupForm(GroupOfStudents g)
        {
            InitializeComponent();
            this.g = g;
            ge = null;
        }

        public EvaluateGroupForm(GroupEvaluation ge)
        {
            InitializeComponent();
            this.ge = ge;
        }

        private void EvaluateGroupForm_Load(object sender, EventArgs e)
        {
            dateTimePicker1.Value = dateTimePicker1.MinDate;

            var con = Configuration.getInstance().getConnection();
            SqlCommand cmd = new SqlCommand("Select Name from Evaluation", con);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            cmbName.DataSource = dt;
            cmbName.ValueMember = "Name";
            cmd.ExecuteNonQuery();

            if(ge != null)
            {
                SqlCommand cmd1 = new SqlCommand("Select Name From Evaluation Where Id = @id", con);
                cmd1.Parameters.AddWithValue("@id", ge.EvaluationId);
                string name = cmd1.ExecuteScalar().ToString();
                cmd1.ExecuteNonQuery();

                cmbName.SelectedItem = name;

                DateTime date = DateTime.Parse(ge.Date);
                dateTimePicker1.Value = date;

                txtMarks.Text = ge.ObtainedMarks.ToString();

                btnInsert.Hide();
            }
            else
            {
                btnUpdate.Hide();
            }
        }

        private void btnInsert_Click(object sender, EventArgs e)
        {
            string date = dateTimePicker1.Text;

            if (checkDate.Checked)
            {
                date = DateTime.Now.ToString("yyyy/MM/dd");
            }

            if(!checkWeightage())
            {
                return;
            }

            var con = Configuration.getInstance().getConnection();

            SqlCommand cmd2 = new SqlCommand("Select Id From Evaluation Where Name = @name", con);
            cmd2.Parameters.AddWithValue("@name", cmbName.SelectedValue.ToString());
            int id = int.Parse(cmd2.ExecuteScalar().ToString());
            cmd2.ExecuteNonQuery();

            SqlCommand cmd10 = new SqlCommand("Select Count(EvaluationId) From GroupEvaluation Where @gId = GroupId And EvaluationId = @eId", con);
            cmd10.Parameters.AddWithValue("@gId", g.Id);
            cmd10.Parameters.AddWithValue("@eId", id);
            int isExist = int.Parse(cmd10.ExecuteScalar().ToString());
            cmd10.ExecuteNonQuery();

            if(isExist > 0)
            {
                MessageBox.Show("This group is already marked for this Evaluation.");
                return;
            }

            SqlCommand cmd = new SqlCommand("Insert into GroupEvaluation values (@gId, @eId, @marks, @date)", con);
            cmd.Parameters.AddWithValue("@gId", g.Id);
            cmd.Parameters.AddWithValue("@eId", id);
            cmd.Parameters.AddWithValue("@marks", txtMarks.Text);
            cmd.Parameters.AddWithValue("@date", date);
            cmd.ExecuteNonQuery();

            goBackHome();
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            var con = Configuration.getInstance().getConnection();

            string date = dateTimePicker1.Text;

            SqlCommand cmdTemp = new SqlCommand("Select Id From Evaluation Where Name = @name", con);
            cmdTemp.Parameters.AddWithValue("@name", cmbName.SelectedValue.ToString());
            SqlDataAdapter da = new SqlDataAdapter(cmdTemp);
            DataTable dt = new DataTable();
            da.Fill(dt);
            int id = int.Parse(dt.Rows[0]["Id"].ToString());
            cmdTemp.ExecuteNonQuery();

            if (id != ge.EvaluationId)
            {
                SqlCommand cmd10 = new SqlCommand("Select Count(EvaluationId) From GroupEvaluation Where @gId = GroupId And EvaluationId = @eId", con);
                cmd10.Parameters.AddWithValue("@gId", g.Id);
                cmd10.Parameters.AddWithValue("@eId", id);
                int isExist = int.Parse(cmd10.ExecuteScalar().ToString());
                cmd10.ExecuteNonQuery();

                if (isExist > 0)
                {
                    MessageBox.Show("This group is already marked for this Evaluation.");
                    return;
                }

                SqlCommand cmd = new SqlCommand("Update GroupEvaluation Set EvaluationId = @eId Where GroupId = @gId", con);
                cmd.Parameters.AddWithValue("@gId", ge.GroupId);
                cmd.Parameters.AddWithValue("@eId", id);
                cmd.ExecuteNonQuery();
            }

            if (txtMarks.Text != ge.ObtainedMarks.ToString())
            {
                SqlCommand cmd = new SqlCommand("Update GroupEvaluation Set ObtainedMarks = @marks Where GroupId = @gId", con);
                cmd.Parameters.AddWithValue("@gId", ge.GroupId);
                cmd.Parameters.AddWithValue("@marks", txtMarks.Text);
                cmd.ExecuteNonQuery();
            }

            if (dateTimePicker1.Text != date)
            {
                SqlCommand cmd1 = new SqlCommand("Update GroupEvaluation Set EvaluationDate = @date Where GroupId = @gId", con);
                cmd1.Parameters.AddWithValue("@gId", ge.GroupId);
                cmd1.Parameters.AddWithValue("@date", date);
                cmd1.ExecuteNonQuery();
            }

            goBackHome();
        }

        private void goBackHome()
        {
            HomePageForm f = new HomePageForm();
            this.Hide();
            f.Show();
        }

        private void txtMarks_TextChanged(object sender, EventArgs e)
        {
            int l = txtMarks.Text.Length - 1;

            if (l >= 0 && txtMarks.Text[l] != '0' && txtMarks.Text[l] != '1' && txtMarks.Text[l] != '2' && txtMarks.Text[l] != '3' && txtMarks.Text[l] != '4' && txtMarks.Text[l] != '5' && txtMarks.Text[l] != '6' && txtMarks.Text[l] != '7' && txtMarks.Text[l] != '8' && txtMarks.Text[l] != '9')
            {
                MessageBox.Show("Only Digits are acceptable in Marks.");
                string temp = txtMarks.Text.Remove(l, 1);
                txtMarks.Text = temp;
            }
        }

        private bool checkWeightage()
        {
            var con = Configuration.getInstance().getConnection();

            SqlCommand cmdTemp = new SqlCommand("Select TotalWeightage From Evaluation Where Name = @gen", con);
            cmdTemp.Parameters.AddWithValue("@gen", cmbName.SelectedValue.ToString());
            SqlDataAdapter da = new SqlDataAdapter(cmdTemp);
            DataTable dt = new DataTable();
            da.Fill(dt);
            int totalMarks = int.Parse(dt.Rows[0]["TotalWeightage"].ToString());
            cmdTemp.ExecuteNonQuery();

            if (int.Parse(txtMarks.Text) > totalMarks)
            {
                MessageBox.Show("Obtained Marks cannot be more than total marks. Total Marks are " + totalMarks.ToString());
                txtMarks.Clear();
                return false;
            }

            return true;
        }
    }
}
