using Edubase.Data.Entity;
using MoreLinq;
using Edubase.Data.Entity.Lookups;
using System.Data.Entity.Migrations;
using EdubaseDiocese = Edubase.Data.Entity.Lookups.Diocese;
using EdubaseGender = Edubase.Data.Entity.Lookups.Gender;
using EdubaseGroupType = Edubase.Data.Entity.Lookups.GroupType;
using EdubaseLocalAuthority = Edubase.Data.Entity.LocalAuthority;
using System.Transactions;
using System.Data.Entity;
using System;
using System.Threading.Tasks;
using System.Linq;
using Edubase.Common;

namespace Edubase.Import
{
    class Program
    {
        static void Main(string[] args)
        {
            ImportData();
        }

        static void ImportData()
        {
            Database.SetInitializer<ApplicationDbContext>(null); // disables data model compatibility checking, enabling ID insert

            using (var source = new EdubaseSourceEntities())
            {
                using (var dest = new ApplicationDbContext(true))
                {
                    Console.WriteLine("Importing lookups...");

                    MigrateData(dest, "AdmissionsPolicy",
                            () => source.Admissionspolicy.ForEach(x => dest.AdmissionsPolicies.AddOrUpdate(Map<AdmissionsPolicy>(x.code, x.name))));

                    MigrateData(dest, "Diocese",
                        () => source.Diocese.ForEach(x => dest.Dioceses.AddOrUpdate(new EdubaseDiocese { Id = x.code, Name = x.name })), false);

                    MigrateData(dest, "ProvisionBoarding",
                        () => source.Boarders.ForEach(x => dest.BoardingProvisions.AddOrUpdate(Map<ProvisionBoarding>(x.code, x.name))));

                    MigrateData(dest, "EducationPhase",
                        () => source.Phaseofeducation.ForEach(x => dest.EducationPhases.AddOrUpdate(Map<EducationPhase>(x.code, x.name))));

                    MigrateData(dest, "EstablishmentStatus",
                        () => source.Establishmentstatus.ForEach(x => dest.EstablishmentStatuses.AddOrUpdate(Map<EstablishmentStatus>(x.code, x.name))));

                    MigrateData(dest, "EstablishmentType",
                        () => source.Typeofestablishment.ForEach(x => dest.EstablishmentTypes.AddOrUpdate(Map<EstablishmentType>(x.code, x.name))));

                    MigrateData(dest, "Gender",
                        () => source.Gender.ForEach(x => dest.Genders.AddOrUpdate(Map<EdubaseGender>(x.code, x.name))));

                    MigrateData(dest, "GroupType",
                        () => source.GroupType.ForEach(x => dest.GroupTypes.AddOrUpdate(Map<EdubaseGroupType>(x.GroupTypecode, x.GroupType1))));

                    MigrateData(dest, "HeadTitle",
                        () => source.Headtitle.ForEach(x => dest.HeadTitles.AddOrUpdate(Map<HeadTitle>(x.code, x.name))));

                    Console.WriteLine("... half way through...");

                    MigrateData(dest, "ProvisionNursery",
                        () => source.Nurseryprovision.ForEach(x => dest.NurseryProvisions.AddOrUpdate(Map<ProvisionNursery>(x.code, x.name))));

                    MigrateData(dest, "ProvisionOfficialSixthForm",
                        () => source.Officialsixthform.ForEach(x => dest.OfficialSixthFormProvisions.AddOrUpdate(Map<ProvisionOfficialSixthForm>(x.code, x.name))));

                    MigrateData(dest, "ProvisionSpecialClasses",
                        () => source.Specialclasses.ForEach(x => dest.SpecialClassesProvisions.AddOrUpdate(Map<ProvisionSpecialClasses>(x.code, x.name))));

                    MigrateData(dest, "ReasonEstablishmentClosed",
                        () => source.Reasonestablishmentclosed.ForEach(x => dest.EstablishmentClosedReasons.AddOrUpdate(Map<ReasonEstablishmentClosed>(x.code, x.name))));

                    MigrateData(dest, "ReasonEstablishmentOpened",
                        () => source.Reasonestablishmentopened.ForEach(x => dest.EstablishmentOpenedReasons.AddOrUpdate(Map<ReasonEstablishmentOpened>(x.code, x.name))));

                    MigrateData(dest, "ReligiousCharacter",
                        () => source.Religiouscharacter.ForEach(x => dest.ReligiousCharacters.AddOrUpdate(Map<ReligiousCharacter>(x.code, x.name))));

                    MigrateData(dest, "ReligiousEthos",
                        () => source.Religiousethos.ForEach(x => dest.ReligiousEthos.AddOrUpdate(Map<ReligiousEthos>(x.code, x.name))));

                    MigrateData(dest, "LocalAuthority",
                        () => source.LocalAuthority.ForEach(x => dest.LocalAuthorities.AddOrUpdate(new EdubaseLocalAuthority
                        {
                            Group = x.C_Group,
                            Id = x.Code.ToInteger().Value,
                            Name = x.Name,
                            Order = x.C_Order.ToInteger().Value
                        })));

                    Console.WriteLine("..done");
                    Console.WriteLine();
                    Console.WriteLine();
                    Console.Write("Importing companies");
                    MigrateCompanies(source, dest);
                    Console.WriteLine("..done");
                    Console.WriteLine();
                    Console.WriteLine();

                    MigrateEstablishments(source, dest);
                    
                }
            }
        }
        

