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
    public class transferController : ControllerBase
    {
        private IContactService _contactService;
        private IUserService _userService;
        private IMessageService _messagesService;

        public transferController(IContactService service, IUserService userService, IMessageService messagesService)
        {
            _contactService = service;
            _userService = userService;
            _messagesService = messagesService;
        }



        // POST: api/Transfers
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Transfer>> PostTransfer([Bind("id, from, to, content ")] Transfer transfer)
        {
            User userToAdd = await _userService.GetByName(transfer.from);
            User currentUser = await _userService.GetByName(transfer.to);
            if (currentUser == null)
            {
                return BadRequest("User does not exist");
            }

            Contact contact = await _contactService.GetContact(currentUser.userName, transfer.from);

            if (await _contactService.CheckIfInUserContacts(currentUser.userName, transfer.from) == false)
            {
                return BadRequest("Contact not exists");
            }

            Message message = new Message();
            message.content = transfer.content;
            message.created = DateTime.Now;
            message.sent = false;
            message.Contact = contact;

            await _messagesService.AddToDB(message);

            return CreatedAtAction("PostTransfer", new { id = message.id }, message);
        }

    }
}
