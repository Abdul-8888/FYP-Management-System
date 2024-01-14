using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FYP_Management_System.BL
{
    public class Evaluation
    {
        private int id;
        private string name;
        private int totalMarks;
        private int totalWeightage;

        public int Id { get => id; set => id = value; }
        public string Name { get => name; set => name = value; }
        public int TotalMarks { get => totalMarks; set => totalMarks = value; }
        public int TotalWeightage { get => totalWeightage; set => totalWeightage = value; }

        public Evaluation()
        {
            Id = 0;
            Name = "";
            TotalMarks = 0;
            TotalWeightage = 0;
        }

        public Evaluation(int id, string name, int totalMarks, int totalWeightage)
        {
            Id = id;
            Name = name;
            TotalMarks = totalMarks;
            TotalWeightage = totalWeightage;
        }
    }
}
