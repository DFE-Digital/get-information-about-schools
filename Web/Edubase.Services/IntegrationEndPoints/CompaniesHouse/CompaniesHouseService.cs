using CompaniesHouse;
using CompaniesHouse.Request;
using Edubase.Common;
using Edubase.Services.Domain;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;

namespace Edubase.Services.IntegrationEndPoints.CompaniesHouse
{
    public class CompaniesHouseService : ICompaniesHouseService
    {
        public async Task<PagedDto<CompanyProfileDto>> SearchByCompaniesHouseNumber(string number)
        {
            var result = await GetCompaniesHouseClient().GetCompanyProfileAsync(number);

            if (result != null && result.Data != null)
            {
                var a = result.Data.RegisteredOfficeAddress;
                return PagedDto<CompanyProfileDto>.One(new CompanyProfileDto
                {
                    Name = result.Data.CompanyName,
                    Address = StringUtil.ConcatNonEmpties(", ", a.CareOf, a.PoBox, a.AddressLine1, a.AddressLine2, a.Locality, a.Region, a.PostalCode),
                    IncorporationDate = result.Data.DateOfCreation,
                    Number = result.Data.CompanyNumber
                });
            }
            else return PagedDto<CompanyProfileDto>.Empty;
        }


        public async Task<PagedDto<CompanyProfileDto>> SearchByName(string text, int skip = 0, int take = 50)
        {
            var result = await GetCompaniesHouseClient().SearchCompanyAsync(new SearchRequest
            {
                Query = text,
                ItemsPerPage = take,
                StartIndex = skip
            });

            if (result != null && result.Data != null && result.Data.Companies.Length > 0)
            {
                return new PagedDto<CompanyProfileDto>(skip, take, result.Data.Companies.Select(x => new CompanyProfileDto
                {
                    Name = x.Title,
                    Address = StringUtil.ConcatNonEmpties(", ", x.Address?.CareOf, x.Address?.PoBox, x.Address?.AddressLine1,
                            x.Address?.AddressLine2, x.Address?.Locality, x.Address?.Region, x.Address?.PostalCode),
                    IncorporationDate = x.DateOfCreation,
                    Number = x.CompanyNumber
                }).ToList(), result.Data.TotalResults.GetValueOrDefault());
            }
            else return PagedDto<CompanyProfileDto>.Empty;
        }

        private CompaniesHouseClient GetCompaniesHouseClient()
        {
            var key = ConfigurationManager.AppSettings["CompaniesHouseApiKey"];
            var settings = new CompaniesHouseSettings(key);
            var client = new CompaniesHouseClient(settings);
            return client;
        }

        ///// <summary>
        ///// Creates a trust purely from companies house number and then returns the GroupUID of the new Trust
        ///// </summary>
        ///// <param name="currentUser"></param>
        ///// <param name="companiesHouseNumber"></param>
        ///// <returns></returns>
        //public async Task<int> CreateAsync(ClaimsPrincipal currentUser, string companiesHouseNumber, int groupTypeId)
        //{
        //    Validate(currentUser);
        //    if (!(currentUser.IsInRole("Admin") || currentUser.IsInRole("Academy")))
        //        throw new SecurityException("Permission denied");

        //    var dto = (await SearchByCompaniesHouseNumber(companiesHouseNumber)).Items.FirstOrDefault();
        //    if (dto != null)
        //    {
        //        using (var dc = new ApplicationDbContext())
        //        {
        //            var model = new GroupCollection
        //            {
        //                CompaniesHouseNumber = companiesHouseNumber,
        //                Name = dto.Name,
        //                OpenDate = dto.IncorporationDate,
        //                GroupTypeId = groupTypeId,
        //                Address = dto.Address
        //            };
        //            dc.Groups.Add(model);
        //            await dc.SaveChangesAsync();
        //            return model.GroupUID;
        //        }
        //    }
        //    else throw new Exception("Company not found");
        //}

        //private void Validate(ClaimsPrincipal currentUser)
        //{
        //    if (currentUser == null) throw new ArgumentNullException(nameof(currentUser));
        //    if (!currentUser.Identity.IsAuthenticated)
        //        throw new SecurityException("Permission denied");
        //}


    }
}
