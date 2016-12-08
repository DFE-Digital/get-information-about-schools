using AutoMapper;
using MoreLinq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Spatial;
using System.Data.SqlClient;
using System.Linq;

namespace Edubase.Import
{
    using Common;
    using Data;
    using Data.Entity;
    using Data.Entity.Lookups;
    using Data.Migrations;
    using Helpers;
    using Mapping;
    using Migrations;
    using Services.Domain;
    using System.IO;

    public class Program
    {
        private static Dictionary<Type, DataTable> _tables;
        private static IMapper _mapper;
        
        private static List<string> _lookupTableNames = new List<string>();

        static void Main(string[] args)
        {
            using (Timing("Recreating the DB"))
            {
                Database.SetInitializer(new DropRecreateDatabase());
                _tables = ApplicationDbContext.Create().GenerateDataTables();
            }

            using (var source = new EdubaseSourceDataEntities())
            {
                using (Timing("Loading Ofsted ratings and mapping configuration"))
                {
                    var ofstedRatings = source.Ofstedratings.ToDictionary(x => x.URN.ToInteger().Value);
                    var laContacts = source.Cclacontacts.ToDictionary(x => x.code.ToInteger().Value);
                    _mapper = MappingConfiguration.Create(ofstedRatings, laContacts);
                }

                Disposer.Using(CreateSqlConnection, x => x.Open(), x => x.Close(), connection =>
                {
                    MigrateAllData(connection, source);
                });
            }

            using (Timing("Seeding DB with app data"))
            using (var context = new ApplicationDbContext())
                new DbSeeder().Seed(context);

            using (Timing("Auto-generating Enum constructs"))
                GenerateEnumConstructs();

        }

        private static void GenerateEnumConstructs()
        {
            #region Template
            const string TEMPLATE =
            @"
namespace Edubase.Services.Enums
{
    public enum [NAME]
    {
        [ITEMS]
    }
}   
";
            #endregion

            _lookupTableNames.Add(_tables.Get<Data.Entity.LocalAuthority>().TableName);
            
            using (var dc = new ApplicationDbContext())
            {
                foreach (var tableName in _lookupTableNames)
                {
                    var count = dc.Database.SqlQuery<int>($"SELECT COUNT(1) FROM {tableName}").FirstOrDefault();
                    if (count < 700)
                    {
                        var query = dc.Database.SqlQuery<LookupDto>($"SELECT Id, Name FROM {tableName}");
                        var items = query.ToList();
                        string enumName = string.Concat("e", tableName);
                        string body = string.Join("\r\n\t\t", items.Select(x => string.Concat(x.Name.AsEnumName(), " = ", x.Id, ",")));
                        var code = TEMPLATE.Replace("[NAME]", enumName).Replace("[ITEMS]", body);
                        File.WriteAllText(string.Concat(enumName, ".cs"), code);
                    }
                }
            }

            Console.WriteLine("REMEMBER TO IMPORT THE ENUMS INTO THE CODEBASE! bye.");
        }

        private static void MigrateAllData(SqlConnection connection, EdubaseSourceDataEntities source)
        {
            MigrateAllLookupData(connection, source);
            MigrateDataInBatches<Data.Entity.LocalAuthority, LocalAuthority>("LAs", source.LocalAuthority, connection, 100, BulkCopyOptionPreserveIds);
            MigrateDataInBatches<Establishment, Establishments>("Establishments", source.Establishments, connection, 10000, BulkCopyOptionPreserveIds);
            MigrateDataInBatches<GroupCollection, GroupData>("Trusts", source.GroupData, connection, 2000, BulkCopyOptionPreserveIds);
            MigrateDataInBatches<Governor, Governors>("Governors", source.Governors, connection, 2000, BulkCopyOptionPreserveIds);
            MigrateDataInBatches<EstablishmentLink, Establishmentlinks>("Establishment Links", source.Establishmentlinks, connection, 2000, BulkCopyOptionNewIds);
            MigrateDataInBatches<EstablishmentGroup, GroupLinks>("Trust/Establishment Links", source.GroupLinks, connection, 2000, BulkCopyOptionNewIds);    
        }
        

