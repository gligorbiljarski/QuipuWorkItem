﻿using System;
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

        // GET: Clients/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Clients/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,FirstName,LastName,Email,BirthDate,Address")] Client client)
        {
            if (ModelState.IsValid)
            {
                // Ensure Address has a default value if it's not provided
                if (string.IsNullOrEmpty(client.Address))
                {
                    client.Address = "Default Address"; // Provide a default address or handle accordingly
                }

                db.Clients.Add(client);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(client);
        }

        // GET: Clients/Edit/5
        [HttpGet]
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

        // POST: Clients/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,FirstName,LastName,Email,BirthDate,Address")] Client client)
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
            if (client == null)
            {
                return HttpNotFound();
            }

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
                        FirstName = (string)x.Element("Name"),
                        LastName = "N/A", // Assuming LastName is not provided in the XML
                        Email = "N/A", // Assuming Email is not provided in the XML
                        BirthDate = (DateTime?)x.Element("BirthDate") ?? DateTime.MinValue, // Handle nullable DateTime
                        Address = string.Join(", ", x.Descendants("Address").Select(a => (string)a).ToList())
                    }).ToList();

                    foreach (var client in clients)
                    {
                        if (client.BirthDate != DateTime.MinValue) // Skip clients with invalid date
                        {
                            db.Clients.Add(client);
                        }
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
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, $"Error exporting data: {ex.Message}");
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
