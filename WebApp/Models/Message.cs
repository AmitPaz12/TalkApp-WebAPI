namespace WebApp.Models
{
    public class Message
    {
        public int Id { get; set; }

        public string Content { get; set; }

        public DateTime CreatedDate { get; set; }

        public bool Sent { get; set; }

        public Contact? Contact { get; set; }

    }
}
