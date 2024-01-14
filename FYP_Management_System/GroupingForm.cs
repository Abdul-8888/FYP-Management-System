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
    public partial class GroupingForm : Form
    {
        private GroupOfStudents g;

        public GroupingForm(GroupOfStudents g)
        {
            InitializeComponent();
            this.g = g;
        }

        private void GroupingForm_Load(object sender, EventArgs e)
        {
            if(g.Id != 0)
            {
                showInGrid();
            }
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            var con = Configuration.getInstance().getConnection();
            SqlCommand cmd10 = new SqlCommand("Select Count(StudentId) From GroupStudent Where GroupId = @gId", con);
            cmd10.Parameters.AddWithValue("@gId", g.Id);
            int isExist = int.Parse(cmd10.ExecuteScalar().ToString());
            cmd10.ExecuteNonQuery();

            if(isExist == 0)
            {
                SqlCommand cmd1 = new SqlCommand("Delete From [Group] Where Id = @gId", con);
                cmd1.Parameters.AddWithValue("@gId", g.Id);
                cmd1.ExecuteNonQuery();
            }

            goBackHome();
        }

        private void goBackHome()
        {
            HomePageForm f = new HomePageForm();
            f.Show();
            this.Hide();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var con = Configuration.getInstance().getConnection();

            SqlCommand cmd2 = new SqlCommand("Select Id From Lookup Where Value = \'Active\'", con);
            int status = int.Parse(cmd2.ExecuteScalar().ToString());
            cmd2.ExecuteNonQuery();

            SqlCommand cmd10 = new SqlCommand("Select Count(StudentId) From GroupStudent Where GroupId = @gId And Status = @status", con);
            cmd10.Parameters.AddWithValue("@gId", g.Id);
            cmd10.Parameters.AddWithValue("@status", status);
            int isExist = int.Parse(cmd10.ExecuteScalar().ToString());
            cmd10.ExecuteNonQuery();

            if (isExist >= 4)
            {
                MessageBox.Show("Group is filled completely. Cannot accommodate anymore students.");
                return;
            }

            SqlCommand cmd5 = new SqlCommand("Select Count(Id) From Student Where RegistrationNo = @rNo", con);
            cmd5.Parameters.AddWithValue("@rNo", txtRNumber.Text);
            isExist = int.Parse(cmd5.ExecuteScalar().ToString());
            cmd5.ExecuteNonQuery();

            if(isExist == 0)
            {
                MessageBox.Show("Invalid Registration Number.");
                return;
            }

            SqlCommand cmd = new SqlCommand("Select Id From Student Where RegistrationNo = @rNumber", con);
            cmd.Parameters.AddWithValue("@rNumber", txtRNumber.Text);
            int id = int.Parse(cmd.ExecuteScalar().ToString());
            cmd.ExecuteNonQuery();

            SqlCommand cmd6 = new SqlCommand("Select Count(StudentId) From GroupStudent Where StudentId = @sId And Status = @status", con);
            cmd6.Parameters.AddWithValue("@sId", id);
            cmd6.Parameters.AddWithValue("@status", status);
            isExist = int.Parse(cmd6.ExecuteScalar().ToString());
            cmd6.ExecuteNonQuery();

            if (isExist > 0)
            {
                MessageBox.Show("Student is already in a group. Remove him/her from the previous group first.");
                return;
            }

            SqlCommand cmd1 = new SqlCommand("Insert into GroupStudent values (@groupId, @studentId, @status, @assignmentDate)", con);
            cmd1.Parameters.AddWithValue("@groupId", g.Id);
            cmd1.Parameters.AddWithValue("@studentId", id);
            cmd1.Parameters.AddWithValue("@status", status);
            cmd1.Parameters.AddWithValue("@assignmentDate", DateTime.Now.ToString("yyyy/MM/dd"));
            cmd1.ExecuteNonQuery();

            showInGrid();

            txtRNumber.Clear();
        }

        private void showInGrid()
        {
            var con = Configuration.getInstance().getConnection();

            SqlCommand cmd = new SqlCommand("Select GroupId, (Select RegistrationNo From Student Where StudentId = Id) As RegistrationNo, (Select [Value] From [Lookup] Where Id = Status) As Status, AssignmentDate From GroupStudent Where GroupId = @gId", con);
            cmd.Parameters.AddWithValue("@gId", g.Id);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            dataGridView1.DataSource = dt;
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if(e.ColumnIndex == 0)
            {
                DataGridViewRow row = this.dataGridView1.Rows[e.RowIndex];

                var con = Configuration.getInstance().getConnection();

                string regNo = row.Cells["RegistrationNo"].Value.ToString();

                SqlCommand cmd = new SqlCommand("Select Id From Student Where RegistrationNo = @rNo", con);
                cmd.Parameters.AddWithValue("@rNo", regNo);
                int id = int.Parse(cmd.ExecuteScalar().ToString());
                cmd.ExecuteNonQuery();

                SqlCommand cmd2 = new SqlCommand("Select Id From Lookup Where Value = \'InActive\'", con);
                int status = int.Parse(cmd2.ExecuteScalar().ToString());
                cmd2.ExecuteNonQuery();

                SqlCommand cmd3 = new SqlCommand("Update GroupStudent Set Status = @status Where StudentId = @sId And GroupId = @gId", con);
                cmd3.Parameters.AddWithValue("@sId", id);
                cmd3.Parameters.AddWithValue("@status", status);
                cmd3.Parameters.AddWithValue("@gId", g.Id);
                cmd3.ExecuteNonQuery();
            }

            showInGrid();
        }
    }
}
