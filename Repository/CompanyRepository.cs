using Contracts;
using Entities;
using Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class CompanyRepository : RepositoryBase<Company>, ICompanyRepository
    {
        public CompanyRepository(RepositoryContext repositoryContext) : base(repositoryContext)
        {
        }

        public void DeleteCompany(Company company)
        {
            Delete(company);
        }

        public IEnumerable<Company> GetAllCompanies(bool trackChanges) =>
                FindAll(trackChanges)
                .OrderBy(c => c.Name)
                .ToList();

        public IEnumerable<Company> GetByIds(IEnumerable<Guid> ids, bool trackChanges)
        {
            return FindByCondition(x => ids.Contains(x.Id), trackChanges).ToList();
        }

        public Company GetCompany(Guid companyId, bool trackChanges) =>
            FindByCondition(c => c.Id.Equals(companyId), trackChanges)
            .SingleOrDefault();

        void ICompanyRepository.CreateCompany(Company company)
        {
            Create(company);
        }
    }
}
