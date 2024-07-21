using System.Net;
using System.Net.Http;
using System.Web.Http;
using QuipuWorkItem.Models;

namespace QuipuWorkItem.Controllers
{
    public class ClientApiController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET api/clientapi/5
        [HttpGet]
        public HttpResponseMessage GetClient(int id)
        {
            var client = db.Clients.Find(id);
            if (client == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }
            return Request.CreateResponse(HttpStatusCode.OK, client);
        }

        // PUT api/clientapi/5
        [HttpPut]
        public HttpResponseMessage PutClient(int id, [FromBody] Client client)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }

            var existingClient = db.Clients.Find(id);
            if (existingClient == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }

            // Update properties with actual names from your model
            existingClient.FirstName = client.FirstName;
            existingClient.LastName = client.LastName;
            existingClient.Email = client.Email;
            existingClient.BirthDate = client.BirthDate;
            // Removed Addresses assignment

            db.SaveChanges();

            return Request.CreateResponse(HttpStatusCode.OK);
        }
    }
}
