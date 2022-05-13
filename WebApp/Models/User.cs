using System.ComponentModel.DataAnnotations;

namespace WebApp.Models
{
    public class User
    {
        [Key]
        public string Name { get; set; }

        public string Password { get; set; }

        public string DisplayName { get; set; }

        public List<Contact>? Contacts { get; set; }


    }
}
