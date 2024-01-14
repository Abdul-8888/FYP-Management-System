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
    public partial class AssigningProjectForm : Form
    {
        private int pId;
        private int aId;
        private int gId;

        public AssigningProjectForm()
        {
            InitializeComponent();
            pId = 0;
            aId = 0;
            gId = 0;
        }

        private void AssigningProjectForm_Load(object sender, EventArgs e)
        {
            var con = Configuration.getInstance().getConnection();

            SqlCommand cmd3 = new SqlCommand("Select Value from Lookup Where Category = \'Advisor_Role\'", con);
            SqlDataAdapter da3 = new SqlDataAdapter(cmd3);
            DataTable dt3 = new DataTable();
            da3.Fill(dt3);
            cmbRole.DataSource = dt3;
            cmbRole.ValueMember = "Value";
            cmd3.ExecuteNonQuery();

            SqlCommand cmd = new SqlCommand("Select * From Project Where Id NOT IN (Select ProjectId From ProjectAdvisor) And Id NOT IN (Select ProjectId From GroupProject)", con);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            ProjectGridView.DataSource = dt;
            cmd.ExecuteNonQuery();

            SqlCommand cmd1 = new SqlCommand("Select * From Advisor Join Person On Advisor.Id = Person.Id", con);
            SqlDataAdapter da1 = new SqlDataAdapter(cmd1);
            DataTable dt1 = new DataTable();
            da1.Fill(dt1);
            AdvisorGridView.DataSource = dt1;
            cmd1.ExecuteNonQuery();

            SqlCommand cmd2 = new SqlCommand("Select GroupId, (Select RegistrationNo From Student Where StudentId = Id) As RegistrationNo, (Select [Value] From [Lookup] Where Id = Status) As Status, AssignmentDate From GroupStudent Where GroupId Not In (Select GroupId From GroupProject)", con);
            SqlDataAdapter da2 = new SqlDataAdapter(cmd2);
            DataTable dt2 = new DataTable();
            da2.Fill(dt2);
            GroupGridView.DataSource = dt2;
        }

        private void ProjectGridView_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 0)
            {
                DataGridViewRow row = this.ProjectGridView.Rows[e.RowIndex];

                pId = int.Parse(row.Cells["Id"].Value.ToString());
            }
        }

        private void AdvisorGridView_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 0)
            {
                DataGridViewRow row = this.AdvisorGridView.Rows[e.RowIndex];

                aId = int.Parse(row.Cells["Id"].Value.ToString());
            }
        }

        private void GroupGridView_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 0)
            {
                DataGridViewRow row = this.GroupGridView.Rows[e.RowIndex];

                gId = int.Parse(row.Cells["GroupId"].Value.ToString());
            }
        }

        private void btnInsert_Click(object sender, EventArgs e)
        {
            //try
            {
                string date = DateTime.Now.ToString("yyyy/MM/dd");

                var con = Configuration.getInstance().getConnection();

                if (pId != 0 && gId != 0)
                {
                    SqlCommand cmd10 = new SqlCommand("Select Count(ProjectId) From GroupProject Where @pId = ProjectId", con);
                    cmd10.Parameters.AddWithValue("@pId", pId);
                    int isExist = int.Parse(cmd10.ExecuteScalar().ToString());
                    cmd10.ExecuteNonQuery();

                    SqlCommand cmd11 = new SqlCommand("Select Count(GroupId) From GroupProject Where @gId = GroupId", con);
                    cmd11.Parameters.AddWithValue("@gId", gId);
                    int isExist1 = int.Parse(cmd11.ExecuteScalar().ToString());
                    cmd11.ExecuteNonQuery();

                    if (isExist > 0)
                    {
                        DialogResult result = MessageBox.Show("This Project is already assigned to another group. Do you want to assign this project to this group?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                        if (result == DialogResult.Yes)
                        {
                            if(isExist1 > 0)
                            {
                                DialogResult result1 = MessageBox.Show("This Group is already assigned to another project. Do you want to assign this project to this group?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                                if(result1 == DialogResult.Yes)
                                {
                                    SqlCommand cmd12 = new SqlCommand("Delete From GroupProject Where ProjectId = @pId", con);
                                    cmd12.Parameters.AddWithValue("@pId", pId);
                                    cmd12.ExecuteNonQuery();

                                    SqlCommand cmd13 = new SqlCommand("Delete From GroupProject Where ProjectId = @gId", con);
                                    cmd13.Parameters.AddWithValue("@gId", gId);
                                    cmd13.ExecuteNonQuery();

                                    SqlCommand cmd14 = new SqlCommand("Insert into GroupProject values (@pId, @gId, @date)", con);
                                    cmd14.Parameters.AddWithValue("@pId", pId);
                                    cmd14.Parameters.AddWithValue("@gId", gId);
                                    cmd14.Parameters.AddWithValue("@date", date);
                                    cmd14.ExecuteNonQuery();
                                }
                            }
                            else
                            {
                                SqlCommand cmd12 = new SqlCommand("Delete From GroupProject Where ProjectId = @pId", con);
                                cmd12.Parameters.AddWithValue("@pId", pId);
                                cmd12.ExecuteNonQuery();

                                SqlCommand cmd14 = new SqlCommand("Insert into GroupProject values (@pId, @gId, @date)", con);
                                cmd14.Parameters.AddWithValue("@pId", pId);
                                cmd14.Parameters.AddWithValue("@gId", gId);
                                cmd14.Parameters.AddWithValue("@date", date);
                                cmd14.ExecuteNonQuery();
                            }
                        }
                    }
                    else
                    {
                        if (isExist1 > 0)
                        {
                            DialogResult result1 = MessageBox.Show("This Group is already assigned to another project. Do you want to assign this project to this group?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                            if (result1 == DialogResult.Yes)
                            {
                                SqlCommand cmd13 = new SqlCommand("Delete From GroupProject Where GroupId = @gId", con);
                                cmd13.Parameters.AddWithValue("@gId", gId);
                                cmd13.ExecuteNonQuery();

                                SqlCommand cmd14 = new SqlCommand("Insert into GroupProject values (@pId, @gId, @date)", con);
                                cmd14.Parameters.AddWithValue("@pId", pId);
                                cmd14.Parameters.AddWithValue("@gId", gId);
                                cmd14.Parameters.AddWithValue("@date", date);
                                cmd14.ExecuteNonQuery();
                            }
                        }
                        else
                        {
                            SqlCommand cmd14 = new SqlCommand("Insert into GroupProject values (@pId, @gId, @date)", con);
                            cmd14.Parameters.AddWithValue("@pId", pId);
                            cmd14.Parameters.AddWithValue("@gId", gId);
                            cmd14.Parameters.AddWithValue("@date", date);
                            cmd14.ExecuteNonQuery();
                        }
                    }

                    MessageBox.Show("Task Completed.");
                }
                else
                {
                    MessageBox.Show("Project not assigned to a Group as some selections were missing.");
                }

                if (pId != 0 && 0 != aId)
                {
                    SqlCommand cmd2 = new SqlCommand("Select Id From Lookup Where Value = @Role", con);
                    cmd2.Parameters.AddWithValue("@Role", cmbRole.SelectedValue.ToString());
                    int role = int.Parse(cmd2.ExecuteScalar().ToString());
                    cmd2.ExecuteNonQuery();

                    SqlCommand cmd10 = new SqlCommand("Select Count(ProjectId) From ProjectAdvisor Where @pId = ProjectId And @aId = AdvisorId", con);
                    cmd10.Parameters.AddWithValue("@pId", pId);
                    cmd10.Parameters.AddWithValue("@aId", aId);
                    int isExist = int.Parse(cmd10.ExecuteScalar().ToString());
                    cmd10.ExecuteNonQuery();

                    SqlCommand cmd11 = new SqlCommand("Select Count(ProjectId) From ProjectAdvisor Where @pId = ProjectId And @role = AdvisorRole", con);
                    cmd11.Parameters.AddWithValue("@pId", pId);
                    cmd11.Parameters.AddWithValue("@role", role);
                    int isExist1 = int.Parse(cmd11.ExecuteScalar().ToString());
                    cmd11.ExecuteNonQuery();

                    if (isExist > 0)
                    {
                        DialogResult result = MessageBox.Show("This Project is already assigned with this Advisor. Do you want to change the Role of this Advisor?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                        if (result == DialogResult.Yes)
                        {
                            SqlCommand cmd13 = new SqlCommand("Delete From ProjectAdvisor Where ProjectId = @pId And AdvisorId = @aId", con);
                            cmd13.Parameters.AddWithValue("@pId", pId);
                            cmd13.Parameters.AddWithValue("@aId", aId);
                            cmd13.ExecuteNonQuery();

                            SqlCommand cmd1 = new SqlCommand("Insert into ProjectAdvisor values (@aId, @pId, @role, @date)", con);
                            cmd1.Parameters.AddWithValue("@aId", aId);
                            cmd1.Parameters.AddWithValue("@pId", pId);
                            cmd1.Parameters.AddWithValue("@role", role);
                            cmd1.Parameters.AddWithValue("@date", date);
                            cmd1.ExecuteNonQuery();
                        }
                    }
                    else if(isExist1 > 0)
                    {
                        DialogResult result = MessageBox.Show("This Project is already assigned with this Advisor Role. Do you want to change the Advisor for this Role?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                        if (result == DialogResult.Yes)
                        {
                            SqlCommand cmd13 = new SqlCommand("Delete From ProjectAdvisor Where ProjectId = @pId And AdvisorRole = @role", con);
                            cmd13.Parameters.AddWithValue("@pId", pId);
                            cmd13.Parameters.AddWithValue("@role", role);
                            cmd13.ExecuteNonQuery();

                            SqlCommand cmd1 = new SqlCommand("Insert into ProjectAdvisor values (@aId, @pId, @role, @date)", con);
                            cmd1.Parameters.AddWithValue("@aId", aId);
                            cmd1.Parameters.AddWithValue("@pId", pId);
                            cmd1.Parameters.AddWithValue("@role", role);
                            cmd1.Parameters.AddWithValue("@date", date);
                            cmd1.ExecuteNonQuery();
                        }
                    }
                    else
                    {

                        SqlCommand cmd1 = new SqlCommand("Insert into ProjectAdvisor values (@aId, @pId, @role, @date)", con);
                        cmd1.Parameters.AddWithValue("@aId", aId);
                        cmd1.Parameters.AddWithValue("@pId", pId);
                        cmd1.Parameters.AddWithValue("@role", role);
                        cmd1.Parameters.AddWithValue("@date", date);
                        cmd1.ExecuteNonQuery();
                    }

                    MessageBox.Show("Task Completed.");
                }
                else
                {
                    MessageBox.Show("Project not assigned to an Advisor as some selections were missing.");
                }
            }
            //catch
            {
                //MessageBox.Show("Select from all three tables to avoid any inconvenience.");
            }

            pId = 0;
            gId = 0;
            aId = 0;
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            HomePageForm f = new HomePageForm();
            f.Show();
            this.Hide();
        }
    }
}
