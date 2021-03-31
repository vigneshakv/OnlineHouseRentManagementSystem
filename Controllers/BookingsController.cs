using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using OnlineHouseRentManagementSystem.EmailSettings;
using OnlineHouseRentManagementSystem.Models;

namespace OnlineHouseRentManagementSystem.Controllers
{
    public class BookingsController : Controller
    {
        private readonly HouseRentManagementEntities db = new HouseRentManagementEntities();

        // GET: bookings
        public ActionResult Index(int property_id = 0)
        {
            try
            {
                if ((string)Session["user_role"] == "customer")
                {
                    int id = (int)Session["customer_id"];
                    var booking_details = db.booking_details.Where(b => b.customer_id == id);
                    return View(booking_details.ToList());
                }
                else
                {
                    int id = (int)Session["seller_id"];
                    var booking_details = db.booking_details.Where(b => b.property.property_id == property_id && b.booking_status!="rejected");
                    ViewData["property_id"] = property_id;
                    return View("SellerIndex", booking_details.ToList());
                }
            }
            catch(Exception)
            {
                return RedirectToAction("SignupLogin", "Home");
            }
        }

        // GET: bookings/Details/5
        public ActionResult Details(int? id)
        {
            try
            {
                if (Session["user_role"] != null)
                {
                    if (id == null)
                    {
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                    }
                    booking_details booking_details = db.booking_details.Find(id);
                    if ((string)Session["user_role"] == "customer")
                    {
                        if (booking_details != null)
                        {
                            // property props = db.properties.Find(booking_details.property_id);
                            return View("BookedDetails", booking_details);
                        }
                    }
                    if (booking_details == null)
                    {
                        return HttpNotFound();
                    }
                    return View(booking_details);
                }
                return RedirectToAction("Index", "Home");
            }
            catch(Exception)
            {
                return RedirectToAction("Index", "Home");
            }
        }

        // GET: bookings/Create
        public ActionResult Create()
        {
            try
            {

                if (Session["user_role"] != null)
                {
                    ViewBag.customer_id = new SelectList(db.customers, "customer_id", "customer_name");
                    ViewBag.property_id = new SelectList(db.properties, "property_id", "property_type");
                    return View();
                }
                return RedirectToAction("Index", "Home");
            }
            catch
            {
                return RedirectToAction("Index", "Home");
            }
        }

        // POST: bookings/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "property_id")] booking_details booking_details)
        {
                if (ModelState.IsValid)
                {
                    int id = (int)Session["customer_id"];
                    booking_details.customer_id = id;
                    booking_details.booking_status = "requested";
                    var cbd = db.booking_details.Count(cus => cus.property_id == booking_details.property_id && cus.customer_id == booking_details.customer_id && (cus.booking_status != null));
                    if (cbd == 0)
                    {
                        var datetime = DateTime.Today.ToString("G");
                        booking_details.book_date = DateTime.Parse(datetime);
                        db.booking_details.Add(booking_details);
                        db.SaveChanges();
                        var customer_set = db.customers.FirstOrDefault(item => item.customer_id == booking_details.customer_id);
                        var customer_name = customer_set.customer_name;
                        var prop_set = db.properties.FirstOrDefault(item => item.property_id == booking_details.property_id);
                        var prop_name = prop_set.property_name;
                        customers_status customers_Status = new customers_status()
                        {
                            customer_id = booking_details.customer_id,
                            message = customer_name + " requested for the property " + prop_name,
                            seller_id = prop_set.seller_id,
                            property_id=prop_set.property_id
                        };
                        db.customers_status.Add(customers_Status);
                        db.SaveChanges();
                        TempData["request"] = "true";
                    }
                    else
                    {
                        TempData["request"] = "false";
                    }
                    return RedirectToAction("Index");
                }

                ViewBag.customer_id = new SelectList(db.customers, "customer_id", "customer_name", booking_details.customer_id);
                ViewBag.property_id = new SelectList(db.properties, "property_id", "property_type", booking_details.property_id);
                return View(booking_details);
            /*}
            catch(Exception)
            {
                return RedirectToAction("Index", "Home");
            }
            */
        }

