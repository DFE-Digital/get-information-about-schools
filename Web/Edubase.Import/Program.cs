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
using System.Collections.Generic;
using System.Data;
using System.ComponentModel;
using System.Data.Entity.Core.Metadata.Edm;
using Edubase.Data.Entity.ComplexTypes;
using System.Data.SqlClient;
using System.Configuration;
using Edubase.Data;

namespace Edubase.Import
{
    using Common.Spatial;
    using Microsoft.SqlServer.Types;
    using System.Data.Entity.Spatial;
    using GovMap = Tuple<string, Type, Func<Governor, object>>;

    class Program
    {
        static void Main(string[] args)
        {
            ImportData();
        }

        static void ImportData()
        {
            Database.SetInitializer<ApplicationDbContext>(null); // disables data model compatibility checking, enabling ID insert

            //Database.SetInitializer(new MigrateDatabaseToLatestVersion<ApplicationDbContext, Edubase.Data.Migrations.Configuration>()); 

            using (var source = new EdubaseSourceEntities())
            {
                using (var dest = new ApplicationDbContext(true))
                {
                    Console.WriteLine("Importing lookups...");

                    MigrateData(dest, "AdmissionsPolicy",
                            () => source.Admissionspolicies.ForEach(x => dest.AdmissionsPolicies.AddOrUpdate(Map<AdmissionsPolicy>(x.code, x.name))));

                    MigrateData(dest, "Diocese",
                        () => source.Diocese.ForEach(x => dest.Dioceses.AddOrUpdate(new EdubaseDiocese { Id = x.code, Name = x.name })), false);

                    MigrateData(dest, "ProvisionBoarding",
                        () => source.Boarders.ForEach(x => dest.BoardingProvisions.AddOrUpdate(Map<ProvisionBoarding>(x.code, x.name))));

                    MigrateData(dest, "EducationPhase",
                        () => source.Phaseofeducations.ForEach(x => dest.EducationPhases.AddOrUpdate(Map<EducationPhase>(x.code, x.name))));

                    MigrateData(dest, "EstablishmentStatus",
                        () => source.Establishmentstatus.ForEach(x => dest.EstablishmentStatuses.AddOrUpdate(Map<EstablishmentStatus>(x.code, x.name))));

                    MigrateData(dest, "EstablishmentType",
                        () => source.Typeofestablishments.ForEach(x => dest.EstablishmentTypes.AddOrUpdate(Map<EstablishmentType>(x.code, x.name))));

                    MigrateData(dest, "Gender",
                        () => source.Genders.ForEach(x => dest.Genders.AddOrUpdate(Map<EdubaseGender>(x.code, x.name))));

                    MigrateData(dest, "GroupType",
                        () => source.GroupTypes.ForEach(x => dest.GroupTypes.AddOrUpdate(Map<EdubaseGroupType>(x.GroupTypecode, x.GroupType1))));

                    MigrateData(dest, "HeadTitle",
                        () => source.Headtitles.ForEach(x => dest.HeadTitles.AddOrUpdate(Map<HeadTitle>(x.code, x.name))));

                    Console.WriteLine("... half way through...");

                    MigrateData(dest, "ProvisionNursery",
                        () => source.Nurseryprovisions.ForEach(x => dest.NurseryProvisions.AddOrUpdate(Map<ProvisionNursery>(x.code, x.name))));

                    MigrateData(dest, "ProvisionOfficialSixthForm",
                        () => source.Officialsixthforms.ForEach(x => dest.OfficialSixthFormProvisions.AddOrUpdate(Map<ProvisionOfficialSixthForm>(x.code, x.name))));

                    MigrateData(dest, "ProvisionSpecialClasses",
                        () => source.Specialclasses.ForEach(x => dest.SpecialClassesProvisions.AddOrUpdate(Map<ProvisionSpecialClasses>(x.code, x.name))));

                    MigrateData(dest, "ReasonEstablishmentClosed",
                        () => source.Reasonestablishmentcloseds.ForEach(x => dest.EstablishmentClosedReasons.AddOrUpdate(Map<ReasonEstablishmentClosed>(x.code, x.name))));

                    MigrateData(dest, "ReasonEstablishmentOpened",
                        () => source.Reasonestablishmentopeneds.ForEach(x => dest.EstablishmentOpenedReasons.AddOrUpdate(Map<ReasonEstablishmentOpened>(x.code, x.name))));

                    MigrateData(dest, "ReligiousCharacter",
                        () => source.Religiouscharacters.ForEach(x => dest.ReligiousCharacters.AddOrUpdate(Map<ReligiousCharacter>(x.code, x.name))));

                    MigrateData(dest, "ReligiousEthos",
                        () => source.Religiousethos.ForEach(x => dest.ReligiousEthos.AddOrUpdate(Map<ReligiousEthos>(x.code, x.name))));

                    MigrateData(dest, "LocalAuthority",
                        () => source.LocalAuthorities.ForEach(x => dest.LocalAuthorities.AddOrUpdate(new EdubaseLocalAuthority
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

                    if (dest.Establishments.Count() == 0) MigrateEstablishments(source, dest);
                    else Console.WriteLine("NOT IMPORTING ESTABLISHMENTS as there is data already present");

                    if (dest.Estab2EstabLinks.Count() == 0) MigrateEstabLinks(source, dest);
                    else Console.WriteLine("NOT IMPORTING Estab2EstabLinks as there is data already present");

                    if (dest.Establishment2CompanyLinks.Count() == 0) MigrateEstablishment2CompanyLinks(source, dest);
                    else Console.WriteLine("NOT IMPORTING Establishment2CompanyLinks as there is data already present");

                    if (dest.Governors.Count() == 0) MigrateGovernors(source, dest);
                    else Console.WriteLine("NOT IMPORTING GOVERNORS as there is data already present");

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
                source.CompanyTables.ForEach(x =>
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

            Console.WriteLine("Loading up locations");
            var locs = new Dictionary<int?, Tuple<int?, int?, LatLon>>();
            using (var betaSource = new EdubaseSourceBetaEntities())
            {
                betaSource.Establishments.Batch(100).ForEach(batch =>
                    batch.ForEach(e => locs.Add(e.URN.ToInteger(), new Tuple<int?, int?, LatLon>(e.Easting.ToInteger(), e.Northing.ToInteger(),
                    new OSGB36Converter().ToWGS84(e.Easting, e.Northing)))));
            }
            Console.WriteLine("...done");


            int count = 0;
            var batchCount = 0;
            var estabs = new List<Establishment>();
            foreach (var batch in source.SchoolsTables.Batch(1000))
            {
                batchCount++;
                Console.WriteLine("Loading batch #" + batchCount);
                batch.ForEach(x =>
                {
                    var e = new Establishment();
                    e.Address.CityOrTown = x.Town.Clean();
                    e.Address.County = x.Countyname.Clean();
                    e.Address.Line1 = x.Street.Clean();
                    e.Address.Line2 = x.Address3.Clean();
                    e.Address.Locality = x.Locality.Clean();
                    e.Address.PostCode = x.Postcode.Clean();
                    e.AdmissionsPolicyId = admpol.FirstOrDefault(a => a.Id == int.Parse(x.AdmissionsPolicycode))?.Id;
                    e.Capacity = x.SchoolCapacity.ToInteger();
                    e.CloseDate = x.CloseDate.ToDateTime(new[] { "dd-MM-yyyy", "dd/MM/yyyy", "dd/MM/yy" });
                    e.Contact.EmailAddress = x.MainEmail.Clean();
                    e.Contact.TelephoneNumber = x.TelephoneNum.Clean();
                    e.Contact.WebsiteAddress = x.SchoolWebsite.Clean();
                    e.ContactAlt.EmailAddress = x.AlternativeEmail.Clean();
                    e.DioceseId = dioceses.FirstOrDefault(a => a.Id == x.Diocesecode)?.Id;
                    e.EducationPhaseId = educationPhases.FirstOrDefault(a => a.Id == int.Parse(x.PhaseOfEducationcode))?.Id;
                    e.EstablishmentNumber = x.EstablishmentNumber.ToInteger();
                    e.TypeId = establishmentTypes.FirstOrDefault(a => a.Id == int.Parse(x.TypeOfEstablishmentcode))?.Id;
                    e.GenderId = genders.FirstOrDefault(a => a.Id == int.Parse(x.Gendercode))?.Id;
                    e.HeadFirstName = x.HeadFirstName.Clean();
                    e.HeadLastName = x.HeadLastName.Clean();
                    e.HeadTitleId = headTitles.FirstOrDefault(a => a.Id == int.Parse(x.HeadTitlecode))?.Id;
                    e.LastChangedDate = x.LastChangedDate.ToDateTime(new[] { "dd-MM-yyyy", "dd/MM/yyyy", "dd/MM/yy" });
                    e.LocalAuthorityId = localAuthorities.FirstOrDefault(a => a.Id == int.Parse(x.LAcode))?.Id;
                    e.Name = x.EstablishmentName.Clean();
                    e.OpenDate = x.OpenDate.ToDateTime(new[] { "dd-MM-yyyy", "dd/MM/yyyy", "dd/MM/yy" });
                    e.ProvisionBoardingId = boardingProvisions.FirstOrDefault(a => a.Id == int.Parse(x.Boarderscode))?.Id;
                    e.ProvisionNurseryId = nurseryProvisions.FirstOrDefault(a => a.Id == int.Parse(x.NurseryProvisioncode))?.Id;
                    e.ProvisionOfficialSixthFormId = officialSixthFormProvisions.FirstOrDefault(a => a.Id == int.Parse(x.OfficialSixthFormcode))?.Id;
                    e.ProvisionSpecialClassesId = specialClassesProvisions.FirstOrDefault(a => a.Id == int.Parse(x.SpecialClassescode))?.Id;
                    e.ReasonEstablishmentClosedId = establishmentClosedReasons.FirstOrDefault(a => a.Id == int.Parse(x.ReasonEstablishmentClosedcode))?.Id;
                    e.ReasonEstablishmentOpenedId = establishmentOpenedReasons.FirstOrDefault(a => a.Id == int.Parse(x.ReasonEstablishmentOpenedcode))?.Id;
                    e.ReligiousCharacterId = religiousCharacters.FirstOrDefault(a => a.Id == int.Parse(x.ReligiousCharactercode))?.Id;
                    e.ReligiousEthosId = religiousEthos.FirstOrDefault(a => a.Id == int.Parse(x.ReligiousEthoscode))?.Id;
                    e.StatusId = establishmentStatuses.FirstOrDefault(a => a.Id == int.Parse(x.EstablishmentStatuscode))?.Id;
                    e.StatutoryHighAge = x.StatutoryHighAge.ToInteger();
                    e.StatutoryLowAge = x.StatutoryLowAge.ToInteger();
                    e.UKPRN = x.UKPRN.ToInteger();
                    e.Urn = x.URN.ToInteger().Value;

                    var location = locs.Get(e.Urn);
                    if (location != null)
                    {
                        e.Easting = location.Item1;
                        e.Northing = location.Item2;
                        e.Location = location.Item3.ToDBGeography();
                    }

                    estabs.Add(e);
                    count++;
                    if (count % 1000 == 0) Console.WriteLine("Loaded up: " + count);
                });
            }

            var dt = ToDataTable(estabs);


            using (var connection = new SqlConnection(ConfigurationManager.ConnectionStrings["EdubaseSqlDb"].ConnectionString))
            {
                var bulkCopy = new SqlBulkCopy (connection, SqlBulkCopyOptions.TableLock | SqlBulkCopyOptions.FireTriggers | SqlBulkCopyOptions.KeepIdentity | SqlBulkCopyOptions.UseInternalTransaction, null );

                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    bulkCopy.ColumnMappings.Add(dt.Columns[i].ColumnName, dt.Columns[i].ColumnName);
                }

                bulkCopy.DestinationTableName = "Establishment";
                connection.Open();
                bulkCopy.WriteToServer(dt);
                connection.Close();
            }
        }

        private static void MigrateEstabLinks(EdubaseSourceEntities source, ApplicationDbContext dc)
        {
            Console.WriteLine("Importing establishments links");
            
            int count = 0;
            var batchCount = 0;

            var table = new DataTable();
            table.Columns.Add("Id", typeof(int));
            table.Columns.Add("LinkName", typeof(string));
            table.Columns.Add("LinkType", typeof(string));
            table.Columns.Add("LinkEstablishedDate", typeof(DateTime));
            table.Columns.Add("Establishment_Urn", typeof(int));
            table.Columns.Add("LinkedEstablishment_Urn", typeof(int));

            foreach (var batch in source.SchoolToSchoolLinks.Batch(1000))
            {
                batchCount++;
                Console.WriteLine("Loading batch #" + batchCount);
                batch.ForEach(x =>
                {
                    var d = x.LinkEstablishedDate.ToDateTime(new[] { "dd-MM-yyyy", "dd/MM/yyyy", "dd/MM/yy" });
                    var row = table.NewRow();
                    row["LinkName"] = (object) x.LinkName.Clean() ?? DBNull.Value;
                    row["LinkType"] = (object) x.LinkType.Clean() ?? DBNull.Value;
                    row["LinkEstablishedDate"] = d.HasValue ? d.Value : (object) DBNull.Value;
                    row["Establishment_Urn"] = x.URN.ToInteger().Value;
                    row["LinkedEstablishment_Urn"] = x.LinkURN.ToInteger().Value;
                    table.Rows.Add(row);
                    count++;
                    if (count % 1000 == 0) Console.WriteLine("Loaded up: " + count);
                });
            }
            

            using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["EdubaseSqlDb"].ConnectionString))
            {
                SqlBulkCopy bulkCopy = new SqlBulkCopy(connection, SqlBulkCopyOptions.TableLock | SqlBulkCopyOptions.UseInternalTransaction, null);
                bulkCopy.DestinationTableName = "Estab2Estab";
                connection.Open();
                bulkCopy.WriteToServer(table);
                connection.Close();
            }

            Console.WriteLine("DONE");
        }

        private static void MigrateGovernors(EdubaseSourceEntities source, ApplicationDbContext dc)
        {
            Console.WriteLine("Importing governors");

            var bods = dc.GovernorAppointingBodies.ToList();
            var roles = dc.GovernorRoles.ToList();

            int count = 0;
            var batchCount = 0;

            var cols = new List<GovMap>()
            {
                new GovMap("Id", typeof(int), x => x.GID.ToInteger().Value),
                new GovMap("EstablishmentUrn", typeof(int), x => x.URN.ToInteger().Value),
                new GovMap("Title", typeof(string), x => x.Title.SQLify()),
                new GovMap("Forename1", typeof(string), x=> x.Forename1.SQLify()),
                new GovMap("Forename2", typeof(string), x=>x.Forename2.SQLify()),
                new GovMap("Surname", typeof(string), x => x.Surname.SQLify()),
                new GovMap("AppointmentStartDate", typeof(DateTime), x => x.Dateofappointment.ToDateTime("dd/MM/yyyy").SQLify()),
                new GovMap("AppointmentEndDate", typeof(DateTime), x => x.Datetermofofficeendsended.ToDateTime("dd/MM/yyyy").SQLify()),
                new GovMap("RoleId", typeof(int), x=> (object) roles.FirstOrDefault(r => r.Name == x.Role)?.Id ?? DBNull.Value),
                new GovMap("GovernorAppointingBodyId", typeof(int), x=> (object) bods.FirstOrDefault(r => r.Name == x.Appointingbody)?.Id ?? DBNull.Value),
                new GovMap("CreatedUtc", typeof(DateTime), x => DateTime.UtcNow),
                new GovMap("LastUpdatedUtc", typeof(DateTime), x=>DateTime.UtcNow),
                new GovMap("IsDeleted", typeof(bool), x=>false),
            };

            var table = new DataTable();
            cols.ForEach(x => table.Columns.Add(x.Item1, x.Item2));

            foreach (var batch in source.Governors.Batch(1000))
            {
                batchCount++;
                Console.WriteLine("Loading batch #" + batchCount);
                batch.ForEach(x =>
                {
                    var row = table.NewRow();
                    cols.ForEach(c => row[c.Item1] = c.Item3(x));
                    table.Rows.Add(row);
                    count++;
                    if (count % 1000 == 0) Console.WriteLine("Loaded up: " + count);
                });
            }


            using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["EdubaseSqlDb"].ConnectionString))
            {
                SqlBulkCopy bulkCopy = new SqlBulkCopy(connection, SqlBulkCopyOptions.TableLock 
                    | SqlBulkCopyOptions.KeepIdentity 
                    | SqlBulkCopyOptions.UseInternalTransaction, null);
                bulkCopy.DestinationTableName = "Governor";
                connection.Open();
                bulkCopy.BulkCopyTimeout = 900;
                bulkCopy.WriteToServer(table);
                connection.Close();
            }

            Console.WriteLine("DONE");
        }

        private static void MigrateEstablishment2CompanyLinks(EdubaseSourceEntities source, ApplicationDbContext dc)
        {
            Console.WriteLine("Importing Establishment2Company links");

            int count = 0;
            var batchCount = 0;

            var table = new DataTable();
            table.Columns.Add("Id", typeof(int));
            table.Columns.Add("JoinedDate", typeof(DateTime));
            table.Columns.Add("Establishment_Urn", typeof(int));
            table.Columns.Add("Company_GroupUID", typeof(int));

            foreach (var batch in source.SchoolToCompanyLinks.Batch(1000))
            {
                batchCount++;
                Console.WriteLine("Loading batch #" + batchCount);
                batch.ForEach(x =>
                {
                    var d = x.Joineddate.ToDateTime(new[] { "dd-MM-yyyy", "dd/MM/yyyy", "dd/MM/yy" });
                    var row = table.NewRow();
                    row["JoinedDate"] = d.HasValue ? d.Value : (object)DBNull.Value;
                    row["Establishment_Urn"] = x.URN.ToInteger().Value;
                    row["Company_GroupUID"] = x.LinkedUID.ToInteger().Value;
                    table.Rows.Add(row);
                    count++;
                    if (count % 1000 == 0) Console.WriteLine("Loaded up: " + count);
                });
            }


            using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["EdubaseSqlDb"].ConnectionString))
            {
                SqlBulkCopy bulkCopy = new SqlBulkCopy(connection, SqlBulkCopyOptions.TableLock | SqlBulkCopyOptions.UseInternalTransaction, null);
                bulkCopy.DestinationTableName = "Establishment2Company";
                connection.Open();
                bulkCopy.WriteToServer(table);
                connection.Close();
            }

