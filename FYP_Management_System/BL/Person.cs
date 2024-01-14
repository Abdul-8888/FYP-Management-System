using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FYP_Management_System.BL
{
    public class Person
    {
        private int id;
        private string firstName;
        private string lastName;
        private string contact;
        private string email;
        private string dob;
        private int gender;

        public int Id { get => id; set => id = value; }
        public string FirstName { get => firstName; set => firstName = value; }
        public string LastName { get => lastName; set => lastName = value; }
        public string Contact { get => contact; set => contact = value; }
        public string Email { get => email; set => email = value; }
        public string Dob { get => dob; set => dob = value; }
        public int Gender { get => gender; set => gender = value; }

        public Person()
        {
            FirstName = "";
            LastName = "";
            Id = 0;
            Contact = "";
            Email = "";
            Dob = "";
            Gender = 0;
        }

        public Person(int id, string firstName, string lastName, string contact, string email, string dob, int gender)
        {
            Id = id;
            FirstName = firstName;
            LastName = lastName;
            Contact = contact;
            Email = email;
            Dob = dob;
            Gender = gender;
        }

    }
}
