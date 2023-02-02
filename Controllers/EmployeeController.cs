using AuthenticationSystem.Identity;
using AuthenticationSystem.Models;
using AuthenticationSystem.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
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
    [Authorize]
    public class EmployeeController : ControllerBase
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IJwtManagerRepository _jwtManagerRepository;
        private readonly ApplicationUserManager _applictionUserManager;
        public EmployeeController(IEmployeeRepository employeeRepository,IJwtManagerRepository jwtManagerRepository,ApplicationUserManager applicationUserManager)
        {
            _employeeRepository = employeeRepository;
            _jwtManagerRepository = jwtManagerRepository;
            _applictionUserManager = applicationUserManager;
        }
       
       //[Authorize(Roles = SD.RoleHR + "," + SD.RoleManager)]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            if (!await CheckValidRole())
            {
                return BadRequest();
            }
            var claimidentity = (ClaimsIdentity)User.Identity;
            var claim = claimidentity.FindFirst(ClaimTypes.Role);

            return Ok(_employeeRepository.GetEmployees());
        }
        
        [HttpGet("{employeeId:int}")]
        public async Task<IActionResult> Get(int employeeId)
        {
            if (!await CheckValidRole())
            {
                return BadRequest();
            }
            if (employeeId == 0) return BadRequest();
            var employeeDetail = _employeeRepository.GetEmployee(employeeId);
            if(employeeDetail == null)
            {
                return NotFound(new {Status=-1,Message="Employee Not Found" });
            }
            return Ok(employeeDetail);
        }
        [Authorize(Roles =SD.RoleManager+","+SD.RoleHR+","+SD.RoleEmployee)]
        [HttpPost]
        public async Task<IActionResult> Create(Employee employee)
        {
            if (!await CheckValidRole())
            {
                return BadRequest();
            }
            if (employee == null || !ModelState.IsValid) return BadRequest(ModelState);
            if (!_employeeRepository.CreateEmployee(employee))
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
            return Ok(new {Status=1,Message="Employee Created Successfully"});
        }
        [Authorize(Roles = SD.RoleHR + "," + SD.RoleManager)]
        [HttpDelete("{employeeId:int}")]
        public async Task<IActionResult> Delete(int employeeId)
        {
            if (!await CheckValidRole())
            {
                return BadRequest();
            }
            if (employeeId == 0 ) return BadRequest();
            var employeeExist = _employeeRepository.GetEmployee(employeeId);
            if (employeeExist == null) return NotFound();
            if (!_employeeRepository.DeleteEmployee(employeeExist))
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
            return Ok(new { Status = 1, Message = "Employee deleted successfully" });
        }
        [Authorize(Roles = SD.RoleManager + "," + SD.RoleHR + "," + SD.RoleEmployee)]
        [HttpPut]
       public async Task<IActionResult> Update(Employee employee)
        {
            if (!await CheckValidRole())
            {
                return BadRequest();
            }
            if (employee == null || !ModelState.IsValid) return BadRequest();
            if (!_employeeRepository.UpdateEmployee(employee))
                return StatusCode(StatusCodes.Status500InternalServerError);
            return Ok(new { Status = 1, Message = "Employee updated successfully" });
        }
        [NonAction]
        public async Task<bool> CheckValidRole()
        {
            var claimIdentity = (ClaimsIdentity)User.Identity;
            var claimUser = claimIdentity.FindFirst(ClaimTypes.Name).Value;
            var claimRole = claimIdentity.FindFirst(ClaimTypes.Role).Value;
            if (claimRole == null || claimUser==null) return false;
            var userFind = await _applictionUserManager.FindByIdAsync(claimUser);
            if (userFind != null)
            {
                var roleExist = await _applictionUserManager.GetRolesAsync(userFind);
                if (roleExist.FirstOrDefault() == claimRole) return true;
            }
            return false;
        }
    }
}
