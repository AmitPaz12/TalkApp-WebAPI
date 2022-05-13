namespace WebApp.Models
{
    public class Invitation
    {
        public int Id { get; set; }
        public string From { get; set; }

        public string To { get; set; }

        public string Server { get; set; }
    }
}
