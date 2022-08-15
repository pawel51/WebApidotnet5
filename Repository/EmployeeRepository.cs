using Contracts;
using Entities;
using Entities.Models;
using Entities.RequestFeatures;
using Microsoft.EntityFrameworkCore;
using Repository.Extensions;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Repository
{
    public class EmployeeRepository : RepositoryBase<Employee>, IEmployeeRepository
    {
        public EmployeeRepository(RepositoryContext repositoryContext) : base(repositoryContext)
        {
        }

        public void CreateEmployeeForCompany(Guid companyId, Employee employee)
        {
            employee.CompanyId = companyId;
            Create(employee);
        }

        public void DeleteEmployee(Employee employee)
        {
            Delete(employee);
        }

        public async Task<Employee> GetEmployeeAsync(Guid companyId, Guid id, bool trackChanges)
        {
            return await FindByCondition(e => e.CompanyId.Equals(companyId) && e.Id.Equals(id), trackChanges)
                .SingleOrDefaultAsync();
        }

        public async Task<PagedList<Employee>> GetEmployeesAsync(Guid companyId, EmployeeParameters empParams, bool trackChanges)
        {
            IQueryable<Employee> query = FindByCondition(e => e.CompanyId.Equals(companyId), trackChanges);

            var employeesList = await query
                    .FilterEmployees(empParams.MinAge, empParams.MaxAge)
                    .Search(empParams.SearchTerm)
                    .Sort(empParams.OrderBy)
                    .ToListAsync();

            return PagedList<Employee>.ToPagedList(employeesList, empParams.PageNumber, empParams.PageSize);
        }
    }
}
