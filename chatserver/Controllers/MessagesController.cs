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
        private StatisticsModel statistics = new StatisticsModel();

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



        //[HttpGet]
        //[EnableCors(origins: "*", headers: "*", methods: "*")]
        //[Route("statistics/timebetweenmsg")]
        //public HttpResponseMessage averageTimeBetweenMessages()
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
        //    }
        //    try
        //    {

        //    }    
        //    catch (Exception e)
        //    {
        //        return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e);
        //    }

        //    return Request.CreateResponse(HttpStatusCode.OK);
        //}


        [HttpGet]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [Route("statistics/scalars")]
        public HttpResponseMessage averageWordsPerMessage(String username = "")
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }

            try
            {
                Dictionary<String, long> scalarDictionary = new Dictionary<string, long>();

                scalarDictionary.Add("avgLettersAllUsers", statistics.getAvgLetters(""));
                scalarDictionary.Add("avgLettersPerUser", statistics.getAvgLetters(username));
                scalarDictionary.Add("timeBetweenMsg", 0);

                return Request.CreateResponse(HttpStatusCode.OK, scalarDictionary);
            }
            catch (Exception e)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e);
            }

        }

        /**/
        [HttpGet]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [Route("statistics/vectors")]
        public HttpResponseMessage averageWordsPerMessage(long hours = 168)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }

            try
            {
                Dictionary<String, long[]> vectorDictionary = new Dictionary<String, long[]>();
                List<NumericChartOutput> wph = statistics.getWordsPerHour(hours);
                List<NumericChartOutput> mph = statistics.getMessagesPerHour(hours);

                long[] wordsPerHourLastWeek = new long[hours];
                long[] msgPerHourLastWeek = new long[hours];

                wph.ForEach(r =>
                {
                    if (Math.Abs(r.K) < msgPerHourLastWeek.Length)
                    {
                        wordsPerHourLastWeek[Math.Abs(r.K)] = r.V;
                    }
                });

                mph.ForEach(r =>
                {
                    if (Math.Abs(r.K) < msgPerHourLastWeek.Length)
                    {
                        msgPerHourLastWeek[Math.Abs(r.K)] = r.V;
                    }
                });
                
                vectorDictionary.Add("wordsPerHourLastWeek", wordsPerHourLastWeek);
                vectorDictionary.Add("msgPerHourLastWeek", msgPerHourLastWeek);

                return Request.CreateResponse(HttpStatusCode.OK, vectorDictionary);
            }
            catch (Exception e)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e);
            }

        }

        /**/
        //[HttpGet]
        //[EnableCors(origins: "*", headers: "*", methods: "*")]
        //[Route("statistics/messages/perhour")]
        //public HttpResponseMessage averageMessagesPerHour(long hours)
        //{

        //    if (!ModelState.IsValid)
        //    {
        //        return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
        //    }

        //    try
        //    {
        //        return Request.CreateResponse(HttpStatusCode.OK, statistics.getMessagesPerHour(1));
        //    }
        //    catch (Exception e)
        //    {
        //        return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e);
        //    }

        //}


        /**
         * Add a chat message to the chat list
         */
        [HttpPut]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [Route("add")]
        public HttpResponseMessage PutMESSAGE(SlimMessageModel slimMessage)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }
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