using Edubase.Common;
using Edubase.Common.Cache;
using Edubase.Common.Reflection;
using Edubase.Data;
using Edubase.Services.Enums;
using Edubase.Services.Establishments.DisplayPolicies;
using Edubase.Services.Establishments.Search;
using Edubase.Services.Lookup;
using Edubase.Services.Texuna.Establishments;
using Edubase.Services.Texuna.Lookup;
using Edubase.Services.Texuna.Security;
using LinqToExcel;
using LinqToExcel.Attributes;
using Newtonsoft.Json;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace Edubase.IntegrationTest.Establishment
{
    public class Layout2TypeMapping
    {
        [ExcelColumn("Layout ID")]
        public int LayoutId { get; set; }

        [ExcelColumn("Type ID")] 
        public int TypeId { get; set; }

        public int[] SampleUrns { get; set; }

        public Dictionary<int, EstablishmentDisplayEditPolicy> Urn2PolicyMap { get; set; } = new Dictionary<int, EstablishmentDisplayEditPolicy>();
    }
    

    public class AnalysisResult
    {
        public int SampleUrn { get; set; }
        public int LayoutId { get; set; }
        public int TypeId { get; set; }
        public string TypeName { get; set; }
        public string FieldName { get; set; }
    }

    [TestFixture]
    public class DisplayRulesTest
    {
        [Test, TestCase("")] // anon and then BO user
        public async Task CheckDisplayRules(string saUserId)
        {
            var p = new GenericPrincipal(new GenericIdentity(""), new string[0]);
            var client = new Services.HttpClientWrapper(new HttpClient(new HttpClientHandler { UseCookies = false}) { BaseAddress = new Uri(ConfigurationManager.AppSettings["TexunaApiBaseAddress"]) });
            var svc = new EstablishmentReadApiService(client, new CachedLookupService(new LookupApiService(client, new SecurityApiService(client)), new CacheAccessor(new JsonConverterCollection() { new DbGeographyConverter() })));

            var excel = new ExcelQueryFactory(System.IO.Path.Combine(TestContext.CurrentContext.TestDirectory, "Establishment Field Display Rules v1.0.xlsx"));
            var mappings = excel.Worksheet<Layout2TypeMapping>("Mapping").Where(x => x.LayoutId > 0).ToList();

            var tasks = mappings.Select(async m =>
            {
                var searchResult = await svc.SearchAsync(new EstablishmentSearchPayload { Filters = new EstablishmentSearchFilters { TypeIds = new int[] { m.TypeId } } }, p);
                m.SampleUrns = searchResult.Items.Take(1).Select(x => x.Urn.Value).ToArray();

                foreach (var urn in m.SampleUrns)
                {
                    var policy = await svc.GetDisplayPolicyAsync(new Services.Establishments.Models.EstablishmentModel { Urn = urn }, p);
                    m.Urn2PolicyMap.Add(urn, policy);
                }
            });

            await Task.WhenAll(tasks);
            

            var sheet = excel.Worksheet("Establishments");
            var cols = excel.GetColumnNames("Establishments").ToList();

            var report = new List<AnalysisResult>();
            
            const int FIELD_NAME_INDEX = 2; 
            foreach (var item in mappings)
            {
                var flagIndex = cols.IndexOf(cols.First(x => x.Contains($"({item.LayoutId})") == true));

                foreach (var row in sheet)
                {
                    var fieldName = row[FIELD_NAME_INDEX]?.Value?.ToString();
                    var flag = row[flagIndex]?.Value?.ToString();
                    if (fieldName.Clean() != null)
                    {
                        foreach (var nvp in item.Urn2PolicyMap)
                        {
                            var yayNay = ReflectionHelper.GetPropertyValue<bool>(nvp.Value, fieldName);
                            if (flag.Equals("Yes", StringComparison.OrdinalIgnoreCase) /*flag.IndexOf("yes", StringComparison.OrdinalIgnoreCase) > -1*/ && yayNay == false)
                            {
                                report.Add(new AnalysisResult
                                {
                                    FieldName = fieldName,
                                    LayoutId = item.LayoutId,
                                    TypeId= item.TypeId,
                                    SampleUrn=nvp.Key,
                                    TypeName= ((eLookupEstablishmentType)item.TypeId).ToString()
                                });
                            }
                        }
                    }

                }
            }


            var lines = report.Select(x => $"{x.FieldName},{x.LayoutId},{x.TypeId},{x.TypeName},{x.SampleUrn}");
            File.WriteAllLines("c:\\temp\\display policy report.csv", lines);


        }
    }
}
