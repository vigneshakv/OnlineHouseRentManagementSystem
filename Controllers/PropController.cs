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
using OnlineHouseRentManagementSystem.ViewModel;

namespace OnlineHouseRentManagementSystem.Controllers
{
    public class PropController : Controller
    {
        private HouseRentManagementEntities db = new HouseRentManagementEntities();

        // GET: Prop
        public ActionResult Index()
        {
            try
            {
                int id = (int)Session["seller_id"];
                var queryproperties = db.properties.Where(property => property.seller_id == id && property.property_status != "booked");
                return View(queryproperties);
            }
            catch (Exception)
            {
                return RedirectToAction("Index", "Home");
            }
            
        }
        public ActionResult CustomerIndex(string s = null)
        {
            string state = Request.Form["state"];
            if(state==null)
            {
                state = s;
            }
            string locality = Request.Form["locality"];
            ViewData.Clear();
            try
            {

                if (state != null && locality != null)
                {
                    state = state.ToLower();
                    locality = locality.ToLower();
                    var views = db.properties.Where(m => m.property_state.Contains(state) && (m.property_address.Contains(locality) || m.property_city.Contains(locality)));
                    views = views.Where(m => m.property_status != "booked");
                    var view  = views.ToList();
                    if (view.Count > 0)
                    {
                        return View(view);
                    }
                    ViewData["placefound"] = "No Match Found";
                    return View(db.properties.Where(m => m.property_status != "booked").ToList());
                }
                else if (locality != null && locality.Length > 0)
                {
                    locality = locality.ToLower();
                    var views = db.properties.Where(m => m.property_address.Contains(locality) || m.property_city.Contains(locality) || m.property_state.Contains(locality));
                    views = views.Where(m => m.property_status != "booked");
                    var view = views.ToList();
                    if (view.Count > 0)
                    {
                        return View(view);
                    }
                    ViewData["placefound"] = "No Match Found";
                    return View(db.properties.Where(m => m.property_status != "booked").ToList());
                }
                else if (state != null)
                {
                    state = state.ToLower();
                    var views = db.properties.Where(m => m.property_address.Contains(state) || m.property_state.Contains(state));
                    views = views.Where(m => m.property_status != "booked");
                    var view = views.ToList();
                    if (view.Count > 0)
                    {
                        return View(view);
                    }
                    ViewData["placefound"] = "No Match Found";
                    return View(db.properties.Where(m => m.property_status != "booked").ToList());
                }
                else
                {
                    ViewData["placefound"] = "No Match Found";
                    return View(db.properties.Where(m => m.property_status != "booked").ToList());
                }
            }
            catch(Exception)
            {
                return View(db.properties.Where(m => m.property_status != "booked").ToList());
            }
        }
        

        // GET: Prop/Details/5
        public ActionResult Details(int? id)
        {
            if (Session["user_role"] != null)
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                property property = db.properties.Find(id);
                if (property == null)
                {
                    return HttpNotFound();
                }
                return View(property);
            }
            return RedirectToAction("SignupLogin", "Home",new { useroption = "customer"});
        }

        // GET: Prop/Create
        public ActionResult Create()
        {
            if (Session["user_role"] != null)
            {
                ViewBag.seller_id = new SelectList(db.sellers, "seller_id", "seller_name");
                return View();
            }
            return RedirectToAction("Index", "Home");
        }

