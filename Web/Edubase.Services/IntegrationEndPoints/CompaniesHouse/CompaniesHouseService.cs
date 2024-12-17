using CompaniesHouse;
using CompaniesHouse.Request;
using Edubase.Services.Domain;
using Edubase.Services.IntegrationEndPoints.CompaniesHouse.Models;
using System;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;

namespace Edubase.Services.IntegrationEndPoints.CompaniesHouse
{
    public class CompaniesHouseService : ICompaniesHouseService
    {
        private const int COMPANIES_HOUSE_SIZE_LIMIT = 400;

        public async Task<PagedDto<CompanyProfile>> SearchByCompaniesHouseNumber(string number)
        {
            var result = await GetCompaniesHouseClient().GetCompanyProfileAsync(number);

            if (result != null && result.Data != null)
            {
                var a = result.Data.RegisteredOfficeAddress;
                return PagedDto<CompanyProfile>.One(new CompanyProfile
                {
                    Name = result.Data.CompanyName,
                    Address = new CompanyAddress { Line1 = a.AddressLine1, Line2 = a.AddressLine2, CityOrTown = a.Locality, PostCode = a.PostalCode },
                    IncorporationDate = result.Data.DateOfCreation,
                    Number = result.Data.CompanyNumber
                });
            }
            else return PagedDto<CompanyProfile>.Empty;
        }


        public async Task<PagedDto<CompanyProfile>> SearchByName(string text, int skip = 0, int take = 50)
        {
            var result = await GetCompaniesHouseClient().SearchCompanyAsync(new SearchCompanyRequest
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
                    Address = new CompanyAddress { Line1 = x.Address?.AddressLine1, Line2 = x.Address?.AddressLine2, CityOrTown = x.Address?.Locality, PostCode = x.Address?.PostalCode },
                    IncorporationDate = x.DateOfCreation,
                    Number = x.CompanyNumber
                }).ToList(), Math.Min(result.Data.TotalResults.GetValueOrDefault(), COMPANIES_HOUSE_SIZE_LIMIT));
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
