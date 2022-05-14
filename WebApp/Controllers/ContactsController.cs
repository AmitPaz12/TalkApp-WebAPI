#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using WebApp.Data;
using WebApp.Services;
using WebApp.Models;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace WebApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContactsController : ControllerBase
    {
        private IContactService _contactService;
        private IUserService _userService;
        private IMessageService _messagesService;
        public IConfiguration _configuration;

        public ContactsController(IContactService service, IUserService userService, IMessageService messagesService, IConfiguration configuration)
        {
            _contactService = service;
            _userService = userService;
            _messagesService = messagesService;
            _configuration = configuration;
        }


        private async Task<User> getUser()
        {
            var currentUser = HttpContext.User;
            string userName = null;
            User user = null;
            if (currentUser.HasClaim(c => c.Type == ClaimTypes.NameIdentifier))
            {
                userName = currentUser.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            }
            if (userName != null)
            {
                user = await _userService.GetByName(userName);
            }
            return user;
                
        }

        // GET: api/Contacts
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<Contact>>> GetContacts()
        {
            User user = await getUser();
            if (user != null)
            {
             return Ok(_contactService.GetContactsByUserName(user.Name).ToList());
            }
            return BadRequest("didn't find user");
        }

        // GET: api/Contacts/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Contact>> GetContact(string id)
        {
            var contact = await _contactService.GetContact(id);

            if (contact == null)
            {
                return NotFound();
            }

            return contact;
        }

        // PUT: api/Contacts/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutContact(string id, Contact contact)
        {
            int result = await _contactService.PutContact(id, contact);

            if (result == -1)
            {
                return BadRequest();
            }
            if (result == 0)
            {
                return NotFound();
            }
            return NoContent();

        }

        // POST: api/Contacts
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Contact>> PostContact([Bind("Id, User, Name, Password, LastSeen, Server, LastMessage, Messages")] Contact contact)
        {

            User user = await getUser();

            if (await _contactService.CheckIfInUserContacts(user.userName, contact.Id))
            {
                return BadRequest("Contact already exists");
            }

            contact.User = user;
            contact.Messages = new List<Message>();
            contact.LastMessage = contact.Server;
            contact.LastSeen = null;

            await _contactService.AddToDB(contact);

            return CreatedAtAction("PostContact", new { id = contact.Id }, contact);
        }

        // DELETE: api/Contacts/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteContact(string id)
        {
            int result = await _contactService.DeleteContact(id);
            if (result == -1)
            {
                return NotFound();
            }
            return NoContent();
        }


        // GET: api/Contact/5/Messages
        [HttpGet("{id}/Messages")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<Message>>> GetMessages(string id)
        {
            Contact contact = await _contactService.GetContact(id);
            return Ok(_contactService.GetMessagesByContact(contact));
        }


        // GET: api/Contact/5/Messages/181
        [HttpGet("{id}/Messages/{id2}")]
        public async Task<ActionResult<Message>> GetMessage(int id2)
        {
            var message = await _messagesService.GetMessage(id2);

            if (message == null)
            {
                return NotFound();
            }

            return Ok(message);
        }

        // PUT: api/Contact/5/Messages/123
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}/Messages/{id2}")]
        public async Task<IActionResult> PutMessage(int id2, Message newMessage)
        {
            Message message = await _messagesService.GetMessage(id2);
            message.Content = newMessage.Content;
            int result = await _messagesService.PutMessage(id2, message);

            if (result == -1)
            {
                return BadRequest();
            }
            if (result == 0)
            {
                return NotFound();
            }
            return NoContent();
        }


        // POST: api/Contacts/5/Messages
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost("{id}/Messages")]
        public async Task<ActionResult<Message>> PostMessage(string id, [Bind("Id, Content, CreatedDate, Sent, Contact")] Message message)
        {
            message.Contact = await _contactService.GetContact(id);
            message.CreatedDate = DateTime.Now;
            message.Sent = false;

            await _messagesService.AddToDB(message);

            return CreatedAtAction("PostMessage", new { id = message.Id }, message);
        }

        // DELETE: api/Contacts/5/Messages/5
        [HttpDelete("{id}/Messages/{id2}")]
        public async Task<IActionResult> DeleteMessage(int id2)
        {
            int result = await _messagesService.DeleteMessage(id2);
            if (result == -1)
            {
                return NotFound();
            }
            return NoContent();
        }

    }
}