        // GET: bookings/Edit/5
        public ActionResult Edit(int? id)
        {
            try
            {
                if (Session["user_role"] != null)
                {
                    if (id == null)
                    {
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                    }
                    booking_details booking_details = db.booking_details.Find(id);
                    if (booking_details == null)
                    {
                        return HttpNotFound();
                    }
                    ViewBag.customer_id = new SelectList(db.customers, "customer_id", "customer_name", booking_details.customer_id);
                    ViewBag.property_id = new SelectList(db.properties, "property_id", "property_type", booking_details.property_id);
                    return View(booking_details);
                }
                return RedirectToAction("Index", "Home");
            }
            catch(Exception)
            {
                return RedirectToAction("Index", "Home");
            }
        }
        // GET: bookings/AD/5
        public ActionResult AD(int si_no = 0,int op = 0)
        {
            
                if (Session["seller_id"] != null)
                {
                    booking_details booking_Details = db.booking_details.Find(si_no);
                    int flag = 0;
                   // var actions = "";
                    if (op == 1 && booking_Details.booking_status != "accepted")
                    {
                        booking_Details.booking_status = "accepted";
                        flag = 1;
                       // actions = "accepted";
                        
                    }
                    else if(op == 0 && booking_Details.booking_status != "rejected")
                    {
                        booking_Details.booking_status = "rejected";
                        flag = 1;
                        //actions = "rejected";
                    }
                    if (flag == 1)
                    {
                        db.Entry(booking_Details).State = EntityState.Modified;
                    /*
                        var prop_name = db.properties.FirstOrDefault(s => s.property_id == booking_Details.property_id);    
                        sellers_status sellers_Status = db.sellers_status.FirstOrDefault(s => s.customer_id == booking_Details.customer_id && s.property_id == booking_Details.property_id);
                        if(sellers_Status!=null)
                        {
                            sellers_Status.message = "Seller " + actions + " request for the property " + prop_name.property_name;
                            db.Entry(sellers_Status).State = EntityState.Modified;    
                        }
                        else
                        {
                            sellers_status sellers_Status1 = new sellers_status()
                            {
                                customer_id = booking_Details.customer_id,
                                message = "Seller " + actions + " request for the property " + prop_name.property_name,
                                property_id = booking_Details.property_id
                            };
                            db.sellers_status.Add(sellers_Status1);
                        }
                    */
                        db.SaveChanges();
                        //Email the status to customer
                        var cus_email = db.customers.Find(booking_Details.customer_id);
                        var property = db.properties.Find(booking_Details.property_id);
                        string email_body = "Your request for the property \" " + CultureInfo.CurrentCulture.TextInfo.ToTitleCase(property.property_name) + " \" was " + booking_Details.booking_status + " by the seller.\n" +
                        "For more information Contact seller :- \n Seller Name : " + CultureInfo.CurrentCulture.TextInfo.ToTitleCase(property.seller.seller_name) + " \n Contact No : " + property.seller.seller_phone;
                        string sendemail = ConfigurationManager.AppSettings["SendEmail"];
                        if (sendemail.ToLower() == "true")
                        {
                            SendEmail.Send(cus_email.customer_email, email_body);
                        }
                    }
                    return RedirectToAction("Index", "Bookings",new { booking_Details.property_id});
                }
                return RedirectToAction("Index", "Home");
        }

        // POST: bookings/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "si_no,property_id,customer_id,booking_status,book_date")] booking_details booking_details)
        {
            try
            {
                if (Session["user_role"] != null)
                {
                    if (ModelState.IsValid)
                    {
                        booking_details.booking_status = booking_details.booking_status.ToLower();
                        db.Entry(booking_details).State = EntityState.Modified;
                        db.SaveChanges();
                        return RedirectToAction("Index");
                    }
                    ViewBag.customer_id = new SelectList(db.customers, "customer_id", "customer_name", booking_details.customer_id);
                    ViewBag.property_id = new SelectList(db.properties, "property_id", "property_type", booking_details.property_id);
                    return View(booking_details);
                }
                return RedirectToAction("Index", "Home");
            }
            catch(Exception)
            {
                return RedirectToAction("Index", "Home");
            }
        }

