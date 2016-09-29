using Edubase.Data.Entity;
using MoreLinq;
using Edubase.Data.Entity.Lookups;
using System.Data.Entity.Migrations;
using EdubaseDiocese = Edubase.Data.Entity.Lookups.Diocese;
using EdubaseGender = Edubase.Data.Entity.Lookups.Gender;
using EdubaseGroupType = Edubase.Data.Entity.Lookups.GroupType;
using System.Transactions;
using System.Data.Entity;
using System;
using System.Threading.Tasks;

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
