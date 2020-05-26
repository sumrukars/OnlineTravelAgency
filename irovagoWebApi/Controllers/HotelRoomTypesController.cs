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
    public class HotelRoomTypesController : ApiController
    {
        private DBModel db = new DBModel();

        public HotelRoomTypesController()
        {
            db.Configuration.ProxyCreationEnabled = false;
        }

        // GET: api/HotelRoomTypes
        public IQueryable<HotelRoomType> GetHotelRoomTypes()
        {
            return db.HotelRoomTypes;
        }

        // GET: api/HotelRoomTypes/5
        [ResponseType(typeof(HotelRoomType))]
        public IHttpActionResult GetHotelRoomType(int id)
        {
            HotelRoomType hotelRoomType = db.HotelRoomTypes.Find(id);
            if (hotelRoomType == null)
            {
                return NotFound();
            }

            return Ok(hotelRoomType);
        }

        // PUT: api/HotelRoomTypes/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutHotelRoomType(int id, HotelRoomType hotelRoomType)
        {

            if (id != hotelRoomType.hotelRoomTypeID)
            {
                return BadRequest();
            }

            db.Entry(hotelRoomType).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!HotelRoomTypeExists(id))
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

        // POST: api/HotelRoomTypes
        [ResponseType(typeof(HotelRoomType))]
        public IHttpActionResult PostHotelRoomType(HotelRoomType hotelRoomType)
        {

            db.HotelRoomTypes.Add(hotelRoomType);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = hotelRoomType.hotelRoomTypeID }, hotelRoomType);
        }

        // DELETE: api/HotelRoomTypes/5
        [ResponseType(typeof(HotelRoomType))]
        public IHttpActionResult DeleteHotelRoomType(int id)
        {
            HotelRoomType hotelRoomType = db.HotelRoomTypes.Find(id);
            if (hotelRoomType == null)
            {
                return NotFound();
            }

            db.HotelRoomTypes.Remove(hotelRoomType);
            db.SaveChanges();

            return Ok(hotelRoomType);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool HotelRoomTypeExists(int id)
        {
            return db.HotelRoomTypes.Count(e => e.hotelRoomTypeID == id) > 0;
        }
    }
}