        // GET: bookings/Delete/5
        public ActionResult Delete(int? id)
        {
            try
            {
                if (Session["user_role"] != null)
                {
                    if (id == null)
                    {
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                    }
                    booking_details booking_details = db.booking_details.Find(id);
                    if (booking_details == null)
                    {
                        return HttpNotFound();
                    }
                    return View(booking_details);
                }
                return RedirectToAction("Index", "Home");
            }
            catch(Exception)
            {
                return RedirectToAction("Index", "Home");
            }
        }

        // POST: bookings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                if (Session["user_role"] != null)
                {
                    booking_details booking_details = db.booking_details.Find(id);

                    //Deletion of all cutomer req action in booking details
                    var customer_actions = db.booking_details.Where(s => s.customer_id == booking_details.customer_id && s.property_id==booking_details.property_id);
                    db.booking_details.RemoveRange(customer_actions);

                    var prop_name = db.properties.FirstOrDefault(s => s.property_id == booking_details.property_id);
                    var seller = db.sellers.FirstOrDefault(s => s.seller_id == prop_name.seller_id);
                    var customer_name = db.customers.FirstOrDefault(s => s.customer_id == booking_details.customer_id);
                    customers_status customers_Status = db.customers_status.FirstOrDefault(s => s.customer_id==booking_details.customer_id && s.property_id==prop_name.property_id);
                    if (customers_Status != null)
                    {
                        customers_Status.message = customer_name.customer_name + " Cancelled the request for the property " + prop_name.property_name;
                        db.Entry(customers_Status).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    else
                    {
                        customers_status customers_Status1 = new customers_status()
                        {
                            customer_id = booking_details.customer_id,
                            seller_id = seller.seller_id,
                            property_id = prop_name.property_id,
                            message = customer_name.customer_name + " Cancelled the request for the property " + prop_name.property_name
                        };
                        db.customers_status.Add(customers_Status1);
                        db.SaveChanges();

                    }

                    //Email the status to seller or Email the status to customer
                    //Also have the table to pass msg to seller [Booking canceled by the customer..]

                    return RedirectToAction("Index");
                }
                return RedirectToAction("Index", "Home");
            }
            catch(Exception)
            {
               // return Content(e.Message);
                return RedirectToAction("Index", "Home");
            }
        }
        //Get: booking/Shortlist/pid
        public ActionResult ShortListed(int? property_id)
        {
            try
            {
                var shortlisted = db.booking_details.Where(m => m.property_id==property_id && m.booking_status == "accepted").ToList();
                if(shortlisted.Count==0)
                {
                    ViewData["listcount"] = 0;
                }
                ViewData["property_id"] = property_id;
                return View(shortlisted);
                //return Content("No requests for this "+ property_id);
            }
            catch(Exception e)
            {
                return Content(e.Message);
            }
        }

