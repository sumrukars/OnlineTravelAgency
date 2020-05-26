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
    public class FavoriteOffersController : ApiController
    {
        private DBModel db = new DBModel();

        public FavoriteOffersController()
        {
            db.Configuration.ProxyCreationEnabled = false;
        }
        // GET: api/FavoriteOffers
        public IQueryable<FavoriteOffer> GetFavoriteOffers()
        {
            return db.FavoriteOffers;
        }

        // GET: api/FavoriteOffers/5
        [ResponseType(typeof(FavoriteOffer))]
        public IHttpActionResult GetFavoriteOffer(int id)
        {
            FavoriteOffer favoriteOffer = db.FavoriteOffers.Find(id);
            if (favoriteOffer == null)
            {
                return NotFound();
            }

            return Ok(favoriteOffer);
        }

        // PUT: api/FavoriteOffers/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutFavoriteOffer(int id, FavoriteOffer favoriteOffer)
        {
           
            if (id != favoriteOffer.favoriteOfferID)
            {
                return BadRequest();
            }

            db.Entry(favoriteOffer).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FavoriteOfferExists(id))
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

        // POST: api/FavoriteOffers
        [ResponseType(typeof(FavoriteOffer))]
        public IHttpActionResult PostFavoriteOffer(FavoriteOffer favoriteOffer)
        {
           

            db.FavoriteOffers.Add(favoriteOffer);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = favoriteOffer.favoriteOfferID }, favoriteOffer);
        }

        // DELETE: api/FavoriteOffers/5
        [ResponseType(typeof(FavoriteOffer))]
        public IHttpActionResult DeleteFavoriteOffer(int id)
        {
            FavoriteOffer favoriteOffer = db.FavoriteOffers.Find(id);
            if (favoriteOffer == null)
            {
                return NotFound();
            }

            db.FavoriteOffers.Remove(favoriteOffer);
            db.SaveChanges();

            return Ok(favoriteOffer);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool FavoriteOfferExists(int id)
        {
            return db.FavoriteOffers.Count(e => e.favoriteOfferID == id) > 0;
        }
    }
}