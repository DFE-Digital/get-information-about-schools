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
    public class GroupsController : ODataController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: odata/Groups
        [EnableQuery(MaxTop = 100, PageSize = 10)]
        public IQueryable<GroupCollection> GetGroups()
        {
            return db.Groups;
        }

        // GET: odata/Groups(5)
        [EnableQuery]
        public SingleResult<GroupCollection> GetGroupCollection([FromODataUri] int key)
        {
            return SingleResult.Create(db.Groups.Where(groupCollection => groupCollection.GroupUID == key));
        }

        // PUT: odata/Groups(5)
        public async Task<IHttpActionResult> Put([FromODataUri] int key, Delta<GroupCollection> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            GroupCollection groupCollection = await db.Groups.FindAsync(key);
            if (groupCollection == null)
            {
                return NotFound();
            }

            patch.Put(groupCollection);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!GroupCollectionExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(groupCollection);
        }

        // POST: odata/Groups
        public async Task<IHttpActionResult> Post(GroupCollection groupCollection)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Groups.Add(groupCollection);
            await db.SaveChangesAsync();

            return Created(groupCollection);
        }

        // PATCH: odata/Groups(5)
        [AcceptVerbs("PATCH", "MERGE")]
        public async Task<IHttpActionResult> Patch([FromODataUri] int key, Delta<GroupCollection> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            GroupCollection groupCollection = await db.Groups.FindAsync(key);
            if (groupCollection == null)
            {
                return NotFound();
            }

            patch.Patch(groupCollection);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!GroupCollectionExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(groupCollection);
        }

        // DELETE: odata/Groups(5)
        public async Task<IHttpActionResult> Delete([FromODataUri] int key)
        {
            GroupCollection groupCollection = await db.Groups.FindAsync(key);
            if (groupCollection == null)
            {
                return NotFound();
            }

            db.Groups.Remove(groupCollection);
            await db.SaveChangesAsync();

            return StatusCode(HttpStatusCode.NoContent);
        }

        // GET: odata/Groups(5)/GroupType
        [EnableQuery]
        public SingleResult<LookupGroupType> GetGroupType([FromODataUri] int key)
        {
            return SingleResult.Create(db.Groups.Where(m => m.GroupUID == key).Select(m => m.GroupType));
        }

        // GET: odata/Groups(5)/Status
        [EnableQuery]
        public SingleResult<LookupGroupStatus> GetStatus([FromODataUri] int key)
        {
            return SingleResult.Create(db.Groups.Where(m => m.GroupUID == key).Select(m => m.Status));
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool GroupCollectionExists(int key)
        {
            return db.Groups.Count(e => e.GroupUID == key) > 0;
        }
    }
}
#endif