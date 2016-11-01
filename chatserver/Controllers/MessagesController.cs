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
using Microsoft.AspNet.SignalR;
using System.IO;
using System.Collections.Concurrent;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace chatserver.Controllers
{
    [RoutePrefix("api/messages")]
    public class MessagesController : ApiController
    {
        private dbEntities db = new dbEntities();
        private StatisticsModel statistics = new StatisticsModel();
        private ChatHub chatHub = new ChatHub();
        IHubContext hubContext = GlobalHost.ConnectionManager.GetHubContext<ChatHub>();

        //private static readonly ConcurrentQueue<StreamWriter> _streammessage = new ConcurrentQueue<StreamWriter>();

        ////public static void OnStreamAvailable(Stream stream, HttpContentHeaders headers, TransportContext context, Task task)
        ////{
        ////    StreamWriter streamwriter = new StreamWriter(stream);
        ////    _streammessage.Enqueue(streamwriter);
        ////}

        //private static void MessageCallback(TweetModel t)
        //{
        //    foreach (var subscriber in _streammessage)
        //    {
        //        subscriber.WriteLine("data:" + JsonConvert.SerializeObject(t) + "n");
        //        subscriber.Flush();
        //    }
        //}

        //public HttpResponseMessage Get(HttpRequestMessage request)
        //{
        //    HttpResponseMessage response = request.CreateResponse();
        //    response.Content = new PushStreamContent(OnStreamAvailable, "text/event-stream");
            
        //    return response;
        //}

        //private Task OnStreamAvailable(Stream arg1, HttpContent arg2, TransportContext arg3)
        //{
        //    StreamWriter streamwriter = new StreamWriter(arg1);
        //    _streammessage.Enqueue(streamwriter);
        //    return null;
        //}


        //public void Post(TweetModel t)
        //{
        //    MessageCallback(t);
        //}

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

        [HttpGet]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [Route("statistics/scalars")]
        public HttpResponseMessage averageWordsPerMessage(String username="")
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }

            try
            {
                Dictionary<String, double> scalarDictionary = new Dictionary<String, double>();

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
            try
            {
                db.MESSAGES.Add(SlimMessageModel.toMessage(slimMessage));
                db.SaveChanges();
                //hubContext.Clients.All.broadcast("Fetch", "stop the chat");

            }
            catch (DbUpdateConcurrencyException ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, ex);
            }

            catch (Exception e)
            {
                //after saving success, bcast to all REFRESH
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e);
            }

            
            return Request.CreateResponse(HttpStatusCode.OK);
        }

       
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}