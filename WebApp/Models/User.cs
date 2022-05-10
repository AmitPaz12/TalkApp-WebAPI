namespace WebApp.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public string Password { get; set; }

        public string Display_name { get; set; }

        public string Profile_pic { get; set; }

        public List<Contact> Contacts { get; set; }


    }
}