        public ActionResult Book(int? si_no)
        {
            try
            {
                if (Session["user_role"] != null)
                {
                    if (si_no == null)
                    {
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                    }
                    booking_details booking_details = db.booking_details.Find(si_no);
                    if (booking_details == null)
                    {
                        return HttpNotFound();
                    }
                    if (booking_details.booking_status != "rejected")
                    {
                        return View(booking_details);
                    }
                    return RedirectToAction("Index", "Bookings", new { property_id = booking_details.property_id });
                }
                return RedirectToAction("Index", "Home");
            }
            catch (Exception)
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public ActionResult BookConfirm(int? si_no)
        {
            try
            {
                var booking_details = db.booking_details.FirstOrDefault(m => m.si_no == si_no);
                booking_details.booking_status = "booked";
                var property = db.properties.FirstOrDefault(m => m.property_id == booking_details.property_id);
                property.property_status = "booked";
                db.Entry(property).State = EntityState.Modified;
                var datetime = DateTime.Now.ToString("dd/MM/yyyy");
                booked_properties booked_Properties = new booked_properties()
                {
                    property_id = property.property_id,
                    property_name = property.property_name.ToLower(),
                    property_type = property.property_type.ToLower(),
                    property_address = property.property_address.ToLower(),
                    property_state = property.property_state.ToLower(),
                    property_city = property.property_city.ToLower(),
                    property_pin = property.property_pin,
                    bhk = property.bhk,
                    seller_id = property.seller_id,
                    seller_name = property.seller.seller_name.ToLower(),
                    customer_id = booking_details.customer_id,
                    customer_name = booking_details.customer.customer_name.ToLower(),
                    book_date = DateTime.Parse(datetime)
                };
                db.booked_properties.Add(booked_Properties);
                db.SaveChanges();
                var changeforothers = db.booking_details.Where(m => m.property_id == booking_details.property_id && m.booking_status != "booked");
                foreach (var item in changeforothers)
                {
                    item.booking_status = "booked for others";
                    db.Entry(item).State = EntityState.Modified;
                }
                db.SaveChanges();
                var customer_det = db.customers.Find(booked_Properties.customer_id);
                var seller_det = db.sellers.Find(booked_Properties.seller_id);
                string email_body = "Property \"" + booked_Properties.property_name + "\" successfully booked for the customer : " + booked_Properties.customer_name + "\n For more information contact, \n Seller Name : "
                        + booked_Properties.seller_name + "\nSeller Email : " + seller_det.seller_email + "\nSeller Phone : " + seller_det.seller_phone;
                string sendemail = ConfigurationManager.AppSettings["SendEmail"];
                if (sendemail.ToLower() == "true")
                {
                    SendEmail.Send(customer_det.customer_email, email_body);
                }
                return RedirectToAction("Index", "Sellers");
                //return RedirectToAction("ShortListed", new { property_id = booking_details.property_id });
            }
            catch (Exception e)
            {
                return Content(e.Message);
            }
        }

        public ActionResult DeleteBooking(int? si_no)
        {
            try
            {
                if (Session["user_role"] != null)
                {
                    if (si_no == null)
                    {
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                    }
                    booking_details booking_details = db.booking_details.Find(si_no);
                    if (booking_details == null)
                    {
                        return HttpNotFound();
                    }
                    return View(booking_details);
                }
                return RedirectToAction("Index", "Home");
            }
            catch (Exception)
            {
                return RedirectToAction("Index", "Home");
            }

        }
        public ActionResult RequestCancel(int? si_no)
        {
            booking_details booking_details = db.booking_details.Find(si_no);
            db.booking_details.Remove(booking_details);
            var prop_name = db.properties.FirstOrDefault(s => s.property_id == booking_details.property_id);
            var seller = db.sellers.FirstOrDefault(s => s.seller_id == prop_name.seller_id);
            var customer_name = db.customers.FirstOrDefault(s => s.customer_id == booking_details.customer_id);
            customers_status customers_Status = db.customers_status.FirstOrDefault(s => s.customer_id == booking_details.customer_id && s.property_id == prop_name.property_id);
            if (customers_Status != null)
            {
                customers_Status.message = customer_name.customer_name + " Cancelled the Booking for the property " +prop_name.property_name;
                db.Entry(customers_Status).State = EntityState.Modified;
                db.SaveChanges();
            }
            else
            {
                customers_status customers_Status1 = new customers_status()
                {
                    customer_id = booking_details.customer_id,
                    seller_id = seller.seller_id,
                    message = customer_name.customer_name + " Cancelled the Booking for the property " + prop_name.property_name,
                    property_id = prop_name.property_id
                };
                db.customers_status.Add(customers_Status1);
                db.SaveChanges();

            }
            DeletionOfCancelledBookings(booking_details.property_id);
            return RedirectToAction("Index");

        }
        [NonAction]
        public void DeletionOfCancelledBookings(int? prop_id)
        {
            var property = db.properties.FirstOrDefault(s => s.property_id == prop_id);
            property.property_status = "available";
            var booked_property = db.booked_properties.FirstOrDefault(s => s.property_id == prop_id);
            db.booked_properties.Remove(booked_property);
            db.Entry(property).State = EntityState.Modified;
            db.SaveChanges();
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
