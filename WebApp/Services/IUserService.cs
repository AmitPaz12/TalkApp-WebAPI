using WebApp.Models;
namespace WebApp.Services
{
    public interface IUserService
    {
        public Task<bool> AddToDB(User user);

        public Task<bool> CheckIfInDB(string name, string password);

        public Task<User?> GetByID(int id);

        public Task<User?> GetByName(string name);

       // public List<Contact> GetAllContacts(string id);

        public Task<List<User>> GetAll();

        public Task<int> PutUser(int id, User user);

        public Task<int> DeleteUser(int id);

        public bool UserExists(int id);




    }
}
