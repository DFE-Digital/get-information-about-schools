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
    public class GovernorsController : ODataController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: odata/Governors
        [EnableQuery(MaxTop=100, PageSize=10)]
        public IQueryable<Governor> GetGovernors()
        {
            return db.Governors;
        }

        // GET: odata/Governors(5)
        [EnableQuery]
        public SingleResult<Governor> GetGovernor([FromODataUri] int key)
        {
            return SingleResult.Create(db.Governors.Where(governor => governor.Id == key));
        }

        // PUT: odata/Governors(5)
        public async Task<IHttpActionResult> Put([FromODataUri] int key, Delta<Governor> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Governor governor = await db.Governors.FindAsync(key);
            if (governor == null)
            {
                return NotFound();
            }

            patch.Put(governor);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!GovernorExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(governor);
        }

        // POST: odata/Governors
        public async Task<IHttpActionResult> Post(Governor governor)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Governors.Add(governor);
            await db.SaveChangesAsync();

            return Created(governor);
        }

        // PATCH: odata/Governors(5)
        [AcceptVerbs("PATCH", "MERGE")]
        public async Task<IHttpActionResult> Patch([FromODataUri] int key, Delta<Governor> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Governor governor = await db.Governors.FindAsync(key);
            if (governor == null)
            {
                return NotFound();
            }

            patch.Patch(governor);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!GovernorExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(governor);
        }

        // DELETE: odata/Governors(5)
        public async Task<IHttpActionResult> Delete([FromODataUri] int key)
        {
            Governor governor = await db.Governors.FindAsync(key);
            if (governor == null)
            {
                return NotFound();
            }

            db.Governors.Remove(governor);
            await db.SaveChangesAsync();

            return StatusCode(HttpStatusCode.NoContent);
        }

        // GET: odata/Governors(5)/AppointingBody
        [EnableQuery]
        public SingleResult<LookupGovernorAppointingBody> GetAppointingBody([FromODataUri] int key)
        {
            return SingleResult.Create(db.Governors.Where(m => m.Id == key).Select(m => m.AppointingBody));
        }

        // GET: odata/Governors(5)/Establishment
        [EnableQuery]
        public SingleResult<Establishment> GetEstablishment([FromODataUri] int key)
        {
            return SingleResult.Create(db.Governors.Where(m => m.Id == key).Select(m => m.Establishment));
        }

        // GET: odata/Governors(5)/Group
        [EnableQuery]
        public SingleResult<GroupCollection> GetGroup([FromODataUri] int key)
        {
            return SingleResult.Create(db.Governors.Where(m => m.Id == key).Select(m => m.Group));
        }

        // GET: odata/Governors(5)/Role
        [EnableQuery]
        public SingleResult<LookupGovernorRole> GetRole([FromODataUri] int key)
        {
            return SingleResult.Create(db.Governors.Where(m => m.Id == key).Select(m => m.Role));
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool GovernorExists(int key)
        {
            return db.Governors.Count(e => e.Id == key) > 0;
        }
    }
}
#endif