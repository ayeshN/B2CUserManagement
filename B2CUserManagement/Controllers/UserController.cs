using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using B2CUserManagement.Interfaces;
using B2CUserManagement.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace B2CUserManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private IUserManager UserManager { get; set; }

        public UserController(IUserManager userManager)
        {
            this.UserManager = userManager;
        }

        [HttpPost]
        public async Task<ActionResult<B2CUser>> AddUsers([FromBody] B2CUser user)
        {
            try
            {
                await this.UserManager.CreateUser(user);
                return this.Ok("User Added");
            }
            catch (Exception ex)
            {
                return this.BadRequest("Could not add user");
            }
        }
    }
}