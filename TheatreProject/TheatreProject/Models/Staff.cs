using System;
using System.Collections.Generic;

namespace TheatreProject.Models
{
    public class Staff : User
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string PostCode { get; set; }
        public DateTime BirthDate { get; set; }

        public virtual ICollection<Post> Posts { get; set; }
    }
}