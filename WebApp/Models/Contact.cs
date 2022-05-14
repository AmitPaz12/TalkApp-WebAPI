using System.Runtime.Serialization;

namespace WebApp.Models
{
    public class Contact
    {
        //displayName
        public string Id { get; set; }

        public User? User { get; set; }

        public string? Name { get; set; }

        public DateTime? LastSeen { get; set; }

        public string? Server { get; set; }

        public string? LastMessage { get; set; }

        [IgnoreDataMember]
        public List<Message>? Messages { get; set; }



    }
}
