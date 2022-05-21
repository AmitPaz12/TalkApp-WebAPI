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
    public class invitationController : ControllerBase
    {
        private IContactService _contactService;
        private IUserService _userService;


        public invitationController(IContactService contactService, IUserService userService)
        {
            _contactService = contactService;
            _userService = userService;
        }


        // POST: api/Invitations
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Invitation>> PostInvitation([Bind("from, to, server")] Invitation invitation)
        {
            User userToAdd = await _userService.GetByName(invitation.from);
            User currentUser = await _userService.GetByName(invitation.to);

            if (currentUser == null)
            {
                return BadRequest("User does not exist");
            }

            if (await _contactService.CheckIfInUserContacts(invitation.to, invitation.from))
            {
                return BadRequest("Contact already exists");
            }
            Contact contact = new Contact();
            contact.id = invitation.from;
            contact.name = userToAdd.displayName;
            contact.User = currentUser;
            contact.Messages = new List<Message>();
            contact.server = invitation.server;
            contact.last = null;
            contact.lastdate = null;

            await _contactService.AddToDB(contact);

            return CreatedAtAction("PostInvitation", new { id = contact.Identifier }, contact);

        }
    }


}
