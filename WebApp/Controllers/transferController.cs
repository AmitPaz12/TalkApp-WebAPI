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
using WebApp.Hubs;

namespace WebApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class transferController : ControllerBase
    {
        private IContactService _contactService;
        private IUserService _userService;
        private IMessageService _messagesService;
        private MessagesHub _messagesHub;
        private ContactHub _contactHub;


        public transferController(IContactService service, IUserService userService, IMessageService messagesService, MessagesHub messgaesHub, ContactHub contactHub)
        {
            _contactService = service;
            _userService = userService;
            _messagesService = messagesService;
            _messagesHub = messgaesHub;
            _contactHub = contactHub;

        }



        // POST: api/Transfers
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Transfer>> PostTransfer([Bind("id, from, to, content ")] Transfer transfer)
        {
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

            TimeSpan now = DateTime.Now.TimeOfDay;
            TimeSpan time = new TimeSpan(now.Hours, now.Minutes, 0);

            Message message = new Message();
            message.content = transfer.content;
            message.created = time;
            message.sent = false;
            message.Contact = contact;

            await _messagesService.AddToDB(message);
            await _contactService.UpdateLastMessage(message.content, contact);
            try
            {
                await _messagesHub.AddMessage(message, transfer.from, transfer.to);
            }
            catch(Exception ex)
            {
                return RedirectToAction();
            }

            try
            {
                await _contactHub.ContactUpdate(currentUser.userName, contact);
            }
            catch (Exception ex)
            {
                return RedirectToAction();
            }
            

            return CreatedAtAction("PostTransfer", new { id = message.id }, message);
        }

    }
}
