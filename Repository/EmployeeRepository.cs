using AuthenticationSystem.Identity;
using AuthenticationSystem.Models;
using AuthenticationSystem.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthenticationSystem.Repository
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly ApplicationDbContext _dbContext;
        public EmployeeRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public bool CreateEmployee(Employee employee)
        {
            _dbContext.Employee.Add(employee);
            return save();
        }

        public bool DeleteEmployee(Employee employee)
        {
            _dbContext.Employee.Remove(employee);
            return save();
        }

        public Employee GetEmployee(int employeeId)
        {
            return _dbContext.Employee.FirstOrDefault(m => m.Id == employeeId);
        }

        public ICollection<Employee> GetEmployees()
        {
            return _dbContext.Employee.ToList();
        }

        public bool save()
        {
            return _dbContext.SaveChanges() == 1 ? true : false;
        }

        public bool UpdateEmployee(Employee employee)
        {
            _dbContext.Employee.Update(employee);
            return save();
        }
    }
}
