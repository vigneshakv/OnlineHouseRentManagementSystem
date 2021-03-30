using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Remoting;
using System.Web;
using System.Web.Mvc;
using OnlineHouseRentManagementSystem.Models;
using OnlineHouseRentManagementSystem.ViewModel;

namespace OnlineHouseRentManagementSystem.Controllers
{
    public class SellersController : Controller
    {
        private HouseRentManagementEntities db = new HouseRentManagementEntities();

        // GET: Sellers
        public ActionResult Index()
        {
            
            if((string) Session["user_role"]=="seller")
            {
                int id = (int)Session["seller_id"];
                var sellermodel = new SellerViewModel()
                {
                    customers_Statuses = db.customers_status.Where(s => s.seller_id == id).ToList(),
                    booked_Properties = db.booked_properties.Where(s => s.seller_id == id).OrderByDescending(o => o.si_no).ToList(),
                    TotalBooking = db.booked_properties.Count(s => s.seller_id == id),
                    TotalRequests = db.booking_details.Count(s => s.property.seller_id == id && s.booking_status == "requested")
                };
                var a = sellermodel.customers_Statuses.FirstOrDefault(s => s.seller_id == id);
                return View("~/Views/Sellers/Home.cshtml", sellermodel);
            }
            return RedirectToAction("Index", "Home");
            

            //Remove latter
            //return View("~/Views/Sellers/Home.cshtml");
        }
        public ActionResult MessageDelete(int? si_no)
        {
            var customer_Status = db.customers_status.Find(si_no);
            db.customers_status.Remove(customer_Status);
            db.SaveChanges();
            return RedirectToAction("Index", "Sellers");
        }

        // GET: Sellers/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            seller seller = db.sellers.Find(id);
            if (seller == null)
            {
                return HttpNotFound();
            }
            return View(seller);
        }

        // GET: Sellers/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Sellers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "seller_id,seller_name,seller_email,seller_phone,seller_photo,seller_state")] seller seller)
        {
            if (ModelState.IsValid)
            {
                Session["name"] = seller.seller_name.ToLower();
                Session["phone"] = seller.seller_phone;
                Session["state"] = seller.seller_state.ToLower();
                Session["email"] = seller.seller_email;
                Session["user_role"] = "seller";
                int con = db.users.Count(s => s.user_email == seller.seller_email);
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

        // GET: Sellers/Edit/5
        public ActionResult Edit(int? id,string saved = null)
        {
            if (Session["user_role"] != null)
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                seller seller = db.sellers.Find(id);
                if (seller == null)
                {
                    return HttpNotFound();
                }
                ViewData["saved"] = saved;
                return View(seller);
            }
            return RedirectToAction("Index", "Home");
        }

        // POST: Sellers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Exclude="seller_photo",Include = "seller_id,seller_name,seller_email,seller_phone,seller_state")] seller seller, HttpPostedFileBase seller_photo)
        {
            if (ModelState.IsValid)
            {
                ViewData.Clear();
                if (seller_photo != null && seller_photo.ContentLength > 0)
                {
                    BinaryReader binaryReader = new BinaryReader(seller_photo.InputStream);
                    seller.seller_photo = binaryReader.ReadBytes((int)seller_photo.ContentLength);
                }
                else
                {
                    var ts = db.sellers.AsNoTracking().FirstOrDefault(s => s.seller_id == seller.seller_id);
                    seller.seller_photo = ts.seller_photo;
                }
                seller.seller_name = seller.seller_name.ToLower();
                seller.seller_state = seller.seller_state.ToLower();
                db.Entry(seller).State = EntityState.Modified;
                db.SaveChanges();
                ViewData["saved"] = "yes";
                return RedirectToAction("Edit", "Sellers", new { id = seller.seller_id, saved = "true"});
            }
            return View(seller);
        }

        // GET: Sellers/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            seller seller = db.sellers.Find(id);
            if (seller == null)
            {
                return HttpNotFound();
            }
            return View(seller);
        }

        // POST: Sellers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            seller seller = db.sellers.Find(id);
            db.sellers.Remove(seller);
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
