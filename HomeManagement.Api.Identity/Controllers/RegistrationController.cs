﻿using HomeManagement.Contracts;
using HomeManagement.Models;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;

namespace HomeManagement.Api.Identity.Controllers
{
    [EnableCors("IdentityApiCorsPolicy")]
    [Route("api/[controller]")]
    [ApiController]
    public class RegistrationController : ControllerBase
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly SignInManager<IdentityUser> signInManager;
        private readonly ICryptography cryptography;

        public RegistrationController(UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            ICryptography cryptography)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.cryptography = cryptography;
        }

        [HttpPost]
        public async Task<IActionResult> Register([FromBody] UserModel userModel)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState.Select(x => x.Value));

            var password = cryptography.Decrypt(userModel.Password);
            var user = new IdentityUser
            {
                Email = userModel.Email,
                UserName = userModel.Email
            };

            var result = await userManager.CreateAsync(user, password);

            if (result.Succeeded) return Ok();
            else return BadRequest(result.Errors);
        }
    }
}