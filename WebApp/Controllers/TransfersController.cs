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
    public class TransfersController : ControllerBase
    {
        private IContactService _contactService;
        private IUserService _userService;
        private IMessageService _messagesService;

        public TransfersController(IContactService service, IUserService userService, IMessageService messagesService)
        {
            _contactService = service;
            _userService = userService;
            _messagesService = messagesService;
        }



        // POST: api/Transfers
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Transfer>> PostTransfer([Bind("Id, From, To, Content ")] Transfer transfer)
        {
            User user = await _userService.GetByName(transfer.To);
            if (user == null)
            {
                return BadRequest("User does not exist");
            }
            Contact contact = await _contactService.GetContact(transfer.From);

            if (await _contactService.CheckIfInUserContacts(user.Name, transfer.From) == false)
            {
                return BadRequest("Contact not exists");
            }
            Message message = new Message();
            message.Id = transfer.Id;
            message.Content = transfer.Content;
            message.CreatedDate = DateTime.Now;
            message.Sent = true;
            message.Contact = contact;

            await _messagesService.AddToDB(message);

            return CreatedAtAction("PostTransfer", new { id = contact.Id }, contact);
        }

    }
}
