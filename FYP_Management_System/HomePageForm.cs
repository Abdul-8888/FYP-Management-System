using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using FYP_Management_System.BL;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace FYP_Management_System
{
    public partial class HomePageForm : Form
    {
        string selectedOption;

        public HomePageForm()
        {
            InitializeComponent();
        }

        private void HomePageForm_Load(object sender, EventArgs e)
        {

        }

        private void cmbMenu_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbMenu.Text == "Add Student")
            {
                Student s = new Student();
                InsertStudentForm f = new InsertStudentForm(s);
                f.Show();
                this.Hide();
            }
            else if (cmbMenu.Text == "Add Advisor")
            {
                Advisor a = new Advisor();
                InsertAdvisorForm f = new InsertAdvisorForm(a);
                f.Show();
                this.Hide();
            }
            else if (cmbMenu.Text == "Add Project")
            {
                Project p = new Project();
                InsertProjectForm f = new InsertProjectForm(p);
                f.Show();
                this.Hide();
            }
            else if (cmbMenu.Text == "Add Evaluation")
            {
                Evaluation eval = new Evaluation();
                InsertEvaluation f = new InsertEvaluation(eval);
                f.Show();
                this.Hide();
            }

            if (cmbMenu.Text == "Retrieve Students")
            {
                var con = Configuration.getInstance().getConnection();
                SqlCommand cmd = new SqlCommand("Select * From Student Join Person On Student.Id = Person.Id", con);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);
                dataGrid.DataSource = dt;

                selectedOption = "Student";
            }
            else if (cmbMenu.Text == "Retrieve Advisors")
            {
                var con = Configuration.getInstance().getConnection();
                SqlCommand cmd = new SqlCommand("Select * From Advisor Join Person On Advisor.Id = Person.Id", con);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);
                dataGrid.DataSource = dt;

                selectedOption = "Advisor";
            }
            else if (cmbMenu.Text == "Retrieve Projects")
            {
                var con = Configuration.getInstance().getConnection();
                SqlCommand cmd = new SqlCommand("Select * From Project", con);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);
                dataGrid.DataSource = dt;

                selectedOption = "Project";
            }
            else if (cmbMenu.Text == "Retrieve Evaluations")
            {
                var con = Configuration.getInstance().getConnection();
                SqlCommand cmd = new SqlCommand("Select * From Evaluation", con);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);
                dataGrid.DataSource = dt;

                selectedOption = "Evaluation";
            }
            else if(cmbMenu.Text == "Create Group")
            {
                var con = Configuration.getInstance().getConnection();

                SqlCommand cmd = new SqlCommand("Insert into [Group] values (@date)", con);
                cmd.Parameters.AddWithValue("@date", DateTime.Now.ToString("yyyy/MM/dd"));
                cmd.ExecuteNonQuery();

                SqlCommand cmd1 = new SqlCommand("Select Top(1) Id From [Group] Order By Id DESC", con);
                int id = int.Parse(cmd1.ExecuteScalar().ToString());
                cmd1.ExecuteNonQuery();

                GroupOfStudents g = new GroupOfStudents(id);
                GroupingForm f = new GroupingForm(g);
                f.Show();
                this.Hide();
            }
            else if(cmbMenu.Text == "Retrieve Groups")
            {
                var con = Configuration.getInstance().getConnection();
                SqlCommand cmd = new SqlCommand("Select GroupId, (Select RegistrationNo From Student Where StudentId = Id) As RegistrationNo, Status, AssignmentDate From GroupStudent", con);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);
                dataGrid.DataSource = dt;

                selectedOption = "Group";
            }
            else if (cmbMenu.Text == "Evaluate Group")
            {
                var con = Configuration.getInstance().getConnection();
                SqlCommand cmd = new SqlCommand("SELECT GroupId, (Select Title From Project Where ProjectId = Id) As Project FROM GroupProject", con);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);
                dataGrid.DataSource = dt;

                selectedOption = "EvaluateGroup";
            }
            else if (cmbMenu.Text == "Assign Project")
            {
                AssigningProjectForm f = new AssigningProjectForm();
                f.Show();
                this.Hide();
            }
            else if(cmbMenu.Text == "Get Assigned Projects, Groups, Advisors")
            {
                var con = Configuration.getInstance().getConnection();
                SqlCommand cmd = new SqlCommand(" Select g.ProjectId, p.Title, p.Description, g.GroupId, (Select FirstName From Person Where Id = a.AdvisorId) As Advisor, (Select Value From Lookup wHERE Id = a.AdvisorRole) As AdvisorRole From Project As p Left Outer Join GroupProject As g On ProjectId = Id Left Outer Join ProjectAdvisor As a On a.ProjectId = p.Id", con);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);
                dataGrid.DataSource = dt;
            }

            else if(cmbMenu.Text == "Retrieve Marks of Groups")
            {
                var con = Configuration.getInstance().getConnection();
                SqlCommand cmd = new SqlCommand("SELECT [GroupId], [EvaluationId] ,[ObtainedMarks] ,[EvaluationDate] FROM [ProjectA].[dbo].[GroupEvaluation]", con);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);
                dataGrid.DataSource = dt;

                selectedOption = "Marks of Groups";
            }
            else if (cmbMenu.Text == "Report for Projects, Students, Advisors")
            {
                string path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "/ProjectAdvStudentsReport.pdf";
                Document DC = new Document();
                FileStream FS = File.Create(path);
                PdfWriter.GetInstance(DC, FS);
                DC.Open();
                DC.Add(new Paragraph(""));

                var c = Configuration.getInstance().getConnection();
                SqlCommand sc = new SqlCommand("Select p.Id as Id, p.Title as Title, (Select FirstName From Person Where pa.AdvisorId = Id) As AdvisorName, (Select Value From Lookup Where Id = pa.AdvisorRole) As AdvisorRole, (Select RegistrationNo From Student Where Id = gs.StudentId) As RegistrationNumber, (Select FirstName From Person Where Id = gs.StudentId) As StudentName From Project As p Full Outer Join ProjectAdvisor As pa On p.Id = pa.ProjectId Full Outer Join GroupProject as gp On gp.ProjectId = p.Id Join GroupStudent as gs On gs.GroupId = gp.GroupId", c);
                SqlDataReader reader;

                reader = sc.ExecuteReader();

                DC.Add(new Paragraph("ID          TITLE          ADVISOR          ROLE          REGISTRATION NO          STUDENT"));
                while (reader.Read())
                {
                    string id = reader["Id"].ToString();
                    string title = reader["Title"].ToString();
                    string advisor = reader["AdvisorName"].ToString();
                    string role = reader["AdvisorRole"].ToString();
                    string reg = reader["RegistrationNumber"].ToString();
                    string student = reader["StudentName"].ToString();
                    DC.Add(new Paragraph(id + "          " + title + "          " + advisor + "          " + role + "          " + reg + "          " + student));
                }

                reader.Close();

                DC.Close();

                MessageBox.Show("Report generated Successfully in Desktop folder.");
            }
            else if (cmbMenu.Text == "Report for Mark Sheet")
            {
                string path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "/MarkSheetReport.pdf";
                Document DC = new Document();
                FileStream FS = File.Create(path);
                PdfWriter.GetInstance(DC, FS);
                DC.Open();
                DC.Add(new Paragraph(""));

                var c = Configuration.getInstance().getConnection();
                SqlCommand sc = new SqlCommand("Select p.Title As [Title], s.RegistrationNo As [RegistrationNo], e.Name as [Evaluation], ge.ObtainedMarks as [Marks] From Project As p Join GroupProject As gp On gp.ProjectId = p.Id Join GroupStudent As gs On gs.GroupId = gp.GroupId Join Student As s On s.Id = gs.StudentId Join GroupEvaluation As ge On ge.GroupId = gs.GroupId Join Evaluation As e On e.Id = ge.EvaluationId", c);
                SqlDataReader reader;

                reader = sc.ExecuteReader();

                DC.Add(new Paragraph("TITLE          REGISTRATION NO          EVALUATION          OBTAINED MARKS"));
                while (reader.Read())
                {
                    string title = reader["Title"].ToString();
                    string reg = reader["RegistrationNo"].ToString();
                    string eval = reader["Evaluation"].ToString();
                    string marks = reader["Marks"].ToString();
                    DC.Add(new Paragraph(title + "          " + reg + "                      " + eval + "                        " + marks));
                }

                reader.Close();

                DC.Close();

                MessageBox.Show("Report generated Successfully in Desktop folder.");
            }
            else if (cmbMenu.Text == "Total Marks of Students")
            {
                string path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "/TotalMarkReport.pdf";
                Document DC = new Document();
                FileStream FS = File.Create(path);
                PdfWriter.GetInstance(DC, FS);
                DC.Open();
                DC.Add(new Paragraph(""));

                var c = Configuration.getInstance().getConnection();
                SqlCommand sc = new SqlCommand("Select RegistrationNo as [RegistrationNo], Sum(Marks) as [TotalMarks] From (Select p.Title As [Title], s.RegistrationNo As [RegistrationNo], e.Name as [Evaluation], ge.ObtainedMarks as [Marks] From Project As p Join GroupProject As gp On gp.ProjectId = p.Id Join GroupStudent As gs On gs.GroupId = gp.GroupId Join Student As s On s.Id = gs.StudentId Join GroupEvaluation As ge On ge.GroupId = gs.GroupId Join Evaluation As e On e.Id = ge.EvaluationId) as t Group by RegistrationNo", c);
                SqlDataReader reader;

                reader = sc.ExecuteReader();

                DC.Add(new Paragraph("REGISTRATION NO          TOTAL MARKS"));
                while (reader.Read())
                {
                    string reg = reader["RegistrationNo"].ToString();
                    string marks = reader["TotalMarks"].ToString();
                    DC.Add(new Paragraph(reg + "                      "+ marks));
                }

                reader.Close();

                DC.Close();

                MessageBox.Show("Report generated Successfully in Desktop folder.");
            }
            else if (cmbMenu.Text == "Students without any Advisor")
            {
                string path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "/MissingAdvisorReport.pdf";
                Document DC = new Document();
                FileStream FS = File.Create(path);
                PdfWriter.GetInstance(DC, FS);
                DC.Open();
                DC.Add(new Paragraph(""));

                var c = Configuration.getInstance().getConnection();
                SqlCommand sc = new SqlCommand("Select RegistrationNo From Student Except Select RegistrationNo From Student as s Join GroupStudent as gs On gs.StudentId = s.Id Join GroupProject as gp On gp.GroupId = gs.GroupId Join ProjectAdvisor as pa On pa.ProjectId = gp.ProjectId", c);
                SqlDataReader reader;

                reader = sc.ExecuteReader();

                DC.Add(new Paragraph("REGISTRATION NO"));
                while (reader.Read())
                {
                    string reg = reader["RegistrationNo"].ToString();
                    DC.Add(new Paragraph(reg));
                }

                reader.Close();

                DC.Close();

                MessageBox.Show("Report generated Successfully in Desktop folder.");
            }
        }

        private void dataGrid_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 0)
            {
                DataGridViewRow row = this.dataGrid.Rows[e.RowIndex];

                if (selectedOption == "Student")
                {
                    int id = int.Parse(row.Cells["Id"].Value.ToString());
                    string fName = row.Cells["FirstName"].Value.ToString();
                    string lName = row.Cells["LastName"].Value.ToString();
                    string contact = row.Cells["Contact"].Value.ToString();
                    string email = row.Cells["Email"].Value.ToString();
                    string dob = row.Cells["DateOfBirth"].Value.ToString();
                    int gender = int.Parse(row.Cells["Gender"].Value.ToString());
                    string regNumber = row.Cells["RegistrationNo"].Value.ToString();

                    Student s = new Student(id, fName, lName, contact, email, dob, gender, regNumber);
                    InsertStudentForm f = new InsertStudentForm(s);
                    f.Show();
                    this.Hide();
                }
                else if (selectedOption == "Advisor")
                {
                    int id = int.Parse(row.Cells["Id"].Value.ToString());
                    string fName = row.Cells["FirstName"].Value.ToString();
                    string lName = row.Cells["LastName"].Value.ToString();
                    string contact = row.Cells["Contact"].Value.ToString();
                    string email = row.Cells["Email"].Value.ToString();
                    string dob = row.Cells["DateOfBirth"].Value.ToString();
                    int gender = int.Parse(row.Cells["Gender"].Value.ToString());
                    int designation = int.Parse(row.Cells["Designation"].Value.ToString());
                    float salary = float.Parse(row.Cells["Salary"].Value.ToString());
                    
                    Advisor a = new Advisor(id, fName, lName, contact, email, dob, gender, designation, salary);
                    InsertAdvisorForm f = new InsertAdvisorForm(a);
                    f.Show();
                    this.Hide();
                }
                else if (selectedOption == "Project")
                {
                    int id = int.Parse(row.Cells["Id"].Value.ToString());
                    string title = row.Cells["Title"].Value.ToString();
                    string description = row.Cells["Description"].Value.ToString();

                    Project p = new Project(id, title, description);
                    InsertProjectForm f = new InsertProjectForm(p);
                    f.Show();
                    this.Hide();
                }
                else if (selectedOption == "Evaluation")
                {
                    int id = int.Parse(row.Cells["Id"].Value.ToString());
                    string name = row.Cells["Name"].Value.ToString();
                    int totalMarks = int.Parse(row.Cells["TotalMarks"].Value.ToString());
                    int totalWeightage = int.Parse(row.Cells["TotalWeightage"].Value.ToString());

                    Evaluation eval = new Evaluation(id, name, totalMarks, totalWeightage);
                    InsertEvaluation f = new InsertEvaluation(eval);
                    f.Show();
                    this.Hide();
                }
                else if(selectedOption == "Group")
                {
                    int id = int.Parse(row.Cells["GroupId"].Value.ToString());

                    GroupOfStudents g = new GroupOfStudents(id);
                    GroupingForm f = new GroupingForm(g);
                    f.Show();
                    this.Hide();
                }
                else if (selectedOption == "EvaluateGroup")
                {
                    int id = int.Parse(row.Cells["GroupId"].Value.ToString());

                    GroupOfStudents g = new GroupOfStudents(id);
                    EvaluateGroupForm f = new EvaluateGroupForm(g);
                    f.Show();
                    this.Hide();
                }
                else if(selectedOption == "Marks of Groups")
                {
                    int id = int.Parse(row.Cells["GroupId"].Value.ToString());
                    int eId = int.Parse(row.Cells["EvaluationId"].Value.ToString());
                    int marks = int.Parse(row.Cells["ObtainedMarks"].Value.ToString());
                    string date = (row.Cells["EvaluationDate"].Value.ToString());

                    GroupEvaluation eval = new GroupEvaluation(id, eId, marks, date);
                    EvaluateGroupForm f = new EvaluateGroupForm(eval);
                    f.Show();
                    this.Hide();
                }
                selectedOption = "";

            }
        }
    }
}
