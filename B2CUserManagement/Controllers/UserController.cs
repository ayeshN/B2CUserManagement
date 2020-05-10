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

        [HttpGet]
        public async Task<ActionResult> Get([FromQuery]string email)
        {
            try
            {
                var user = await this.UserManager.GetUserByEmail(email);

                if (user == null)
                {
                    return this.NotFound("User do not exist");
                }


                return this.Ok(user.CurrentPage[0].DisplayName);
            }
            catch (Exception ex)
            {
                return this.BadRequest("Could not get the user");
            }
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

        [HttpDelete("{email}")]
        public async Task<ActionResult> Delete(string email)
        {
            try
            {
                var result = await this.UserManager.DeleteUser(email);

                if (result)
                {
                    return this.Ok("User deleted");
                }
                return this.BadRequest("Couldn't delete the user");
            }
            catch (Exception ex)
            {
                return this.BadRequest("Could not delete User");
            }
        }
    }
}