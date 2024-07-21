using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;
using Newtonsoft.Json;
using QuipuWorkItem.Models;

namespace QuipuWorkItem.Controllers
{
    public class ClientsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private static readonly HttpClient httpClient = new HttpClient();

        // GET: Clients/Index
        public ActionResult Index()
        {
            var clients = db.Clients.ToList(); // Fetch all clients from the database
            return View(clients); // Return the view with the "clients" model
        }

        // GET: Clients/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Clients/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,FirstName,LastName,Email,BirthDate")] Client client)
        {
            if (ModelState.IsValid)
            {
                db.Clients.Add(client);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(client);
        }

        // GET: Clients/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Client client = db.Clients.Find(id);
            if (client == null)
            {
                return HttpNotFound();
            }
            return View(client);
        }


        // GET: Clients/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Client client = db.Clients.Find(id);
            if (client == null)
            {
                return HttpNotFound();
            }
            return View(client);
        }

        // POST: Clients/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,FirstName,LastName,Email,BirthDate")] Client client)
        {
            if (ModelState.IsValid)
            {
                db.Entry(client).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(client);
        }

        // GET: Clients/Delete/5
        public ActionResult Delete(int id)
        {
            Client client = db.Clients.Find(id);
            if (client == null)
            {
                return HttpNotFound();
            }
            return View(client);
        }

        // POST: Clients/DeleteConfirmed
        [HttpPost, ActionName("DeleteConfirmed")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Client client = db.Clients.Find(id);
            db.Clients.Remove(client);
            db.SaveChanges();
            return RedirectToAction("Index");
        }




        // GET: Clients/UploadXml
        public ActionResult UploadXml()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UploadXml(HttpPostedFileBase file)
        {
            if (file != null && file.ContentLength > 0)
            {
                try
                {
                    var filePath = Path.Combine(Server.MapPath("~/App_Data/Uploads"), Path.GetFileName(file.FileName));
                    file.SaveAs(filePath);

                    var xDocument = XDocument.Load(filePath);

                    var clients = xDocument.Descendants("Client").Select(x => new Client
                    {
                        // Handle null cases and correct XML element names
                        FirstName = (string)x.Element("FirstName"),
                        LastName = (string)x.Element("LastName"),
                        Email = (string)x.Element("Email"),
                        BirthDate = (DateTime)(DateTime?)x.Element("DateOfBirth") // Use nullable DateTime to handle parsing errors
                    }).ToList();

                    foreach (var client in clients)
                    {
                        if (client.BirthDate == null) // Skip clients with invalid date
                        {
                            continue;
                        }
                        db.Clients.Add(client);
                    }

                    db.SaveChanges();

                    ViewBag.Message = "File uploaded and clients imported successfully.";
                }
                catch (Exception ex)
                {
                    ViewBag.Message = "ERROR: " + ex.Message;
                }
            }
            else
            {
                ViewBag.Message = "You have not specified a file.";
            }

            return View();
        }

        [HttpGet]
        public ActionResult ExportToJson()
        {
            try
            {
                var clients = db.Clients.ToList();
                var json = JsonConvert.SerializeObject(clients, Formatting.Indented);

                // Ensure the directory exists
                var exportDirectory = Server.MapPath("~/App_Data/Exports");
                if (!Directory.Exists(exportDirectory))
                {
                    Directory.CreateDirectory(exportDirectory);
                }

                var fileName = "ClientsExport.json";
                var filePath = Path.Combine(exportDirectory, fileName);

                System.IO.File.WriteAllText(filePath, json);

                // Return the file to the user
                return File(filePath, "application/json", fileName);
            }
            catch (Exception ex)
            {
                // Log the exception or handle it as needed
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.InternalServerError, $"Error exporting data: {ex.Message}");
            }
        }



        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
