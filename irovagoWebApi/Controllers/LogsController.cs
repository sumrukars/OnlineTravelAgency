﻿using System;
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
    public class LogsController : ApiController
    {
        private DBModel db = new DBModel();

        public LogsController()
        {
            db.Configuration.ProxyCreationEnabled = false;
        }

        // GET: api/Logs
        public IQueryable<Log> GetLogs()
        {
            return db.Logs;
        }

        // GET: api/Logs/5
        [ResponseType(typeof(Log))]
        public IHttpActionResult GetLog(int id)
        {
            Log log = db.Logs.Find(id);
            if (log == null)
            {
                return NotFound();
            }

            return Ok(log);
        }

        // PUT: api/Logs/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutLog(int id, Log log)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != log.logID)
            {
                return BadRequest();
            }

            db.Entry(log).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!LogExists(id))
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

        // POST: api/Logs
        [ResponseType(typeof(Log))]
        public IHttpActionResult PostLog(Log log)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Logs.Add(log);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = log.logID }, log);
        }

        // DELETE: api/Logs/5
        [ResponseType(typeof(Log))]
        public IHttpActionResult DeleteLog(int id)
        {
            Log log = db.Logs.Find(id);
            if (log == null)
            {
                return NotFound();
            }

            db.Logs.Remove(log);
            db.SaveChanges();

            return Ok(log);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool LogExists(int id)
        {
            return db.Logs.Count(e => e.logID == id) > 0;
        }
    }
}