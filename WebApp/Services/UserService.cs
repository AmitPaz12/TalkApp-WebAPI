using WebApp.Models;
using WebApp.Data;
using Microsoft.EntityFrameworkCore;
using WebApp.Services;
using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace WebApp.Services
{
    public class UserService : IUserService
    {
        private readonly WebAppContext _context;

        public UserService(WebAppContext context)
        {
            _context = context;
        }

        public async Task<bool> AddToDB(User user)
        {
            _context.Add(user);
            await _context.SaveChangesAsync();
            return true;
        }

        public  async Task<bool> CheckIfInDB(string name, string password)
        {
            return await _context.User.AnyAsync(e => e.userName == name && e.password == password);
        }


        public async Task<List<User>> GetAll()
        {
            return await _context.User.ToListAsync();
        }

        public async Task<User?> GetByID(int id)
        {
            var users = await GetAll();
            var user = users.Find(m => m.userName == name);

            return user;
        }

        public async Task<User?> GetByName(string name)
        {
            var users = await GetAll();
            var user = users.Find(m => m.userName == name);

            return user;
        }

      /*  public async Task<List<Contact>> GetContacts(string name)
        {
            User? user = await GetByName(name);
            if (user == null)
            {
                return new List<Contact>();
            }
            return user.Contacts;
        }*/


        public async Task<int> PutUser(int id, User user)
        {
            if (name != user.userName)
            {
                return -1;
            }

            _context.Entry(user).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(id))
                {
                    return 0;
                }
                else
                {
                    throw;
                }
            }
            return 1;
        }


        /*        public async Task<ActionResult<User>> PostUser(User user)
                {
                    _context.User.Add(user);
                    await _context.SaveChangesAsync();

                    return CreatedAtAction("GetUser", new { id = user.Id }, user);
                }
        */

        public async Task<int> DeleteUser(int id)
        {
            var user = await _context.User.FindAsync(id);
            if (user == null)
            {
                return -1;
            }

            _context.User.Remove(user);
            await _context.SaveChangesAsync();

            return 1;
        }

        public bool UserExists(int id)
        {
            return _context.User.Any(e => e.userName == name);

        }


    }
}
