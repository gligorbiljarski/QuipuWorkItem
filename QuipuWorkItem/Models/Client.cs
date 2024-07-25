// Models/Client.cs
using System;
using System.ComponentModel.DataAnnotations;

namespace QuipuWorkItem.Models
{
    public class Client
    {
        public int ID { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime BirthDate { get; set; }

        public string Address { get; set; }
    }
}
