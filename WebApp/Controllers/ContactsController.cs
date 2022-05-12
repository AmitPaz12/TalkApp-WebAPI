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
        private IContactService _service;
        private IUserService _userService;

        public IConfiguration _configuration;

        public ContactsController(IContactService service, IUserService userService, IConfiguration configuration)
        {
            _service = service;
            _userService = userService;
            _configuration = configuration;
        }




        // GET: api/Contacts
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<Contact>>> GetContacts()
        {
            var currentUser = HttpContext.User;
            string userID = null;
            if (currentUser.HasClaim(c => c.Type == "UserId"))
            {
                userID = currentUser.Claims.FirstOrDefault(c => c.Type == "UserId").Value;
            }
            if (userID != null)
            {
                User user = await _userService.GetByID(Int16.Parse(userID));
                if (user != null)
                {
                    return _service.GetContactsByUserID(Int16.Parse(userID)).ToList();
                }
                return BadRequest("didn't find user");
            }
            return BadRequest("no claim");
        }

        // GET: api/Contacts/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Contact>> GetContact(string id)
        {
            var contact = await _service.GetContact(id);

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
            int result = await _service.PutContact(id, contact);

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

            if (await _service.CheckIfInDB(contact.Id))
            {
                return NotFound("Contact not exist");
            }
            /*var claims = new[]
            {
                 new Claim(JwtRegisteredClaimNames.Sub, _configuration["JWTParams:Subject"]),
                 new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                 new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()),
                 new Claim("ContactId", contact.Id)
                };*/

            /*var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWTParams:SecretKey"]));
            var mac = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                _configuration["JWTParams:Issuer"],
                _configuration["JWTParams:Audience"],
                claims,
                expires: DateTime.UtcNow.AddMinutes(20),
                signingCredentials: mac);*/


            contact.User = new User();
            contact.Messages = new List<Message>();
            contact.LastMessage = contact.Server;
            contact.LastSeen = null;

            await _service.AddToDB(contact);

            return CreatedAtAction("GetContact", new { id = contact.Id }, contact);
        }

        // DELETE: api/Contacts/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteContact(string id)
        {
            int result = await _service.DeleteContact(id);
            if (result == -1)
            {
                return NotFound();
            }
            return NoContent();
        }

        /*private bool ContactExists(int id)
        {
            return _context.Contact.Any(e => e.Id == id);
        }*/
    }
}
