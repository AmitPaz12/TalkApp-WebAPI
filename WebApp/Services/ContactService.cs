using WebApp.Data;
using WebApp.Models;
using Microsoft.EntityFrameworkCore;

namespace WebApp.Services
{
    public class ContactService : IContactService
    {

        private readonly WebAppContext _context;

        public ContactService(WebAppContext context)
        {
            _context = context;
        }


        public async Task<Contact> GetContact(string id)
        {
            return await _context.Contact.FindAsync(id);
        }

        public async Task<List<Contact>> GetAllContacts()
        {
            return await _context.Contact.ToListAsync();
        }

        public async Task<bool> CheckIfInDB(string id)
        {
            return await _context.Contact.AnyAsync(c => c.Id == id);
        }

        public async Task<bool> CheckIfInUserContacts(string userName, string contactId)
        {
            return await _context.Contact.AnyAsync(c => c.Id == contactId && c.User.Name == userName);
        }

        public async Task<bool> AddToDB(Contact contact)
        {
            _context.Contact.Add(contact);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<int> PutContact(string id, Contact contact)
        {
            if (id != contact.Id)
            {
                return -1;
            }

            _context.Entry(contact).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ContactExists(id))
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

        public async Task<int> DeleteContact(string id)
        {
            var contact = await _context.Contact.FindAsync(id);
            if (contact == null)
            {
                return -1;
            }

            _context.Contact.Remove(contact);
            await _context.SaveChangesAsync();

            return 1;
        }

        public bool ContactExists(string id)
        {
            return _context.Contact.Any(c => c.Id == id);
        }


        public IQueryable<Contact> GetContactsByUserName(string name)
        {
            var contacts = from contact in _context.Contact
                           where contact.User.Name == name
                           select contact;

            return contacts;
        }

        public IQueryable<Message> GetMessagesByContact(Contact contact)
        {
            var messages = from message in _context.Message
                           where message.Contact.Id == contact.Id
                           select message;

            return messages;
        }
    }
}
