using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FYP_Management_System.BL
{
    public class Project
    {
        private int id;
        private string title;
        private string description;

        public int Id { get => id; set => id = value; }
        public string Title { get => title; set => title = value; }
        public string Description { get => description; set => description = value; }

        public Project()
        {
            Title = "";
            Description = "";
            Id = 0;
        }

        public Project(int id, string title, string description)
        {
            Id = id;
            Title = title;
            Description = description;
        }
    }
}