        private static void MigrateData(ApplicationDbContext dc, string tableName, Action act, bool enableIdentityInsert = true)
        {
            using (var tran = dc.Database.BeginTransaction())
            {
                if(enableIdentityInsert) dc.Database.ExecuteSqlCommand("SET IDENTITY_INSERT " + tableName + " ON");
                act();
                dc.SaveChanges();
                if(enableIdentityInsert) dc.Database.ExecuteSqlCommand("SET IDENTITY_INSERT " + tableName + " OFF");
                tran.Commit();
            }
        }

        private static void MigrateCompanies(EdubaseSourceEntities source, ApplicationDbContext dc)
        {
            var groupTypes = dc.GroupTypes.ToArrayAsync().Result;
            Action<string> toggle = theSwitch => dc.Database.ExecuteSqlCommand("SET IDENTITY_INSERT " + nameof(Company) + " " + theSwitch);
            using (var tran = dc.Database.BeginTransaction())
            {
                toggle("ON");
                source.CompanyTable.ForEach(x =>
                {
                    var groupType = groupTypes.FirstOrDefault(gt => gt.Id == int.Parse(x.GroupTypecode));
                    var comp = new Company();
                    comp.ClosedDate = x.ClosedDate.ToDateTime(new[] { "dd-MM-yyyy", "dd/MM/yyyy", "dd/MM/yy" });
                    comp.CompaniesHouseNumber = x.CompaniesHouseNumber.Clean();
                    comp.GroupId = x.GroupID.Clean();
                    comp.GroupStatus = x.GroupStatus.Clean();
                    comp.GroupStatusCode = x.GroupStatuscode.Clean();
                    comp.GroupTypeId = groupType.Id;
                    comp.GroupUID = x.GroupUID.ToInteger().Value;
                    comp.Name = x.GroupName.Clean();
                    dc.Companies.AddOrUpdate(comp);
                });

                dc.SaveChanges();
                toggle("OFF");
                tran.Commit();
            }
        }


