using AuthenticationSystem.Models;
using AuthenticationSystem.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
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
    [Authorize]
    public class EmployeeController : ControllerBase
    {
        private readonly IEmployeeRepository _employeeRepository;
        public EmployeeController(IEmployeeRepository employeeRepository)
        {
            _employeeRepository = employeeRepository;
        }
        [HttpGet]
        public IActionResult GetAll()
        {
           /* var user = User.Identities;*/

            //return Ok("created successfully");
            return Ok(_employeeRepository.GetEmployees());
        }
        [HttpGet("{employeeId:int}")]
        public IActionResult Get(int employeeId)
        {
            if (employeeId == 0) return BadRequest();
            var employeeDetail = _employeeRepository.GetEmployee(employeeId);
            if(employeeDetail == null)
            {
                return NotFound(new {Status=-1,Message="Employee Not Found" });
            }
            return Ok(employeeDetail);
        }
        [HttpPost]
        public IActionResult Create(Employee employee)
        {
            if (employee == null || !ModelState.IsValid) return BadRequest(ModelState);
            if (!_employeeRepository.CreateEmployee(employee))
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
            return Ok(new {Status=1,Message="Employee Created Successfully"});
        }
        [HttpDelete("{employeeId:int}")]
        public IActionResult Delete(int employeeId)
        {
            if (employeeId == 0 ) return BadRequest();
            var employeeExist = _employeeRepository.GetEmployee(employeeId);
            if (employeeExist == null) return NotFound();
            if (!_employeeRepository.DeleteEmployee(employeeExist))
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
            return Ok(new { Status = 1, Message = "Employee deleted successfully" });
        }
       [HttpPut]
       public IActionResult Update(Employee employee)
        {
            if (employee == null || !ModelState.IsValid) return BadRequest();
            if (!_employeeRepository.UpdateEmployee(employee))
                return StatusCode(StatusCodes.Status500InternalServerError);
            return Ok(new { Status = 1, Message = "Employee updated successfully" });
        }
    }
}