            Console.WriteLine("DONE");
        }

        private static T Map<T>(string id, string name) where T : LookupBase, new() => new T
        {
            Id = int.Parse(id),
            Name = name,
            CreatedUtc = DateTime.UtcNow,
            LastUpdatedUtc = DateTime.UtcNow,
            IsDeleted = false
        };

        public static DataTable ToDataTable<T>(IList<T> data)
        {
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(typeof(T));
            DataTable table = new DataTable();

            List<string> cols = new List<string>();

            foreach (PropertyDescriptor prop in properties)
            {
                if (prop.Name.Equals("LAESTAB") || prop.Name.Equals("FullAddress") || prop.Name.Equals("HeadteacherFullName")) continue;
                if (!prop.PropertyType.IsClass || prop.PropertyType == typeof(string))
                {
                    var col = prop.Name;
                    table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
                    cols.Add(col);
                }
                else if(prop.PropertyType == typeof(DbGeography))
                {
                    var col = prop.Name;
                    table.Columns.Add(col, typeof(SqlGeography));
                    cols.Add(col);
                }
                else if (prop.PropertyType == typeof(ContactDetail) || prop.PropertyType == typeof(Address))
                {
                    PropertyDescriptorCollection properties2 = TypeDescriptor.GetProperties(prop.PropertyType);
                    foreach (PropertyDescriptor prop2 in properties2)
                    {
                        var col = prop.Name + "_" + prop2.Name;
                        table.Columns.Add(prop.Name + "_" + prop2.Name, Nullable.GetUnderlyingType(prop2.PropertyType) ?? prop2.PropertyType);
                        cols.Add(col);
                    }
                }
            }

            //table.Columns.Add("TypeId", typeof(int));

            foreach (T item in data)
            {
                DataRow row = table.NewRow();

                foreach (PropertyDescriptor prop in properties)
                {
                    if (prop.Name.Equals("LAESTAB") || prop.Name.Equals("FullAddress") || prop.Name.Equals("HeadteacherFullName")) continue;
                    if (!prop.PropertyType.IsClass || prop.PropertyType == typeof(string))
                    {
                        row[prop.Name]= prop.GetValue(item) ?? DBNull.Value;
                    }
                    else if (prop.PropertyType == typeof(DbGeography))
                    {
                        row[prop.Name] = ((DbGeography)prop.GetValue(item)).ToSqlGeography().SQLify();
                    }
                    else if (prop.PropertyType == typeof(ContactDetail) || prop.PropertyType == typeof(Address))
                    {
                        var inner = prop.GetValue(item);
                        PropertyDescriptorCollection properties2 = TypeDescriptor.GetProperties(prop.PropertyType);
                        foreach (PropertyDescriptor prop2 in properties2)
                        {
                            var colName = prop.Name + "_" + prop2.Name;
                            row[colName] = prop2.GetValue(inner) ?? DBNull.Value;
                        }
                    }
                }
                table.Rows.Add(row);
            }
            return table;
        }
    }
}
