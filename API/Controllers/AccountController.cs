using System.Security.Claims;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Dtos;
using API.Services;
using Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [ApiController]
    [AllowAnonymous]
    [Route("api/[controller]")]
    //{{url}}/api/account
    public class AccountController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly TokenService _tokenService;

        public AccountController(
            UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager,
            TokenService tokenService
            )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenService = tokenService;
        }


        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
        {
            var user = await _userManager.FindByEmailAsync(loginDto.Email);
            if (user == null) return Unauthorized("user not found");

            var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);
            if (result.Succeeded)
            {
                return CreateUserDto(user);
            }
            return Unauthorized("wrong password, please try again");
        }

        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> Regisert(RegisterDto regDto)

        {
            if (await _userManager.Users.AnyAsync(x => x.Email == regDto.Email))
            {
                return BadRequest("Email already taken, please enter a different email");
            }
            if (await _userManager.Users.AnyAsync(x => x.UserName == regDto.Username))
            {
                return BadRequest("Usrname already taken, please enter a different Username");
            }
            //both UserName and Email have to be unique, 
            //otherwise UserManager will not register this new user in database
            var user = new AppUser
            {
                DisplayName = regDto.DisplayName,
                UserName = regDto.Username,
                Email = regDto.Email
            };
            var result = await _userManager.CreateAsync(user, regDto.Password);
            //
            if (result.Succeeded)
            {
                return CreateUserDto(user);
            }
            return BadRequest("problem registering new user");
        }

        private UserDto CreateUserDto(AppUser user)
        {
            return new UserDto
            {
                DisplayName = user.DisplayName,
                Image = null,
                Token = _tokenService.CreateToken(user),
                Username = user.UserName
            };
        }

        [Authorize]
        [HttpGet]
        public async Task<ActionResult<UserDto>> GetCurrentUser()
        {
            var user = await _userManager.FindByEmailAsync(User.FindFirstValue(ClaimTypes.Email));

            return CreateUserDto(user);

        }



    }
}

