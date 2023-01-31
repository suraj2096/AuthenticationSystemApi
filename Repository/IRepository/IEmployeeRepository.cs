using AuthenticationSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthenticationSystem.Repository.IRepository
{
    public interface IEmployeeRepository
    {
        public ICollection<Employee> GetEmployees();
        public bool CreateEmployee(Employee employee);
        public bool UpdateEmployee(Employee employee);
        public bool DeleteEmployee(Employee employee);
        public Employee GetEmployee(int employeeId);
        public bool save();
    }
}
