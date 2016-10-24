using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using chatserver.Models;

namespace chatserver.Controllers
{
    public class MessageController : ApiController
    {
        private chatserverContext db = new chatserverContext();

        // GET api/Message
        public IEnumerable<MESSAGE> GetMESSAGEs()
        {
            return db.MESSAGEs.AsEnumerable();
        }

        // GET api/Message/5
        public MESSAGE GetMESSAGE(int id)
        {
            MESSAGE message = db.MESSAGEs.Find(id);
            if (message == null)
            {
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            }

            return message;
        }

        // PUT api/Message/5
        public HttpResponseMessage PutMESSAGE(int id, MESSAGE message)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }

            if (id != message.ID)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }

            db.Entry(message).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, ex);
            }

            return Request.CreateResponse(HttpStatusCode.OK);
        }

        // POST api/Message
        public HttpResponseMessage PostMESSAGE(MESSAGE message)
        {
            if (ModelState.IsValid)
            {
                db.MESSAGEs.Add(message);
                db.SaveChanges();

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Created, message);
                response.Headers.Location = new Uri(Url.Link("DefaultApi", new { id = message.ID }));
                return response;
            }
            else
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }
        }

        // DELETE api/Message/5
        public HttpResponseMessage DeleteMESSAGE(int id)
        {
            MESSAGE message = db.MESSAGEs.Find(id);
            if (message == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }

            db.MESSAGEs.Remove(message);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, ex);
            }

            return Request.CreateResponse(HttpStatusCode.OK, message);
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}