        private static void MigrateDataInBatches<TDestEntity, TSourceEntity>(string label, IQueryable<TSourceEntity> sourceEntities, SqlConnection connection, int batchSize, SqlBulkCopyOptions options)
        {
            using (Timing(label))
            {
                var dataTable = _tables.Get<TDestEntity>();
                new SqlCommand($"DELETE FROM {dataTable.TableName}", connection).ExecuteNonQuery();
                
                int count = 0;
                sourceEntities.Batch(batchSize).ForEach(batch =>
                {
                    var entities = batch.Select(l => _mapper.Map<TSourceEntity, TDestEntity>(l)).ToArray();
                    FillDataTable(entities.Cast<object>(), dataTable);
                    Import(dataTable, connection, options);
                    dataTable.Clear();
                    count += batch.Count();
                    Console.WriteLine($"\t{count} imported");
                });
            }
        }

        private static void FillDataTable(IEnumerable<object> source, DataTable dataTable)
        {
            foreach (var item in source)
            {
                dataTable.CreateRow(x =>
                {
                    foreach (var column in dataTable.Columns.Cast<DataColumn>())
                    {
                        var name = column.ColumnName;
                        var value = (name.Contains("_")
                            ? item.GetPropertyValue(name.GetPart("_")).GetPropertyValue(name.GetPart("_", 1))
                            : item.GetPropertyValue(name)).SQLify();

                        if (value.GetType() == typeof(DbGeography))
                        {
                            value = (value as DbGeography).ToSqlGeography().SQLify();
                        }

                        x[column] = value;
                    }
                });
            }
        }

