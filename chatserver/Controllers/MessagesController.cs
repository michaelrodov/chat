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
using System.Web.Http.Cors;

namespace chatserver.Controllers
{
    [RoutePrefix("api/messages")]
    public class MessagesController : ApiController
    {
        private dbEntities db = new dbEntities();

        /***
         * Fetch the messages for a certain user starting from certain id
         */ 
        [HttpGet]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [Route("get/afterid/{afterid}/user/{user}")]
        public IEnumerable<MESSAGE> GetMESSAGEs(long afterid, String user)
        {
            //return db.MESSAGES.AsEnumerable();
            try
            {
                return db.MESSAGES
                    .Where(o => (o.ID > afterid))
                    //.Where(o => (String.Equals(o.TO, user) || String.Equals(o.FROM, user)))
                    .AsEnumerable();
            }
            catch (Exception e)
            {
                throw new Exception("Error while trying to fetch messages for: " + user + " starting id: " + afterid);
            }

        }

        // GET api/Messages/5
        //public MESSAGE GetMESSAGE(int id)
        //{
        //    MESSAGE message = db.MESSAGES.Find(id);
        //    if (message == null)
        //    {
        //        throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
        //    }

        //    return message;
        //}

        [HttpPut]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [Route("add")]
        public HttpResponseMessage PutMESSAGE(SlimMessageModel slimMessage)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }
            //db.MESSAGES.Create(SlimMessageModel.toMessage(slimMessage));
            //db.Entry(SlimMessageModel.toMessage(slimMessage)).State = EntityState.Modified;
            db.MESSAGES.Add(SlimMessageModel.toMessage(slimMessage));
            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, ex);
            }
            catch (Exception e)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e);
            }

            return Request.CreateResponse(HttpStatusCode.OK);
        }

        // POST api/Messages
        //public HttpResponseMessage PostMESSAGE(MESSAGE message)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.MESSAGES.Add(message);
        //        db.SaveChanges();

        //        HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Created, message);
        //        response.Headers.Location = new Uri(Url.Link("DefaultApi", new { id = message.ID }));
        //        return response;
        //    }
        //    else
        //    {
        //        return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
        //    }
        //}

        // DELETE api/Messages/5
        //public HttpResponseMessage DeleteMESSAGE(int id)
        //{
        //    MESSAGE message = db.MESSAGES.Find(id);
        //    if (message == null)
        //    {
        //        return Request.CreateResponse(HttpStatusCode.NotFound);
        //    }

        //    db.MESSAGES.Remove(message);

        //    try
        //    {
        //        db.SaveChanges();
        //    }
        //    catch (DbUpdateConcurrencyException ex)
        //    {
        //        return Request.CreateErrorResponse(HttpStatusCode.NotFound, ex);
        //    }

        //    return Request.CreateResponse(HttpStatusCode.OK, message);
        //}

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}