        private static void MigrateEstablishments(EdubaseSourceEntities source, ApplicationDbContext dc)
        {
            Console.WriteLine("Importing establishments");
            Console.Write("Loading lookups...");
            var admpol = dc.AdmissionsPolicies.ToList();
            var dioceses = dc.Dioceses.ToList();
            var educationPhases = dc.EducationPhases.ToList();
            var establishmentTypes = dc.EstablishmentTypes.ToList();
            var genders = dc.Genders.ToList();
            var headTitles = dc.HeadTitles.ToList();
            var localAuthorities = dc.LocalAuthorities.ToList();
            var boardingProvisions = dc.BoardingProvisions.ToList();
            var nurseryProvisions = dc.NurseryProvisions.ToList();
            var officialSixthFormProvisions = dc.OfficialSixthFormProvisions.ToList();
            var specialClassesProvisions = dc.SpecialClassesProvisions.ToList();
            var establishmentClosedReasons = dc.EstablishmentClosedReasons.ToList();
            var establishmentOpenedReasons = dc.EstablishmentOpenedReasons.ToList();
            var religiousCharacters = dc.ReligiousCharacters.ToList();
            var religiousEthos = dc.ReligiousEthos.ToList();
            var establishmentStatuses = dc.EstablishmentStatuses.ToList();
            Console.WriteLine("\rLoading lookups...done");

            int count = 0;
            Action<string> toggle = theSwitch => dc.Database.ExecuteSqlCommand("SET IDENTITY_INSERT " + nameof(Establishment) + " " + theSwitch);
            using (var tran = dc.Database.BeginTransaction())
            {
                toggle("ON");
                source.SchoolsTable.ForEach(x =>
                {
                    var e = new Establishment();
                    e.Address.CityOrTown = x.Town.Clean();
                    e.Address.County = x.Countyname.Clean();
                    e.Address.Line1 = x.Street.Clean();
                    e.Address.Line2 = x.Address3.Clean();
                    e.Address.Locality = x.Locality.Clean();
                    e.Address.PostCode = x.Postcode.Clean();
                    e.AdmissionsPolicy = admpol.FirstOrDefault(a => a.Id == int.Parse(x.AdmissionsPolicycode));
                    e.Capacity = x.SchoolCapacity.ToInteger();
                    e.CloseDate = x.CloseDate.ToDateTime(new[] { "dd-MM-yyyy", "dd/MM/yyyy", "dd/MM/yy" });
                    e.Contact.EmailAddress = x.MainEmail.Clean();
                    e.Contact.TelephoneNumber = x.TelephoneNum.Clean();
                    e.Contact.WebsiteAddress = x.SchoolWebsite.Clean();
                    e.ContactAlt.EmailAddress = x.AlternativeEmail.Clean();
                    e.Diocese = dioceses.FirstOrDefault(a => a.Id == x.Diocesecode);
                    e.EducationPhase = educationPhases.FirstOrDefault(a => a.Id == int.Parse(x.PhaseOfEducationcode));
                    e.EstablishmentNumber = x.EstablishmentNumber.ToInteger();
                    e.EstablishmentType = establishmentTypes.FirstOrDefault(a => a.Id == int.Parse(x.TypeOfEstablishmentcode));
                    e.Gender = genders.FirstOrDefault(a => a.Id == int.Parse(x.Gendercode));
                    e.HeadFirstName = x.HeadFirstName.Clean();
                    e.HeadLastName = x.HeadLastName.Clean();
                    e.HeadTitle = headTitles.FirstOrDefault(a => a.Id == int.Parse(x.HeadTitlecode));
                    e.LastChangedDate = x.LastChangedDate.ToDateTime(new[] { "dd-MM-yyyy", "dd/MM/yyyy", "dd/MM/yy" });
                    e.LocalAuthority = localAuthorities.FirstOrDefault(a => a.Id == int.Parse(x.LAcode));
                    e.Name = x.EstablishmentName.Clean();
                    e.OpenDate = x.OpenDate.ToDateTime(new[] { "dd-MM-yyyy", "dd/MM/yyyy", "dd/MM/yy" });
                    e.ProvisionBoarding = boardingProvisions.FirstOrDefault(a => a.Id == int.Parse(x.Boarderscode));
                    e.ProvisionNursery = nurseryProvisions.FirstOrDefault(a => a.Id == int.Parse(x.NurseryProvisioncode));
                    e.ProvisionOfficialSixthForm = officialSixthFormProvisions.FirstOrDefault(a => a.Id == int.Parse(x.OfficialSixthFormcode));
                    e.ProvisionSpecialClasses = specialClassesProvisions.FirstOrDefault(a => a.Id == int.Parse(x.SpecialClassescode));
                    e.ReasonEstablishmentClosed = establishmentClosedReasons.FirstOrDefault(a => a.Id == int.Parse(x.ReasonEstablishmentClosedcode));
                    e.ReasonEstablishmentOpened = establishmentOpenedReasons.FirstOrDefault(a => a.Id == int.Parse(x.ReasonEstablishmentOpenedcode));
                    e.ReligiousCharacter = religiousCharacters.FirstOrDefault(a => a.Id == int.Parse(x.ReligiousCharactercode));
                    e.ReligiousEthos = religiousEthos.FirstOrDefault(a => a.Id == int.Parse(x.ReligiousEthoscode));
                    e.Status = establishmentStatuses.FirstOrDefault(a => a.Id == int.Parse(x.EstablishmentStatuscode));
                    e.StatutoryHighAge = x.StatutoryHighAge.ToInteger();
                    e.StatutoryLowAge = x.StatutoryLowAge.ToInteger();
                    e.UKPRN = x.UKPRN.ToInteger();
                    e.Urn = x.URN.ToInteger().Value;
                    dc.Establishments.AddOrUpdate(e);
                    count++;

                    if (count % 100 == 0)
                    {
                        dc.SaveChanges();
                    }
                    Console.WriteLine($"Loaded {count}");
                });
                Console.WriteLine("Saving...");
                dc.SaveChanges();
                toggle("OFF");
                Console.WriteLine("Committing...");
                tran.Commit();
                Console.WriteLine("DONE");
            }
        }

        private static T Map<T>(string id, string name) where T : LookupBase, new() => new T
        {
            Id = int.Parse(id),
            Name = name,
            CreatedUtc = DateTime.UtcNow,
            LastUpdatedUtc = DateTime.UtcNow,
            IsDeleted = false
        };
    }
}