        private static void MigrateAllLookupData(SqlConnection connection, EdubaseSourceDataEntities source)
        {
            using (Timing("lookup tables"))
            {
                MigrateLookup<LookupAdmissionsPolicy>(source.Admissionspolicy, connection);
                MigrateLookup<LookupAccommodationChanged>(source.Accomodationchanged, connection);
                MigrateLookup<LookupGovernorAppointingBody>(source.Appointingbody, connection);
                MigrateLookup<LookupProvisionBoarding>(source.Boarders, connection);
                MigrateLookup<LookupBoardingEstablishment>(source.Boardingestablishment, connection);
                MigrateLookup<LookupCCGovernance>(source.Ccgovernance, connection);
                MigrateLookup<LookupCCOperationalHours>(source.Ccoperationalhours, connection);
                MigrateLookup<LookupCCPhaseType>(source.Ccphasetype, connection);
                MigrateLookup<LookupChildcareFacilities>(source.Childcarefacilities, connection);
                MigrateLookup<LookupDiocese>(source.Diocese, connection);
                MigrateLookup<LookupDirectProvisionOfEarlyYears>(source.Directprovisionofearlyyears, connection);
                MigrateLookup<LookupEstablishmentStatus>(source.Establishmentstatus, connection);
                MigrateLookup<LookupFurtherEducationType>(source.Furthereducationtype, connection);
                MigrateLookup<LookupGender>(source.Gender, connection);
                MigrateLookup<LookupGroupType>(source.GroupType, connection);
                MigrateLookup<LookupHeadTitle>(source.Headtitle, connection);
                MigrateLookup<LookupIndependentSchoolType>(source.Independentschooltype, connection);
                MigrateLookup<LookupInspectorate>(source.Inspectorate, connection);
                MigrateLookup<LookupInspectorateName>(source.Inspectoratename, connection);
                MigrateLookup<LookupLocalGovernors>(source.Localgovernors, connection);
                MigrateLookup<LookupNationality>(source.Nationality, connection);
                MigrateLookup<LookupProvisionNursery>(source.Nurseryprovision, connection);
                MigrateLookup<LookupProvisionOfficialSixthForm>(source.Officialsixthform, connection);
                MigrateLookup<LookupEducationPhase>(source.Phaseofeducation, connection);
                MigrateLookup<LookupPRUEBD>(source.PRUEBD, connection);
                MigrateLookup<LookupPruEducatedByOthers>(source.Prueducatedbyothers, connection);
                MigrateLookup<LookupPruFulltimeProvision>(source.Prufulltimeprovision, connection);
                MigrateLookup<LookupPRUSEN>(source.PRUSEN, connection);
                MigrateLookup<LookupReasonEstablishmentClosed>(source.Reasonestablishmentclosed, connection);
                MigrateLookup<LookupReasonEstablishmentOpened>(source.Reasonestablishmentopened, connection);
                MigrateLookup<LookupReligiousCharacter>(source.Religiouscharacter, connection);
                MigrateLookup<LookupReligiousEthos>(source.Religiousethos, connection);
                MigrateLookup<LookupResourcedProvision>(source.Resourcedprovision, connection);
                MigrateLookup<LookupSection41Approved>(source.Section41approved, connection);
                MigrateLookup<LookupProvisionSpecialClasses>(source.Specialclasses, connection);
                MigrateLookup<LookupSpecialEducationNeeds>(source.Specialeducationaneeds, connection);
                MigrateLookup<LookupTeenageMothersProvision>(source.Teenagemothers, connection);
                MigrateLookup<LookupTypeOfResourcedProvision>(source.Typeofresourcedprovision, connection);
                MigrateLookup<LookupEstablishmentType>(source.Typeofestablishment, connection);
                MigrateLookup<LookupCCDisadvantagedArea>(source.Ccdisadvantagedarea, connection);
                MigrateLookup<LookupEstablishmentTypeGroup>(source.Establishmenttypegroup, connection);
                MigrateLookup<LookupGovernmentOfficeRegion>(source.GOR, connection);
                MigrateLookup<LookupDistrictAdministrative>(source.Districtadministrative, connection);
                MigrateLookup<LookupParliamentaryConstituency>(source.Parliamentaryconstituency, connection);
                MigrateLookup<LookupUrbanRural>(source.Urbanrural, connection);
                MigrateLookup<LookupGSSLA>(source.Gsslacode, connection);
                MigrateLookup<LookupCASWard>(source.Casward, connection);
                MigrateLookup<LookupMSOA>(source.MSOA, connection);
                MigrateLookup<LookupLSOA>(source.LSOA, connection);
                MigrateLookup<LookupAdministrativeWard>(source.Administrativeward, connection);
                MigrateLookup<LookupCCGroupLead>(source.Ccgrouplead, connection);
                
                var childrensCentresGroupflags = source.Childrenscentresgroupflag.ToList()
                    .Select(x => new { Name = x.ChildrensCentresGroupFlag1.Clean(), Code = x.code }).ToList();
                MigrateLookup<LookupDeliveryModel>(childrensCentresGroupflags, connection);

                var governorRoles = source.Governors.Where(x => !string.IsNullOrEmpty(x.Role))
                    .Select(x => x.Role).Distinct().ToList()
                    .Select(x => new { Name = x.Clean() }).ToList();
                MigrateLookup<LookupGovernorRole>(governorRoles, connection);

                var linkTypes = source.Establishmentlinks.Where(x => !string.IsNullOrEmpty(x.LinkType))
                    .Select(x => x.LinkType).Distinct().ToList()
                    .Select(x => new { Name = x.Clean() }).ToList();
                MigrateLookup<LookupEstablishmentLinkType>(linkTypes, connection);

                var groupStatuses = source.GroupData.Where(x => !string.IsNullOrEmpty(x.GroupStatuscode))
                    .Select(x => new { Name = x.GroupStatus, Code = x.GroupStatuscode }).Distinct().ToList()
                    .Select(x => new { Name = x.Name.Clean(), Code = ProcessCode(x.Code.Clean()) }).ToList();
                MigrateLookup<LookupGroupStatus>(groupStatuses, connection);
            }
        }

