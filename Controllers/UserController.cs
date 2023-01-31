using AuthenticationSystem.Identity;
using AuthenticationSystem.Models;
using AuthenticationSystem.Models.DTOs;
using AuthenticationSystem.Repository;
using AuthenticationSystem.Repository.IRepository;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthenticationSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        
        private readonly IUserServiceRepository _userService;
        private readonly IMapper _mapper;
        
        
        public UserController(IUserServiceRepository userService,IMapper mapper)
        {
            _userService = userService;
            _mapper = mapper;
           
        }
        [Route("Login")]
        [HttpPost]
        public async Task<IActionResult> Login([FromBody]User user)
        {
            if (await _userService.IsUnique(user.UserName)) return Ok(new { Message = "Please Register first then login!!!" });
            var userAuthorize = await _userService.AuthenticateUser(user.UserName, user.Password);
            if (userAuthorize == null) return Unauthorized();
            return Ok(new {token=userAuthorize.Token});
        }
        [HttpPost]
        [Route("Register")]
        public async Task<IActionResult> Register([FromBody]UserRegisterDTO userRegisterDetail)
        {

            var ApplicationUserDetail = _mapper.Map<ApplicationUser>(userRegisterDetail);
            ApplicationUserDetail.PasswordHash = userRegisterDetail.Password;
            if (ApplicationUserDetail == null || !ModelState.IsValid) return BadRequest();
            if (!await _userService.IsUnique(userRegisterDetail.UserName)) return Ok(new { Message = "You are already register go to login" });
            var registerUser = await _userService.RegisterUser(ApplicationUserDetail);
            if (!registerUser) return StatusCode(StatusCodes.Status500InternalServerError);
            return Ok(new { Message = "Register successfully!!!" });
            
        }

    }
}
