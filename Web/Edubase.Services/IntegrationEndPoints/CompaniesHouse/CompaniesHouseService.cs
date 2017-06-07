using CompaniesHouse;
using CompaniesHouse.Request;
using Edubase.Services.Domain;
using Edubase.Services.IntegrationEndPoints.CompaniesHouse.Models;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;

namespace Edubase.Services.IntegrationEndPoints.CompaniesHouse
{
    public class CompaniesHouseService : ICompaniesHouseService
    {
        public async Task<PagedDto<CompanyProfile>> SearchByCompaniesHouseNumber(string number)
        {
            var result = await GetCompaniesHouseClient().GetCompanyProfileAsync(number);

            if (result != null && result.Data != null)
            {
                var a = result.Data.RegisteredOfficeAddress;
                return PagedDto<CompanyProfile>.One(new CompanyProfile
                {
                    Name = result.Data.CompanyName,
                    Address = new CompanyAddress { Line1 = a.AddressLine1, Line2 = a.AddressLine2, Line3 = a.Locality, CityOrTown = a.Region, PostCode = a.PostalCode },
                    IncorporationDate = result.Data.DateOfCreation,
                    Number = result.Data.CompanyNumber
                });
            }
            else return PagedDto<CompanyProfile>.Empty;
        }


        public async Task<PagedDto<CompanyProfile>> SearchByName(string text, int skip = 0, int take = 50)
        {
            var result = await GetCompaniesHouseClient().SearchCompanyAsync(new SearchRequest
            {
                Query = text,
                ItemsPerPage = take,
                StartIndex = skip
            });

            if (result != null && result.Data != null && result.Data.Companies.Length > 0)
            {
                return new PagedDto<CompanyProfile>(skip, take, result.Data.Companies.Select(x => new CompanyProfile
                {
                    Name = x.Title,
                    Address = new CompanyAddress { Line1 = x.Address?.AddressLine1, Line2 = x.Address?.AddressLine2, Line3 = x.Address?.Locality, CityOrTown = x.Address?.Region, PostCode = x.Address?.PostalCode },
                    IncorporationDate = x.DateOfCreation,
                    Number = x.CompanyNumber
                }).ToList(), result.Data.TotalResults.GetValueOrDefault());
            }
            else return PagedDto<CompanyProfile>.Empty;
        }

        private CompaniesHouseClient GetCompaniesHouseClient()
        {
            var key = ConfigurationManager.AppSettings["CompaniesHouseApiKey"];
            var settings = new CompaniesHouseSettings(key);
            var client = new CompaniesHouseClient(settings);
            return client;
        }
        
    }
}