        public static LookupBase ConvertToLookup<T>(dynamic source) where T : LookupBase, new()
        {
            const string COL_CODE = "Code";
            const string COL_ORDER = "C_Order";
            const string COL_NAME = "Name";

            var retVal = new T();

            retVal.Code = ProcessCode(Helper.GetPropertyValue(source, COL_CODE, "GroupTypecode"));
            retVal.Name = Helper.GetPropertyValue(source, COL_NAME, "GroupType1");
            retVal.DisplayOrder = Helper.ToShort(Helper.GetPropertyValue(source, COL_ORDER));

            return retVal;
        }
        
        public static DataTable ToLookupDataTable<T>(IEnumerable<LookupBase> data, Dictionary<Type, DataTable> tables)
        {
            var table = tables.Get<T>();
            data.ForEach(lookup => {
                table.CreateRow(x =>
                {
                    lookup.Set(x, l => l.Code)
                        .Set(l => l.Name)
                        .Set(l => l.DisplayOrder)
                        .Set(l => l.IsDeleted)
                        .Set(l => l.CreatedUtc)
                        .Set(l => l.LastUpdatedUtc);
                });
            });

            _lookupTableNames.Add(table.TableName);

            return table;
        }
        
        private static void MigrateLookup<TDest>(IEnumerable sourceData, SqlConnection connection)
            where TDest : LookupBase, new() => Migrate(CreateLookupDataTable<TDest>(sourceData, _tables), connection);

        private static void Migrate(DataTable table, SqlConnection connection) => Migrate(table, connection, BulkCopyOptionNewIds);

        private static void Migrate(DataTable table, SqlConnection connection, SqlBulkCopyOptions option)
        {
            if (new SqlCommand($"SELECT COUNT(1) FROM {table.TableName}", connection).ExecuteScalar()?.ToString().ToInteger().GetValueOrDefault() == 0)
            {
                Import(table, connection, option);
            }
            else Console.WriteLine($"\t >> Ignoring {table.TableName} as it has data in it.");
        }

        private static void Import(DataTable table, SqlConnection connection, SqlBulkCopyOptions option)
        {
            using (var bulkCopy = new SqlBulkCopy(connection, option, null) { DestinationTableName = table.TableName, BulkCopyTimeout = 900 })
                bulkCopy.WriteToServer(table);
        }

        private static SqlConnection CreateSqlConnection() => new SqlConnection(ConfigurationManager.ConnectionStrings["EdubaseSqlDb"].ConnectionString);

        private static SqlBulkCopyOptions BulkCopyOptionPreserveIds => SqlBulkCopyOptions.TableLock
                        | SqlBulkCopyOptions.KeepIdentity
                        | SqlBulkCopyOptions.UseInternalTransaction;

        private static SqlBulkCopyOptions BulkCopyOptionNewIds => SqlBulkCopyOptions.TableLock
                        | SqlBulkCopyOptions.UseInternalTransaction;

        private static DataTable CreateLookupDataTable<T>(IEnumerable data, Dictionary<Type, DataTable> tables) where T : LookupBase, new()
        {
            var items = data.Cast<object>().ToList().Select(x => ConvertToLookup<T>(x));
            var table = ToLookupDataTable<T>(items, tables);
            return table;
        }

        private static DataTable CreateDataTable<T>(IEnumerable data, Dictionary<Type, DataTable> tables) where T : LookupBase, new()
        {
            var items = data.Cast<object>().ToList().Select(x => ConvertToLookup<T>(x));
            var table = ToLookupDataTable<T>(items, tables);
            return table;
        }

        private static IDisposable Timing(string label)
            => Disposer.Timed(() => Console.WriteLine($"Importing {label}"), ms => Console.WriteLine($"...done in {ms}ms"));

        public static string ProcessCode(string code) => code.IsInteger() ? code.ToInteger().ToString() : code; 

    }
}
