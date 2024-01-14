using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace FYP_Management_System
{
    public partial class Form1 : Form
    {
        int picChange = 1;
        string para = "UET Lahore is one of the top engineering universities in Pakistan, known for its excellent academic programs and research facilities. With a rich history spanning over a century, UET Lahore has established itself as a leading institution in the field of engineering and technology. UET Lahore offers a wide range of undergraduate and graduate programs in various engineering disciplines, as well as computer science and architecture. The university boasts a highly qualified faculty, state-of-the-art laboratories, and a strong emphasis on research and innovation.";
        int paraChar = 0;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            timer2.Start();
            timer3.Start();
        }


        private void timer2_Tick(object sender, EventArgs e)
        {
            string path = "D:\\Study\\DB lab\\Mid project\\Pictures\\" + picChange.ToString() + ".jpg";
            pictureBox2.ImageLocation = string.Format(path);
            pictureBox2.SizeMode = PictureBoxSizeMode.Zoom;
            picChange += 1;
            picChange %= 5;
        }

        private void timer3_Tick(object sender, EventArgs e)
        {
            richTextBox1.Text += para[paraChar];
            paraChar++;

            if(paraChar >= 564)
            {
                timer2.Stop();
                timer3.Stop();

                label4.Text = "Continue";
                label4.MouseEnter += new EventHandler(label4_MouseEnter);
                label4.MouseLeave += new EventHandler(label4_MouseLeave);
            }
        }

        private void label4_Click(object sender, EventArgs e)
        {
            HomePageForm f = new HomePageForm();
            this.Hide();
            f.Show();
        }

        private void label4_MouseEnter(object sender, EventArgs e)
        {
            label4.ForeColor = Color.Green;
            label4.Text = "Continue  -->";
        }

        private void label4_MouseLeave(object sender, EventArgs e)
        {
            label4.ForeColor = Color.Black;
            label4.Text = "Continue";
        }
    }
}
