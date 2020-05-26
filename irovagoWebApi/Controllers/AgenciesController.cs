using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using irovagoWebApi.Models;

namespace irovagoWebApi.Controllers
{
    public class AgenciesController : ApiController
    {
        private DBModel db = new DBModel();

        public AgenciesController()
        {
            db.Configuration.ProxyCreationEnabled = false;
        }

        // GET: api/Agencies
        public IQueryable<Agency> GetAgencies()
        {
            return db.Agencies;
        }

        // GET: api/Agencies/5
        [ResponseType(typeof(Agency))]
        public IHttpActionResult GetAgency(int id)
        {
            Agency agency = db.Agencies.Find(id);
            if (agency == null)
            {
                return NotFound();
            }

            return Ok(agency);
        }

        // PUT: api/Agencies/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutAgency(int id, Agency agency)
        {

            if (id != agency.agencyID)
            {
                return BadRequest();
            }

            db.Entry(agency).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AgencyExists(id))
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

        // POST: api/Agencies
        [ResponseType(typeof(Agency))]
        public IHttpActionResult PostAgency(Agency agency)
        {

            db.Agencies.Add(agency);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = agency.agencyID }, agency);
        }

        // DELETE: api/Agencies/5
        [ResponseType(typeof(Agency))]
        public IHttpActionResult DeleteAgency(int id)
        {
            Agency agency = db.Agencies.Find(id);
            if (agency == null)
            {
                return NotFound();
            }

            db.Agencies.Remove(agency);
            db.SaveChanges();

            return Ok(agency);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool AgencyExists(int id)
        {
            return db.Agencies.Count(e => e.agencyID == id) > 0;
        }
    }
}