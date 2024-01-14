using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FYP_Management_System.BL
{
    public class Advisor: Person
    {
        private int designation;
        private float salary;

        public int Designation { get => designation; set => designation = value; }
        public float Salary { get => salary; set => salary = value; }

        public Advisor(): base()
        {
            Designation = 0;
            Salary = 0;
        }

        public Advisor(int id, string firstName, string lastName, string contact, string email, string dob, int gender, int designation, float salary) : base(id, firstName, lastName, contact, email, dob, gender)
        {
            Designation = designation;
            Salary = salary;
        }
    }
}
