using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FYP_Management_System.BL
{
    public class GroupOfStudents
    {
        private int id;

        public int Id { get => id; set => id = value; }

        public GroupOfStudents(int id)
        {
            Id = id;
        }
    }
}
