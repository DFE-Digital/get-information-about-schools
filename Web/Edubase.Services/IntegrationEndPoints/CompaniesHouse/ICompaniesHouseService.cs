using System.Threading.Tasks;
using Edubase.Services.Domain;

namespace Edubase.Services.IntegrationEndPoints.CompaniesHouse
{
    public interface ICompaniesHouseService
    {
        Task<PagedDto<CompanyProfileDto>> SearchByCompaniesHouseNumber(string number);
        Task<PagedDto<CompanyProfileDto>> SearchByName(string text, int skip = 0, int take = 50);
    }
}