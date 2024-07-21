using System.ComponentModel.DataAnnotations;

namespace QuipuWorkItem.Models
{
    public class Address
    {
        [Key]
        public int Id { get; set; } 

        public int Type { get; set; }
        public string AddressText { get; set; } 
    }
}