        // POST: Prop/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "property_id,property_type,property_name,property_address,property_state,property_city,property_pin,bhk,floor,total_floor,bathroom,bedroom,property_age,facing,property_size,lease,expected_rent,expected_deposit,preferred_tenants,furnishing,parking,description,property_status,post_date,balcony,gated_security,water_supply,available_date")] property property,
            HttpPostedFileBase[] property_image)
        {
            try
            {             
                int id = (int)Session["seller_id"];
                property.seller_id = id;
                property.property_type = property.property_type.ToLower();
                property.property_name = property.property_name.ToLower();
                property.property_address = property.property_address.ToLower();
                property.property_state = property.property_state.ToLower();
                property.property_city = property.property_city.ToLower();
                property.facing = property.facing.ToLower();
                property.lease = property.lease.ToLower();
                property.preferred_tenants = property.preferred_tenants.ToLower();
                property.furnishing = property.furnishing.ToLower();
                property.parking = property.parking.ToLower();
                var datetime = DateTime.Now.ToString("dd/MM/yyyy");
                property.post_date = DateTime.Parse(datetime);
                property.property_status = "available";
                property.gated_security = property.gated_security.ToLower();
                property.water_supply = property.water_supply.ToLower();

                db.properties.Add(property);
                db.SaveChanges();
                var lastproperty_id = db.properties.OrderByDescending(pid => pid.property_id).FirstOrDefault(pid => pid.seller_id == id);
                int lastpid = lastproperty_id.property_id;
                if (property_image.GetValue(0)!=null)
                {
                    foreach (var item in property_image)
                    {
                        BinaryReader binaryReader = new BinaryReader(item.InputStream);
                        property_images property_Images = new property_images
                        {
                            property_id = lastpid,
                            property_images1 = binaryReader.ReadBytes((int)item.ContentLength)
                        };
                        db.property_images.Add(property_Images);
                    }
                }
                db.SaveChanges();
                return RedirectToAction("Index");
                
            }
            catch(Exception e)
            {
                return Content(e.Message);
                return RedirectToAction("Index", "Home");
            }
        }

        // GET: Prop/Edit/5
        public ActionResult Edit(int? id)
        {
            try
            {
                int sid = (int)Session["seller_id"];
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                property property = db.properties.Find(id);
                if (property == null)
                {
                    return HttpNotFound();
                }
                ViewBag.seller_id = new SelectList(db.sellers, "seller_id", "seller_name", property.seller_id);
                return View(property);
            }
            catch(Exception e)
            {
                return Content(e.Message);
                return RedirectToAction("Index", "Home");
            }
        }

        // POST: Prop/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Exclude ="seller_id",Include = "property_id,property_type,property_name,property_address,property_state,property_city,property_pin,bhk,floor,total_floor,bathroom,bedroom,property_age,facing,property_size,lease,expected_rent,expected_deposit,preferred_tenants,furnishing,parking,description,property_status,balcony,water_supply,gated_security,available_date")] property property,
            HttpPostedFileBase[] property_image)
        {
            try {
            if (ModelState.IsValid)
            {
                int id = (int)Session["seller_id"];
                property.seller_id = id;
                property.property_type = property.property_type.ToLower();
                property.property_name = property.property_name.ToLower();
                property.property_address = property.property_address.ToLower();
                property.property_state = property.property_state.ToLower();
                property.property_city = property.property_city.ToLower();
                property.facing = property.facing.ToLower();
                property.lease = property.lease.ToLower();
                property.preferred_tenants = property.preferred_tenants.ToLower();
                property.furnishing = property.furnishing.ToLower();
                property.parking = property.parking.ToLower();
                var datetime = DateTime.Today.ToString("G");
                property.post_date = DateTime.Parse(datetime);
                property.property_status = "available";
                property.gated_security = property.gated_security.ToLower();
                property.water_supply = property.water_supply.ToLower();

                db.Entry(property).State = EntityState.Modified;
                db.SaveChanges();
                if (property_image.Length > 0)
                {
                    foreach (var image in property_image)
                    {
                        if (image != null && image.ContentLength > 0)
                        {
                            BinaryReader binaryReader = new BinaryReader(image.InputStream);
                            property_images property_Images = new property_images
                            {
                                property_id = property.property_id,
                                property_images1 = binaryReader.ReadBytes(image.ContentLength)
                            };
                            db.Entry(property_Images).State = EntityState.Added;
                        }
                    }
                }
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            }
            catch(Exception e)
            {
                return Content(e.Message);
                return RedirectToAction("Index", "Home");
            }
            ViewBag.seller_id = new SelectList(db.sellers, "seller_id", "seller_name", property.seller_id);
            return View(property);
        }

        // GET: Prop/Delete/5
        public ActionResult Delete(int? id)
        {
            try
            {
                int sid = (int)Session["seller_id"];
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                property property = db.properties.Find(id);
                if (property == null)
                {
                    return HttpNotFound();
                }
                return View(property);
            }
            catch(Exception)
            {
                return RedirectToAction("Index", "Home");
            }
        }

        // POST: Prop/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            if (Session["user_role"] != null)
            {
                property property = db.properties.Find(id);
                db.properties.Remove(property);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return RedirectToAction("Index", "Home");
        }

        public ActionResult DeleteImage(int id,int propertyid)
        {
            try
            {
                int sid = (int)Session["seller_id"];
                property_images property_Images = db.property_images.Find(id);
                db.property_images.Remove(property_Images);
                db.SaveChanges();
                return RedirectToAction("Edit", "Prop", new { id = propertyid });
            }
            catch(Exception)
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public ActionResult TopSeller()
        {
            var sellergroup = from p in db.booked_properties
                              group p by p.seller_id;
            var topseller = sellergroup.OrderByDescending(s=>s.Count()).Take(5);
            var topsellerid = topseller.Select(s => s.Key);
            List<TopSellers> sellerss = new List<TopSellers>();
            foreach (var item in topsellerid)
            {
                var seller = db.sellers.Find(item);
                var sellerinfo = new TopSellers()
                {
                    Sellers = seller,
                    TotalBooking = db.booked_properties.Count(s => s.seller_id == seller.seller_id)
                };
                sellerss.Add(sellerinfo);
            }
            return View(sellerss);
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
