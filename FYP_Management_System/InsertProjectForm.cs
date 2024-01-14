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
    public partial class InsertProjectForm : Form
    {
        private Project p;

        public InsertProjectForm(Project p)
        {
            InitializeComponent();
            this.p = p;
        }

        private void InsertProjectForm_Load(object sender, EventArgs e)
        {
            if (p.Title != "")
            {
                txtTitle.Text = p.Title;
                txtDescription.Text = p.Description;

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
                SqlCommand cmd = new SqlCommand("Select Count(Title) From Project Where @Title = Title", con);
                cmd.Parameters.AddWithValue("@Title", txtTitle.Text);

                int isExist = (int)cmd.ExecuteScalar();

                if (isExist > 0)
                {
                    MessageBox.Show("Project Title already Exists.");
                    return;
                }

                else
                {
                    if (txtTitle.Text != "")
                    {
                        SqlCommand cmd1 = new SqlCommand("Insert into Project (Title) values (@Title)", con);
                        cmd1.Parameters.AddWithValue("@Title", txtTitle.Text);
                        cmd1.ExecuteNonQuery();
                    }
                    else
                    {
                        MessageBox.Show("Title of Project is Missing");
                        return;
                    }

                    if (txtDescription.Text != "")
                    {
                        SqlCommand cmd1 = new SqlCommand("Update Project Set Description = @Description Where Title = @title", con);
                        cmd1.Parameters.AddWithValue("@Description", txtDescription.Text);
                        cmd1.Parameters.AddWithValue("@Title", txtTitle.Text);
                        cmd1.ExecuteNonQuery();
                    }
                }
            }
            catch
            {
                MessageBox.Show("Enter Project Title and Description Correctly.");
            }

            MessageBox.Show("Request Successfully Completed.");
            goBackToHome();
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                var con = Configuration.getInstance().getConnection();
                
                if (p.Title != txtTitle.Text)
                {
                    SqlCommand cmd10 = new SqlCommand("Select Count(Title) From Project Where @Title = Title", con);
                    cmd10.Parameters.AddWithValue("@Title", txtTitle.Text);

                    int isExist = int.Parse(cmd10.ExecuteScalar().ToString());

                    if (isExist > 0)
                    {
                        MessageBox.Show("Project Title already Exists.");
                        return;
                    }

                    SqlCommand cmd = new SqlCommand("Update Project Set Title = @title Where Id = @id", con);
                    cmd.Parameters.AddWithValue("@id", p.Id);
                    cmd.Parameters.AddWithValue("@title", txtTitle.Text);
                    cmd.ExecuteNonQuery();
                }

                if (p.Description != txtDescription.Text)
                {
                    SqlCommand cmd1 = new SqlCommand("Update Project Set Description = @Description Where Id = @id", con);
                    cmd1.Parameters.AddWithValue("@id", p.Id);
                    cmd1.Parameters.AddWithValue("@description", txtDescription.Text);
                    cmd1.ExecuteNonQuery();
                }
            }
            catch
            {
                MessageBox.Show("Enter Project Title and Description Correctly.");
            }

            MessageBox.Show("Request Successfully Completed.");
            goBackToHome();
        }

        private void goBackToHome()
        {
            HomePageForm f = new HomePageForm();
            this.Hide();
            f.Show();
        }
    }
}
