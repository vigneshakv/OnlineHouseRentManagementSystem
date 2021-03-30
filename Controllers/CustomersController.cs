using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using OnlineHouseRentManagementSystem.Models;

namespace OnlineHouseRentManagementSystem.Controllers
{
    public class CustomersController : Controller
    {
        private HouseRentManagementEntities db = new HouseRentManagementEntities();

        // GET: Customers
        public ActionResult Index()
        {
            return View(db.customers.ToList());
        }

        // GET: Customers/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            customer customer = db.customers.Find(id);
            if (customer == null)
            {
                return HttpNotFound();
            }
            return View(customer);
        }

        // GET: Customers/Create
        public ActionResult Create()
        {
            return View("/Views/Home/SignupLogin.cshtml");
        }

        // POST: Customers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "customer_id,customer_name,customer_email,customer_phone,customer_photo,customer_state")] customer customer)
        {
            
            if (ModelState.IsValid)
            {
                Session["name"] = customer.customer_name.ToLower();
                Session["phone"] = customer.customer_phone;
                Session["state"] = customer.customer_state.ToLower();
                Session["email"] = customer.customer_email;
                Session["user_role"] = "customer";
                int con = db.users.Count(s => s.user_email == customer.customer_email);
                if (con == 0)
                {
                    return RedirectToAction("Create", "Users");
                }
                else
                {
                    Session["message"] = "User email already exist..!";
                    return RedirectToAction("SignupLogin", "Home");
                }
            }
            return View("/Views/Home/SignupLogin.cshtml");
        }

        // GET: Customers/Edit/5
        public ActionResult Edit(int? id, string saved = null)
        {
            if (Session["user_role"] != null)
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                customer customer = db.customers.Find(id);
                if (customer == null)
                {
                    return HttpNotFound();
                }
                ViewData["saved"] = saved;
                return View(customer);
            }
            return RedirectToAction("Index", "Home");
        }

        // POST: Customers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "customer_id,customer_name,customer_email,customer_phone,customer_state")] customer customer, HttpPostedFileBase customer_photo)
        {
            if (ModelState.IsValid)
            {
                ViewData.Clear();
                if (customer_photo != null && customer_photo.ContentLength > 0)
                {
                    BinaryReader binaryReader = new BinaryReader(customer_photo.InputStream);
                    customer.customer_photo = binaryReader.ReadBytes((int)customer_photo.ContentLength);
                }
                else
                {
                    var ts = db.customers.AsNoTracking().FirstOrDefault(s => s.customer_id == customer.customer_id);
                    customer.customer_photo = ts.customer_photo;
                }
                customer.customer_name = customer.customer_name.ToLower();
                customer.customer_state = customer.customer_state.ToLower();
                db.Entry(customer).State = EntityState.Modified;
                db.SaveChanges();
                ViewData["saved"] = "yes";
                return RedirectToAction("Edit", "Customers", new { id = customer.customer_id, saved = "true" });
            }
            return View(customer);
        }

        // GET: Customers/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            customer customer = db.customers.Find(id);
            if (customer == null)
            {
                return HttpNotFound();
            }
            return View(customer);
        }

        // POST: Customers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            customer customer = db.customers.Find(id);
            db.customers.Remove(customer);
            db.SaveChanges();
            return RedirectToAction("Index");
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
