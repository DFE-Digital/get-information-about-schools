using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Edubase.Data.Entity;

namespace Edubase.Web.UI.Controllers.Api
{
    public class EstablishmentsController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: api/Establishments
        public IQueryable<Establishment> GetEstablishments()
        {
            return db.Establishments;
        }

        // GET: api/Establishments/5
        [ResponseType(typeof(Establishment))]
        public async Task<IHttpActionResult> GetEstablishment(int id)
        {
            Establishment establishment = await db.Establishments.FindAsync(id);
            if (establishment == null)
            {
                return NotFound();
            }

            return Ok(establishment);
        }

        // PUT: api/Establishments/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutEstablishment(int id, Establishment establishment)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != establishment.Urn)
            {
                return BadRequest();
            }

            db.Entry(establishment).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EstablishmentExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/Establishments
        [ResponseType(typeof(Establishment))]
        public async Task<IHttpActionResult> PostEstablishment(Establishment establishment)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Establishments.Add(establishment);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = establishment.Urn }, establishment);
        }

        // DELETE: api/Establishments/5
        [ResponseType(typeof(Establishment))]
        public async Task<IHttpActionResult> DeleteEstablishment(int id)
        {
            Establishment establishment = await db.Establishments.FindAsync(id);
            if (establishment == null)
            {
                return NotFound();
            }

            db.Establishments.Remove(establishment);
            await db.SaveChangesAsync();

            return Ok(establishment);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool EstablishmentExists(int id)
        {
            return db.Establishments.Count(e => e.Urn == id) > 0;
        }
    }
}