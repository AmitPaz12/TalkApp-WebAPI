namespace WebApp.Services
{
    public class ContactPost
    {
        public string id { get; set; }
        public string name { get; set; }
        public string server { get; set; }
        public string? last { get; set; }
        public TimeSpan? lastdate { get; set; }
    }
}