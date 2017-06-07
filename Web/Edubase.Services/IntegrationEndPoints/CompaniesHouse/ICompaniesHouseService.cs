using System.Threading.Tasks;
using Edubase.Services.Domain;
using Edubase.Services.IntegrationEndPoints.CompaniesHouse.Models;

namespace Edubase.Services.IntegrationEndPoints.CompaniesHouse
{
    public interface ICompaniesHouseService
    {
        Task<PagedDto<CompanyProfile>> SearchByCompaniesHouseNumber(string number);
        Task<PagedDto<CompanyProfile>> SearchByName(string text, int skip = 0, int take = 50);
    }
}