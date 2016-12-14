#if (QA)
using Edubase.Data.Entity;
using Edubase.Data.Entity.Lookups;
using System.Data;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.OData;

namespace Edubase.Web.UI.Controllers.Api
{
    public class EstablishmentsController : ODataController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: odata/Establishments
        [EnableQuery(MaxTop = 100, PageSize = 10)]
        public IQueryable<Establishment> GetEstablishments()
        {
            return db.Establishments;
        }

        // GET: odata/Establishments(5)
        [EnableQuery]
        public SingleResult<Establishment> GetEstablishment([FromODataUri] int key)
        {
            return SingleResult.Create(db.Establishments.Where(establishment => establishment.Urn == key));
        }

        // PUT: odata/Establishments(5)
        public async Task<IHttpActionResult> Put([FromODataUri] int key, Delta<Establishment> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Establishment establishment = await db.Establishments.FindAsync(key);
            if (establishment == null)
            {
                return NotFound();
            }

            patch.Put(establishment);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EstablishmentExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(establishment);
        }

        // POST: odata/Establishments
        public async Task<IHttpActionResult> Post(Establishment establishment)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Establishments.Add(establishment);
            await db.SaveChangesAsync();

            return Created(establishment);
        }

        // PATCH: odata/Establishments(5)
        [AcceptVerbs("PATCH", "MERGE")]
        public async Task<IHttpActionResult> Patch([FromODataUri] int key, Delta<Establishment> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Establishment establishment = await db.Establishments.FindAsync(key);
            if (establishment == null)
            {
                return NotFound();
            }

            patch.Patch(establishment);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EstablishmentExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(establishment);
        }

        // DELETE: odata/Establishments(5)
        public async Task<IHttpActionResult> Delete([FromODataUri] int key)
        {
            Establishment establishment = await db.Establishments.FindAsync(key);
            if (establishment == null)
            {
                return NotFound();
            }

            db.Establishments.Remove(establishment);
            await db.SaveChangesAsync();

            return StatusCode(HttpStatusCode.NoContent);
        }

        // GET: odata/Establishments(5)/AdministrativeDistrict
        [EnableQuery]
        public SingleResult<LookupDistrictAdministrative> GetAdministrativeDistrict([FromODataUri] int key)
        {
            return SingleResult.Create(db.Establishments.Where(m => m.Urn == key).Select(m => m.AdministrativeDistrict));
        }

        // GET: odata/Establishments(5)/AdministrativeWard
        [EnableQuery]
        public SingleResult<LookupAdministrativeWard> GetAdministrativeWard([FromODataUri] int key)
        {
            return SingleResult.Create(db.Establishments.Where(m => m.Urn == key).Select(m => m.AdministrativeWard));
        }

        // GET: odata/Establishments(5)/AdmissionsPolicy
        [EnableQuery]
        public SingleResult<LookupAdmissionsPolicy> GetAdmissionsPolicy([FromODataUri] int key)
        {
            return SingleResult.Create(db.Establishments.Where(m => m.Urn == key).Select(m => m.AdmissionsPolicy));
        }

        // GET: odata/Establishments(5)/BSOInspectorate
        [EnableQuery]
        public SingleResult<LookupInspectorateName> GetBSOInspectorate([FromODataUri] int key)
        {
            return SingleResult.Create(db.Establishments.Where(m => m.Urn == key).Select(m => m.BSOInspectorate));
        }

        // GET: odata/Establishments(5)/CASWard
        [EnableQuery]
        public SingleResult<LookupCASWard> GetCASWard([FromODataUri] int key)
        {
            return SingleResult.Create(db.Establishments.Where(m => m.Urn == key).Select(m => m.CASWard));
        }

        // GET: odata/Establishments(5)/ChildcareFacilities
        [EnableQuery]
        public SingleResult<LookupChildcareFacilities> GetChildcareFacilities([FromODataUri] int key)
        {
            return SingleResult.Create(db.Establishments.Where(m => m.Urn == key).Select(m => m.ChildcareFacilities));
        }

        // GET: odata/Establishments(5)/Diocese
        [EnableQuery]
        public SingleResult<LookupDiocese> GetDiocese([FromODataUri] int key)
        {
            return SingleResult.Create(db.Establishments.Where(m => m.Urn == key).Select(m => m.Diocese));
        }

        // GET: odata/Establishments(5)/EducationPhase
        [EnableQuery]
        public SingleResult<LookupEducationPhase> GetEducationPhase([FromODataUri] int key)
        {
            return SingleResult.Create(db.Establishments.Where(m => m.Urn == key).Select(m => m.EducationPhase));
        }

        // GET: odata/Establishments(5)/EstablishmentType
        [EnableQuery]
        public SingleResult<LookupEstablishmentType> GetEstablishmentType([FromODataUri] int key)
        {
            return SingleResult.Create(db.Establishments.Where(m => m.Urn == key).Select(m => m.EstablishmentType));
        }

        // GET: odata/Establishments(5)/EstablishmentTypeGroup
        [EnableQuery]
        public SingleResult<LookupEstablishmentTypeGroup> GetEstablishmentTypeGroup([FromODataUri] int key)
        {
            return SingleResult.Create(db.Establishments.Where(m => m.Urn == key).Select(m => m.EstablishmentTypeGroup));
        }

        // GET: odata/Establishments(5)/FurtherEducationType
        [EnableQuery]
        public SingleResult<LookupFurtherEducationType> GetFurtherEducationType([FromODataUri] int key)
        {
            return SingleResult.Create(db.Establishments.Where(m => m.Urn == key).Select(m => m.FurtherEducationType));
        }

        // GET: odata/Establishments(5)/Gender
        [EnableQuery]
        public SingleResult<LookupGender> GetGender([FromODataUri] int key)
        {
            return SingleResult.Create(db.Establishments.Where(m => m.Urn == key).Select(m => m.Gender));
        }

        // GET: odata/Establishments(5)/GovernmentOfficeRegion
        [EnableQuery]
        public SingleResult<LookupGovernmentOfficeRegion> GetGovernmentOfficeRegion([FromODataUri] int key)
        {
            return SingleResult.Create(db.Establishments.Where(m => m.Urn == key).Select(m => m.GovernmentOfficeRegion));
        }

        // GET: odata/Establishments(5)/GSSLA
        [EnableQuery]
        public SingleResult<LookupGSSLA> GetGSSLA([FromODataUri] int key)
        {
            return SingleResult.Create(db.Establishments.Where(m => m.Urn == key).Select(m => m.GSSLA));
        }

        // GET: odata/Establishments(5)/HeadTitle
        [EnableQuery]
        public SingleResult<LookupHeadTitle> GetHeadTitle([FromODataUri] int key)
        {
            return SingleResult.Create(db.Establishments.Where(m => m.Urn == key).Select(m => m.HeadTitle));
        }

        // GET: odata/Establishments(5)/Inspectorate
        [EnableQuery]
        public SingleResult<LookupInspectorate> GetInspectorate([FromODataUri] int key)
        {
            return SingleResult.Create(db.Establishments.Where(m => m.Urn == key).Select(m => m.Inspectorate));
        }

        // GET: odata/Establishments(5)/LocalAuthority
        [EnableQuery]
        public SingleResult<LocalAuthority> GetLocalAuthority([FromODataUri] int key)
        {
            return SingleResult.Create(db.Establishments.Where(m => m.Urn == key).Select(m => m.LocalAuthority));
        }

        // GET: odata/Establishments(5)/LSOA
        [EnableQuery]
        public SingleResult<LookupLSOA> GetLSOA([FromODataUri] int key)
        {
            return SingleResult.Create(db.Establishments.Where(m => m.Urn == key).Select(m => m.LSOA));
        }

        // GET: odata/Establishments(5)/MSOA
        [EnableQuery]
        public SingleResult<LookupMSOA> GetMSOA([FromODataUri] int key)
        {
            return SingleResult.Create(db.Establishments.Where(m => m.Urn == key).Select(m => m.MSOA));
        }

        // GET: odata/Establishments(5)/ParliamentaryConstituency
        [EnableQuery]
        public SingleResult<LookupParliamentaryConstituency> GetParliamentaryConstituency([FromODataUri] int key)
        {
            return SingleResult.Create(db.Establishments.Where(m => m.Urn == key).Select(m => m.ParliamentaryConstituency));
        }

        // GET: odata/Establishments(5)/ProvisionBoarding
        [EnableQuery]
        public SingleResult<LookupProvisionBoarding> GetProvisionBoarding([FromODataUri] int key)
        {
            return SingleResult.Create(db.Establishments.Where(m => m.Urn == key).Select(m => m.ProvisionBoarding));
        }

        // GET: odata/Establishments(5)/ProvisionNursery
        [EnableQuery]
        public SingleResult<LookupProvisionNursery> GetProvisionNursery([FromODataUri] int key)
        {
            return SingleResult.Create(db.Establishments.Where(m => m.Urn == key).Select(m => m.ProvisionNursery));
        }

        // GET: odata/Establishments(5)/ProvisionOfficialSixthForm
        [EnableQuery]
        public SingleResult<LookupProvisionOfficialSixthForm> GetProvisionOfficialSixthForm([FromODataUri] int key)
        {
            return SingleResult.Create(db.Establishments.Where(m => m.Urn == key).Select(m => m.ProvisionOfficialSixthForm));
        }

        // GET: odata/Establishments(5)/ProvisionSpecialClasses
        [EnableQuery]
        public SingleResult<LookupProvisionSpecialClasses> GetProvisionSpecialClasses([FromODataUri] int key)
        {
            return SingleResult.Create(db.Establishments.Where(m => m.Urn == key).Select(m => m.ProvisionSpecialClasses));
        }

        // GET: odata/Establishments(5)/PRUEBD
        [EnableQuery]
        public SingleResult<LookupPRUEBD> GetPRUEBD([FromODataUri] int key)
        {
            return SingleResult.Create(db.Establishments.Where(m => m.Urn == key).Select(m => m.PRUEBD));
        }

        // GET: odata/Establishments(5)/PruEducatedByOthers
        [EnableQuery]
        public SingleResult<LookupPruEducatedByOthers> GetPruEducatedByOthers([FromODataUri] int key)
        {
            return SingleResult.Create(db.Establishments.Where(m => m.Urn == key).Select(m => m.PruEducatedByOthers));
        }

        // GET: odata/Establishments(5)/PruFulltimeProvision
        [EnableQuery]
        public SingleResult<LookupPruFulltimeProvision> GetPruFulltimeProvision([FromODataUri] int key)
        {
            return SingleResult.Create(db.Establishments.Where(m => m.Urn == key).Select(m => m.PruFulltimeProvision));
        }

        // GET: odata/Establishments(5)/PRUSEN
        [EnableQuery]
        public SingleResult<LookupPRUSEN> GetPRUSEN([FromODataUri] int key)
        {
            return SingleResult.Create(db.Establishments.Where(m => m.Urn == key).Select(m => m.PRUSEN));
        }

        // GET: odata/Establishments(5)/ReasonEstablishmentClosed
        [EnableQuery]
        public SingleResult<LookupReasonEstablishmentClosed> GetReasonEstablishmentClosed([FromODataUri] int key)
        {
            return SingleResult.Create(db.Establishments.Where(m => m.Urn == key).Select(m => m.ReasonEstablishmentClosed));
        }

        // GET: odata/Establishments(5)/ReasonEstablishmentOpened
        [EnableQuery]
        public SingleResult<LookupReasonEstablishmentOpened> GetReasonEstablishmentOpened([FromODataUri] int key)
        {
            return SingleResult.Create(db.Establishments.Where(m => m.Urn == key).Select(m => m.ReasonEstablishmentOpened));
        }

        // GET: odata/Establishments(5)/ReligiousCharacter
        [EnableQuery]
        public SingleResult<LookupReligiousCharacter> GetReligiousCharacter([FromODataUri] int key)
        {
            return SingleResult.Create(db.Establishments.Where(m => m.Urn == key).Select(m => m.ReligiousCharacter));
        }

        // GET: odata/Establishments(5)/ReligiousEthos
        [EnableQuery]
        public SingleResult<LookupReligiousEthos> GetReligiousEthos([FromODataUri] int key)
        {
            return SingleResult.Create(db.Establishments.Where(m => m.Urn == key).Select(m => m.ReligiousEthos));
        }

        // GET: odata/Establishments(5)/RSCRegion
        [EnableQuery]
        public SingleResult<LocalAuthority> GetRSCRegion([FromODataUri] int key)
        {
            return SingleResult.Create(db.Establishments.Where(m => m.Urn == key).Select(m => m.RSCRegion));
        }

        // GET: odata/Establishments(5)/Section41Approved
        [EnableQuery]
        public SingleResult<LookupSection41Approved> GetSection41Approved([FromODataUri] int key)
        {
            return SingleResult.Create(db.Establishments.Where(m => m.Urn == key).Select(m => m.Section41Approved));
        }

        // GET: odata/Establishments(5)/SEN1
        [EnableQuery]
        public SingleResult<LookupSpecialEducationNeeds> GetSEN1([FromODataUri] int key)
        {
            return SingleResult.Create(db.Establishments.Where(m => m.Urn == key).Select(m => m.SEN1));
        }

        // GET: odata/Establishments(5)/SEN2
        [EnableQuery]
        public SingleResult<LookupSpecialEducationNeeds> GetSEN2([FromODataUri] int key)
        {
            return SingleResult.Create(db.Establishments.Where(m => m.Urn == key).Select(m => m.SEN2));
        }

        // GET: odata/Establishments(5)/SEN3
        [EnableQuery]
        public SingleResult<LookupSpecialEducationNeeds> GetSEN3([FromODataUri] int key)
        {
            return SingleResult.Create(db.Establishments.Where(m => m.Urn == key).Select(m => m.SEN3));
        }

        // GET: odata/Establishments(5)/SEN4
        [EnableQuery]
        public SingleResult<LookupSpecialEducationNeeds> GetSEN4([FromODataUri] int key)
        {
            return SingleResult.Create(db.Establishments.Where(m => m.Urn == key).Select(m => m.SEN4));
        }

        // GET: odata/Establishments(5)/Status
        [EnableQuery]
        public SingleResult<LookupEstablishmentStatus> GetStatus([FromODataUri] int key)
        {
            return SingleResult.Create(db.Establishments.Where(m => m.Urn == key).Select(m => m.Status));
        }

        // GET: odata/Establishments(5)/TeenageMothersProvision
        [EnableQuery]
        public SingleResult<LookupTeenageMothersProvision> GetTeenageMothersProvision([FromODataUri] int key)
        {
            return SingleResult.Create(db.Establishments.Where(m => m.Urn == key).Select(m => m.TeenageMothersProvision));
        }

        // GET: odata/Establishments(5)/TypeOfResourcedProvision
        [EnableQuery]
        public SingleResult<LookupTypeOfResourcedProvision> GetTypeOfResourcedProvision([FromODataUri] int key)
        {
            return SingleResult.Create(db.Establishments.Where(m => m.Urn == key).Select(m => m.TypeOfResourcedProvision));
        }

        // GET: odata/Establishments(5)/UrbanRural
        [EnableQuery]
        public SingleResult<LookupUrbanRural> GetUrbanRural([FromODataUri] int key)
        {
            return SingleResult.Create(db.Establishments.Where(m => m.Urn == key).Select(m => m.UrbanRural));
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool EstablishmentExists(int key)
        {
            return db.Establishments.Count(e => e.Urn == key) > 0;
        }
    }
}
#endif