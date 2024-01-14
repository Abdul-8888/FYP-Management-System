using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FYP_Management_System.BL
{
    public class Student : Person
    {
        private string registrationNumber;

        public string RegistrationNumber { get => registrationNumber; set => registrationNumber = value; }

        public Student(): base()
        {
            RegistrationNumber = "";
        }

        public Student(int id, string firstName, string lastName, string contact, string email, string dob, int gender, string registrationNumber): base(id, firstName, lastName, contact, email, dob, gender)
        {
            RegistrationNumber = registrationNumber;
        }
    }
}
