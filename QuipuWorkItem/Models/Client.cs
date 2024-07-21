using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace QuipuWorkItem.Models
{
    public class Client
    {
        public int ID { get; set; }

        [Required(ErrorMessage = "First Name is required")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last Name is required")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress]
        public string Email { get; set; }

        public DateTime BirthDate { get; set; }
    }
}
