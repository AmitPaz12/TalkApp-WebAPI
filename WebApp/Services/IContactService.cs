using WebApp.Models;
namespace WebApp.Services
{
    public interface IContactService
    {

        public Task<Contact> GetContact(string id);

        public Task<List<Contact>> GetAllContacts();

        public Task<bool> CheckIfInDB(string id);

        public Task<bool> AddToDB(Contact contact);

        public Task<int> PutContact(string id, Contact contact);

        public Task<int> DeleteContact(string id);

    }
}
