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
using System.Security.Claims;
using System.Threading.Tasks;

namespace AuthenticationSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        
        private readonly IUserServiceRepository _userService;
        private readonly IJwtManagerRepository _jwtManager;
        private readonly ApplicationUserManager _userManager;
        private readonly IMapper _mapper;
        
        
        public UserController(IUserServiceRepository userService,IMapper mapper,IJwtManagerRepository jwtManager,ApplicationUserManager userManager)
        {
            _userService = userService;
            _mapper = mapper;
            _jwtManager = jwtManager;
            _userManager = userManager;           
        }
        [Route("Login")]
        [HttpPost]
        public async Task<IActionResult> Login([FromBody]UserLoginDetail user)
        {
            if (await _userService.IsUnique(user.UserName)) return Ok(new { Message = "Please Register first then login!!!" });
            var userAuthorize = await _userService.AuthenticateUser(user.UserName, user.Password);
            if (userAuthorize == null) return Unauthorized();

            return Ok(new {Token=userAuthorize.Token,RefreshToken=userAuthorize.RefreshToken});
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
        [Route("RefreshToken")]
        [HttpPost]
        public async  Task<IActionResult> RefreshToken(UserToken userToken)
        {
            Request.Headers.TryGetValue("IS-TOKEN-EXPIRED", out var headerValue);
            if (userToken == null || !ModelState.IsValid || headerValue.FirstOrDefault()=="")
            {
                return BadRequest();
            }
            var claimUserDataFromToken = _jwtManager.GetClaimsFromExpiredToken(userToken.Token);
            if (claimUserDataFromToken == null)
            {
                return Unauthorized(new { Message = "Invalid token!!!!!!" });
            }
            var claimUserIdentity = (ClaimsIdentity)claimUserDataFromToken.Identity;
            var claimUser = claimUserIdentity.FindFirst(ClaimTypes.Name);
            if(claimUser == null)
            {
                return Unauthorized();
            }
            var checkUserInDb = await _userManager.FindByIdAsync(claimUser.Value);
            if (checkUserInDb == null) return Unauthorized();
            var userGetRole = await _userManager.GetRolesAsync(checkUserInDb);
            checkUserInDb.Role = userGetRole.FirstOrDefault();
            if(checkUserInDb.RefreshToken != userToken.RefreshToken)
            {
                return Unauthorized(new { Message = "Go to login!!!!!!" });
            }
            var generateNewToken = _jwtManager.GenerateToken(checkUserInDb);
            if(!await _userService.UpdateUserRefreshToken(generateNewToken))
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }

            UserToken usertoken = new UserToken()
            {
                Token = generateNewToken.Token,
                RefreshToken = generateNewToken.RefreshToken,
            };
            return Ok(usertoken);
        }

    }
}
