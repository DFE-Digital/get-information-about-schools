using Edubase.Data.Entity;
using Edubase.Data.Entity.Lookups;
using System;
using System.Linq;
using Edubase.Common;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using MoreLinq;

namespace Edubase.Import
{
    using Services;
    using Helpers;
    using System.Collections;
    using System.Diagnostics;
    using System.Linq.Expressions;
    using Mapping;
    using AutoMapper;
    using System.Data.Entity.Spatial;
    using Microsoft.SqlServer.Types;

    public class Program
    {
        private static Dictionary<Type, DataTable> _tables;
        private static IMapper _mapper;

        static void Main(string[] args)
        {
            _mapper = MappingConfiguration.Create();
            _tables = new ApplicationDbContext().GenerateDataTables();

            using (var source = new EdubaseSourceDataEntities())
            {
                Disposer.Using(CreateSqlConnection, x => x.Open(), x => x.Close(), connection =>
                {
                    MigrateAllData(connection, source);
                });
            }
        }

        private static void MigrateAllData(SqlConnection connection, EdubaseSourceDataEntities source)
        {
            MigrateAllLookupData(connection, source);
            MigrateEstablishmentsData(connection, source);

            // TODO:
            /* - Import establishments
             * - Import estab2estab links
             * - Import trusts
             * - Import trust2estab links
             * - Import Governors
             */




        }

        private static void MigrateEstablishmentsData(SqlConnection connection, EdubaseSourceDataEntities source)
        {
            Console.WriteLine("Importing establishments");
            var sw = Stopwatch.StartNew();

            var dataTable = _tables.Get<Establishment>();
            new SqlCommand($"DELETE FROM {dataTable.TableName}", connection).ExecuteNonQuery();

            const int BATCH_SIZE = 1000;
            int count = 0;
            source.Establishments.Batch(BATCH_SIZE).ForEach(batch =>
            {
                var estabs = batch.Select(e => _mapper.Map<Establishments, Establishment>(e)).ToArray();
                FillDataTable(estabs, dataTable);
                Import(dataTable, connection, BulkCopyOptionPreserveIds);
                dataTable.Clear();
                count += batch.Count();
                Console.WriteLine($"\t{count} imported");
            });
            
            Console.WriteLine($"...done in {sw.ElapsedMilliseconds}ms");
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
            Console.WriteLine("Importing lookups");
            var sw = Stopwatch.StartNew();
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
            Console.WriteLine($"...done in {sw.ElapsedMilliseconds}ms");
        }

        public static LookupBase ConvertToLookup<T>(dynamic source) where T : LookupBase, new()
        {
            const string COL_CODE = "Code";
            const string COL_ORDER = "C_Order";
            const string COL_NAME = "Name";

            var retVal = new T();

            retVal.Code = Helper.GetPropertyValue(source, COL_CODE, "GroupTypecode");
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




    }
}
