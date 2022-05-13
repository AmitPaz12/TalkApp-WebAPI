using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using WebApp.Data;
using WebApp.Models;
using WebApp.Services;

namespace WebApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InvitationsController : ControllerBase
    {
        private IContactService _contactService;
        private IUserService _userService;


        public InvitationsController(IContactService contactService, IUserService userService)
        {
            _contactService = contactService;
            _userService = userService;
        }


        // POST: api/Invitations
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Invitation>> PostInvitation([Bind("Id, From, To, Server ")] Invitation invitation)
        {
            User user = await _userService.GetByName(invitation.To);
            if (user == null)
            {
                return BadRequest("User does not exist");
            }
            if (await _contactService.CheckIfInUserContacts(user.Name, invitation.From)) 
            {
                return BadRequest("Contact already exists");
            }
            Contact contact = new Contact();
            contact.Id = invitation.From;
            contact.Name = invitation.From;
            contact.User = user;
            contact.Messages = new List<Message>();
            contact.Server = invitation.Server;
            contact.LastMessage = null;
            contact.LastSeen = null;

            await _contactService.AddToDB(contact);

            return CreatedAtAction("PostInvitation", new { id = contact.Id }, contact);

        }
    }

        
}
